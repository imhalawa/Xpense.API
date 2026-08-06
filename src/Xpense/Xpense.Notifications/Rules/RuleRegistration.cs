using Microsoft.Extensions.DependencyInjection;

namespace Xpense.Notifications.Rules;

public static class RuleRegistration
{
    /// <summary>
    /// Finds every rule in this assembly and registers it, plus a dispatcher for each body type any
    /// rule cares about.
    /// <para>
    /// A scan rather than a list, for the reason <c>MapEndpoints</c> scans for slices: a registration
    /// list is a second place to remember, and the failure when you forget is a rule that silently
    /// never runs.
    /// </para>
    /// </summary>
    public static IServiceCollection AddNotificationRules(this IServiceCollection services)
    {
        var ruleInterface = typeof(INotificationRule<>);

        var closedRuleInterfaces = typeof(RuleRegistration).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(type => type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType
                                     && @interface.GetGenericTypeDefinition() == ruleInterface)
                .Select(@interface => (Implementation: type, Interface: @interface)))
            .ToArray();

        foreach (var (implementation, @interface) in closedRuleInterfaces)
            services.AddScoped(@interface, implementation);

        // One dispatcher per body type, however many rules that type has.
        foreach (var bodyType in closedRuleInterfaces
                     .Select(rule => rule.Interface.GetGenericArguments()[0])
                     .Distinct())
        {
            services.AddScoped(
                typeof(IEventDispatcher),
                typeof(EventDispatcher<>).MakeGenericType(bodyType));
        }

        return services;
    }
}
