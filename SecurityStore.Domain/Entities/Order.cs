using SecurityStore.Domain.Common;
using SecurityStore.Domain.Enums;

namespace SecurityStore.Domain.Entities;

// موجودیت سفارش
// Order entity
public class Order : BaseEntity
{
    // شماره سفارش
    public string OrderNumber { get; set; } = string.Empty;
    
    // شناسه کاربر
    public string UserId { get; set; } = string.Empty;
    
    // وضعیت سفارش
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // مجموع قیمت آیتم‌ها
    public decimal SubTotal { get; set; }
    
    // هزینه ارسال
    public decimal ShippingCost { get; set; }
    
    // تخفیف
    public decimal Discount { get; set; }
    
    // مالیات
    public decimal Tax { get; set; }
    
    // مجموع نهایی
    public decimal TotalAmount { get; set; }
    
    // آدرس ارسال
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingPostalCode { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    
    // یادداشت‌های سفارش
    public string? Notes { get; set; }
    
    // تاریخ پرداخت
    public DateTime? PaidAt { get; set; }
    
    // روش پرداخت
    public string? PaymentMethod { get; set; }
    
    // شناسه تراکنش
    public string? TransactionId { get; set; }
    
    // آیتم‌های سفارش
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
