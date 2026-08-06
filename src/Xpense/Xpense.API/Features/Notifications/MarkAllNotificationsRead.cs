using System;
using System.Linq;
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
/// Marks every unread notification read, and reports how many that was.
/// <para>
/// POST rather than PATCH: there is no single resource being patched. The only bulk write in this
/// codebase, which is why it is one statement rather than a load-and-loop -- clearing a long backlog
/// should not mean tracking every row to set one column.
/// </para>
/// </summary>
public sealed class MarkAllNotificationsRead : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/v1/notifications/read-all", Handle)
            .WithName(nameof(MarkAllNotificationsRead));

    private static async Task<Ok<MarkAllReadResponse>> Handle(XpenseDbContext db, CancellationToken ct)
    {
        var readAt = DateTime.UtcNow;

        // Only the unread ones, so an already-read notification keeps the time it was first seen.
        var marked = await db.Notifications
            .Where(notification => notification.ReadAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(notification => notification.ReadAt, readAt)
                    .SetProperty(notification => notification.UpdatedAt, readAt),
                ct);

        return TypedResults.Ok(new MarkAllReadResponse(marked));
    }
}

public sealed record MarkAllReadResponse(int Marked);
