using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Binah.Core.Utilities;

/// <summary>
/// Factory for creating retry policies
/// </summary>
public static class RetryPolicyFactory
{
    /// <summary>
    /// Execute action with exponential backoff retry
    /// </summary>
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> action,
        int maxRetries = 3,
        int baseDelayMs = 1000,
        ILogger? logger = null)
    {
        var attempt = 0;
        Exception? lastException = null;

        while (attempt < maxRetries)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempt++;

                if (attempt >= maxRetries)
                {
                    logger?.LogError(ex, "Max retries ({MaxRetries}) reached", maxRetries);
                    throw;
                }

                var delay = CalculateExponentialBackoff(attempt, baseDelayMs);
                logger?.LogWarning(
                    ex,
                    "Attempt {Attempt} failed, retrying in {Delay}ms",
                    attempt,
                    delay);

                await Task.Delay(delay);
            }
        }

        throw lastException ?? new Exception("Retry failed with no exception");
    }

    /// <summary>
    /// Execute action with exponential backoff retry (void return)
    /// </summary>
    public static async Task ExecuteWithRetryAsync(
        Func<Task> action,
        int maxRetries = 3,
        int baseDelayMs = 1000,
        ILogger? logger = null)
    {
        await ExecuteWithRetryAsync(async () =>
        {
            await action();
            return true;
        }, maxRetries, baseDelayMs, logger);
    }

    private static int CalculateExponentialBackoff(int attempt, int baseDelayMs)
    {
        return baseDelayMs * (int)Math.Pow(2, attempt - 1);
    }
}
