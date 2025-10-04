namespace SecurityStore.Application.DTOs;

// DTO آیتم سبد خرید
// Cart item DTO
public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal TotalPrice => Price * Quantity;
}
