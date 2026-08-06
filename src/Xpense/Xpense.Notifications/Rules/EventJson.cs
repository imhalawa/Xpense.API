using System.Text.Json;

namespace Xpense.Notifications.Rules;

/// <summary>
/// One set of serialization options for everything on this side of the wire.
/// <para>
/// Must match what <c>EventBus</c> writes, or a stored body will not read back. Shared through a
/// single field rather than constructed per call, because a new <see cref="JsonSerializerOptions"/>
/// per operation defeats the reflection cache behind it.
/// </para>
/// </summary>
public static class EventJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
