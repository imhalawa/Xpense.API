using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Xpense.Notifications;

public sealed class EventPump(
    IServiceScopeFactory scopes,
    ILogger<EventPump> logger) : BackgroundService
{
    private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(1);

    private static readonly TimeSpan ErrorDelay = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event pump started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // A scope per batch, so the DbContext and its change tracker do not live for the
                // lifetime of the process and accumulate every entity ever loaded.
                using var scope = scopes.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<EventProcessor>();

                if (await processor.ProcessBatch(cancellationToken) < EventProcessor.BatchSize)
                    await Task.Delay(IdleDelay, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                // Never let the loop die. A failure here is an unreachable database or a bug, and in
                // both cases stopping means events pile up silently until somebody notices.
                logger.LogError(exception, "Event pump iteration failed");
                await Task.Delay(ErrorDelay, cancellationToken);
            }
        }

        logger.LogInformation("Event pump stopped");
    }
}
