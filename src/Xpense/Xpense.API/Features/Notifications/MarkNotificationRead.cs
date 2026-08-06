using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Xpense.API.Infrastructure;
using Xpense.Domain.Exceptions;
using Xpense.Persistence;

namespace Xpense.API.Features.Notifications;

/// <summary>
/// Marks one notification read.
/// <para>
/// PATCH rather than PUT: this changes one aspect of the resource and does not replace it. Idempotent
/// -- marking an already-read notification returns it unchanged rather than moving its timestamp, so
/// a client retrying a failed request cannot rewrite when you first saw something.
/// </para>
/// </summary>
public sealed class MarkNotificationRead : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPatch("/api/v1/notifications/{id:int}/read", Handle)
            .WithName(nameof(MarkNotificationRead));

    private static async Task<Ok<NotificationResponse>> Handle(
        int id,
        XpenseDbContext db,
        CancellationToken ct)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(item => item.Id == id, ct)
                           ?? throw new NotificationNotFoundException(id);

        notification.MarkAsRead();

        // No row-count check: marking an already-read notification changes nothing, so SaveChanges
        // returning 0 is the correct outcome rather than a failure.
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(NotificationResponse.Of(notification));
    }
}
