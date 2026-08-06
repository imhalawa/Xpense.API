using System.Linq;
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
/// Notifications, newest first, paged. <c>?unread=true</c> narrows to the ones not yet seen.
/// <para>
/// The unread total is returned alongside the page so a client rendering a list and a badge together
/// needs one request rather than two.
/// </para>
/// </summary>
public sealed class ListNotifications : IEndpoint
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 25;

    /// <summary>
    /// A ceiling, unlike ListTransactions which has none. Without one, ?pageSize=1000000 reads the
    /// whole table in one request, and notifications are the table most likely to grow without bound.
    /// </summary>
    private const int MaxPageSize = 100;

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/v1/notifications", Handle).WithName(nameof(ListNotifications));

    private static async Task<Ok<NotificationPageResponse>> Handle(
        XpenseDbContext db,
        CancellationToken ct,
        int page = DefaultPage,
        int pageSize = DefaultPageSize,
        bool unread = false)
    {
        if (page <= 0 || pageSize <= 0 || pageSize > MaxPageSize)
            throw new InvalidFilteredResultParams(page, pageSize);

        var all = db.Notifications.AsNoTracking();
        var selected = unread ? all.Where(notification => notification.ReadAt == null) : all;

        var totalItems = await selected.CountAsync(ct);
        var totalPages = totalItems / pageSize + (totalItems % pageSize > 0 ? 1 : 0);
        var unreadItems = await all.CountAsync(notification => notification.ReadAt == null, ct);

        var notifications = await selected
            // By when Xpense decided to say it, not by when the underlying money moved: a notification
            // about a backdated expense is news today.
            .OrderByDescending(notification => notification.CreatedAt)
            .ThenByDescending(notification => notification.Id)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync(ct);

        return TypedResults.Ok(new NotificationPageResponse(
            notifications.Select(NotificationResponse.Of).ToArray(),
            page,
            pageSize,
            totalItems,
            totalPages,
            unreadItems));
    }
}
