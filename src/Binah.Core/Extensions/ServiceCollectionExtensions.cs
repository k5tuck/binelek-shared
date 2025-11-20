using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace Binah.Core.Extensions;

/// <summary>
/// Extension methods for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add resilient HTTP client with retry and circuit breaker policies
    /// </summary>
    public static IHttpClientBuilder AddResilientHttpClient(
        this IServiceCollection services,
        string name,
        string baseUrl,
        int retryCount = 3,
        int circuitBreakerThreshold = 5,
        int circuitBreakerDurationSeconds = 30)
    {
        return services.AddHttpClient(name, client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount))
        .AddPolicyHandler(GetCircuitBreakerPolicy(circuitBreakerThreshold, circuitBreakerDurationSeconds));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log retry attempt
                    Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
        int threshold,
        int durationSeconds)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                threshold,
                TimeSpan.FromSeconds(durationSeconds),
                onBreak: (outcome, breakDelay) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {breakDelay.TotalSeconds}s");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                });
    }
}
