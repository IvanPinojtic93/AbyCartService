using CartService.Domain.Models;

namespace CartService.Application.DTOs;

public record CartResponse(CartItem[] Items);
