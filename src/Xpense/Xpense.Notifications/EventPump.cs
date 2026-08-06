using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Xpense.Notifications;

/// <summary>
/// Decides when to look for work. All it does is call <see cref="EventProcessor"/> in a loop -- what
/// happens to an event lives there, so it can be driven a batch at a time without a background loop.
/// </summary>
public sealed class EventPump(
    IServiceScopeFactory scopes,
    ILogger<EventPump> logger) : BackgroundService
{
    /// <summary>
    /// How long to wait after a partial batch. A full one skips the wait, so a burst drains at full
    /// speed while an idle queue costs one cheap indexed query per second.
    /// </summary>
    private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(1);

    /// <summary>How long to wait after an unexpected failure, so an unreachable database is not hammered.</summary>
    private static readonly TimeSpan ErrorDelay = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("Event pump started");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // A scope per batch, so the DbContext and its change tracker do not live for the
                // lifetime of the process and accumulate every entity ever loaded.
                using var scope = scopes.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<EventProcessor>();

                if (await processor.ProcessBatch(ct) < EventProcessor.BatchSize)
                    await Task.Delay(IdleDelay, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                // Never let the loop die. A failure here is an unreachable database or a bug, and in
                // both cases stopping means events pile up silently until somebody notices.
                logger.LogError(exception, "Event pump iteration failed");
                await Task.Delay(ErrorDelay, ct);
            }
        }

        logger.LogInformation("Event pump stopped");
    }
}
