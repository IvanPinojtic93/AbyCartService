using CartService.Domain.Models;

namespace CartService.Application.Common.Interfaces;

public interface ICartRepository
{
    Task<CartItem[]?> GetAsync(string userId, CancellationToken cancellationToken = default);
    Task SetAsync(string userId, CartItem[] items, CancellationToken cancellationToken = default);
    Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
}
