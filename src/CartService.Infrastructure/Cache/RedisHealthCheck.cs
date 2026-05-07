using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace CartService.Infrastructure.Cache;

public sealed class RedisHealthCheck(IConnectionMultiplexer mux) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(mux.IsConnected
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Redis not connected"));
}
