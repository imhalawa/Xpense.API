using FluentAssertions;
using Mono.Cecil;
using Xpense.Domain.Events;

namespace Xpense.Tests.Architecture;

/// <summary>
/// Event bodies live in Xpense.Domain, beside the entities, which is convenient and one careless
/// property away from a problem: an event is a wire format, written now and read back possibly much
/// later, so a body holding an entity pins the queue's contents to the schema. Rename a property and
/// every unprocessed event stops deserializing.
/// <para>
/// Keeping them out of a project of their own was a deliberate choice to avoid a fifth project
/// holding two files. This test is what buys back the safety that choice gave up -- the same argument
/// SliceIsolationTests makes about a convention in a README being weaker than a build error.
/// </para>
/// </summary>
[TestFixture]
public class EventContractTests
{
    private const string EntitiesNamespace = "Xpense.Domain.Entities";
    private const string EventsNamespace = "Xpense.Domain.Events";

    private static ModuleDefinition module = null!;

    [OneTimeSetUp]
    public void LoadAssembly() => module = ModuleDefinition.ReadModule(typeof(EventBody).Assembly.Location);

    [OneTimeTearDown]
    public void Dispose() => module?.Dispose();

    [Test]
    public void No_event_body_holds_an_entity()
    {
        var violations = new List<string>();

        foreach (var body in Bodies())
        {
            foreach (var property in body.Properties)
            {
                if (Mentions(property.PropertyType, EntitiesNamespace))
                    violations.Add($"{body.Name}.{property.Name} is {property.PropertyType.Name}");
            }
        }

        violations.Should().BeEmpty(
            "an event body is a wire format and must hold primitives, enums and ids only -- an entity "
            + "inside one ties unprocessed events to the current schema");
    }

    /// <summary>
    /// Money is the one to watch: it is a value object rather than an entity, so the check above
    /// misses it, and it is exactly the type someone would reach for on an event carrying an amount.
    /// Amounts travel as minor units plus a currency, the same way they do in every other contract.
    /// </summary>
    [Test]
    public void No_event_body_holds_a_value_object()
    {
        var violations = Bodies()
            .SelectMany(body => body.Properties
                .Where(property => Mentions(property.PropertyType, "Xpense.Domain.ValueObjects"))
                .Select(property => $"{body.Name}.{property.Name} is {property.PropertyType.Name}"))
            .ToList();

        violations.Should().BeEmpty(
            "amounts on the wire are minor units plus a currency, not a Money");
    }

    /// <summary>
    /// A body outside Events would still work, and would sit somewhere nobody thinks to look when
    /// asking what this system publishes.
    /// </summary>
    [Test]
    public void Every_event_body_lives_in_the_events_namespace()
    {
        var strays = Bodies()
            .Where(body => body.Namespace != EventsNamespace)
            .Select(body => body.FullName)
            .ToList();

        strays.Should().BeEmpty("event bodies belong in Xpense.Domain.Events");
    }

    private static IEnumerable<TypeDefinition> Bodies() =>
        module.GetTypes().Where(type =>
            type is { IsAbstract: false, IsInterface: false }
            && type.BaseType?.FullName == typeof(EventBody).FullName);

    /// <summary>
    /// Checks the type and, for a generic such as a collection, its arguments -- a
    /// <c>List&lt;Transaction&gt;</c> is as much a problem as a bare one.
    /// </summary>
    private static bool Mentions(TypeReference type, string @namespace)
    {
        if (type.Namespace == @namespace)
            return true;

        return type is GenericInstanceType generic
               && generic.GenericArguments.Any(argument => Mentions(argument, @namespace));
    }
}
