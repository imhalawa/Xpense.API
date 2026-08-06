using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Xpense.API.Infrastructure;
using Xpense.Persistence;

namespace Xpense.API.Features.Notifications;

/// <summary>
/// How many notifications are unread, and nothing else. A badge polls this, so it stays one indexed
/// count rather than a page of rows the caller throws away.
/// </summary>
public sealed class GetUnreadNotificationCount : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/v1/notifications/unread-count", Handle)
            .WithName(nameof(GetUnreadNotificationCount));

    private static async Task<Ok<UnreadCountResponse>> Handle(XpenseDbContext db, CancellationToken ct) =>
        TypedResults.Ok(new UnreadCountResponse(
            await db.Notifications.CountAsync(notification => notification.ReadAt == null, ct)));
}
