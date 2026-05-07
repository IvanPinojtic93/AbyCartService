using CartService.Application.Common.Interfaces;
using CartService.Domain.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace CartService.Infrastructure.Cache;

public sealed class CartRepository(IConnectionMultiplexer mux) : ICartRepository
{
    private static string Key(string userId) => $"cart:{userId}";

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<CartItem[]?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var db = mux.GetDatabase();
        var value = await db.StringGetAsync(Key(userId));
        if (!value.HasValue) return null;

        return JsonSerializer.Deserialize<CartItem[]>(value.ToString(), JsonOpts);
    }

    public async Task SetAsync(string userId, CartItem[] items, CancellationToken cancellationToken = default)
    {
        var db = mux.GetDatabase();
        await db.StringSetAsync(Key(userId), JsonSerializer.Serialize(items), TimeSpan.FromDays(7));
    }

    public async Task DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var db = mux.GetDatabase();
        await db.KeyDeleteAsync(Key(userId));
    }
}
