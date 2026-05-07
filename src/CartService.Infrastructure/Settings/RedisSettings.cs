namespace CartService.Infrastructure.Settings;

public sealed class RedisSettings
{
    public required string ConnectionString { get; init; }
}
