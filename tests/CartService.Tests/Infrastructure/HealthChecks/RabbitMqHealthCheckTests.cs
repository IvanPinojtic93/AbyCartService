using CartService.Infrastructure.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute.ExceptionExtensions;
using RabbitMQ.Client;

namespace CartService.Tests.Infrastructure.HealthChecks;

public class RabbitMqHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenConnectionSucceeds_ReturnsHealthy()
    {
        var factory = Substitute.For<IConnectionFactory>();
        factory.CreateConnection().Returns(Substitute.For<IConnection>());

        var check = new RabbitMqHealthCheck(factory);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenConnectionFails_ReturnsUnhealthy()
    {
        var factory = Substitute.For<IConnectionFactory>();
        factory.CreateConnection().Throws(new Exception("Connection refused"));

        var check = new RabbitMqHealthCheck(factory);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be("Connection refused");
    }
}
