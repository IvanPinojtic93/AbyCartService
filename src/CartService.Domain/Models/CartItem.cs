namespace CartService.Domain.Models;

public record CartItem(Guid ProductId, string ProductName, double Price, int Quantity, Guid StockItemId);
