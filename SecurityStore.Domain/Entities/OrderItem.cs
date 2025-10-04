using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// آیتم سفارش
// Order item entity
public class OrderItem : BaseEntity
{
    // شناسه سفارش
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    // شناسه محصول
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    // نام محصول (snapshot)
    public string ProductName { get; set; } = string.Empty;
    
    // قیمت واحد (snapshot)
    public decimal UnitPrice { get; set; }
    
    // تعداد
    public int Quantity { get; set; }
    
    // قیمت کل این آیتم
    public decimal TotalPrice { get; set; }
}
