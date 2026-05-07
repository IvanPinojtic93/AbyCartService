using CartService.Infrastructure.Cache;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace CartService.Tests.Infrastructure.HealthChecks;

public class RedisHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenConnected_ReturnsHealthy()
    {
        var mux = Substitute.For<IConnectionMultiplexer>();
        mux.IsConnected.Returns(true);

        var check = new RedisHealthCheck(mux);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenDisconnected_ReturnsUnhealthy()
    {
        var mux = Substitute.For<IConnectionMultiplexer>();
        mux.IsConnected.Returns(false);

        var check = new RedisHealthCheck(mux);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be("Redis not connected");
    }
}
