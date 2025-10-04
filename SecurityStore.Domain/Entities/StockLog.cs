using SecurityStore.Domain.Common;
using SecurityStore.Domain.Enums;

namespace SecurityStore.Domain.Entities;

// گزارش تغییرات موجودی
// Stock changes log
public class StockLog : BaseEntity
{
    // شناسه محصول
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    // نوع عملیات
    public StockAction Action { get; set; }
    
    // تعداد (مثبت برای افزایش، منفی برای کاهش)
    public int Quantity { get; set; }
    
    // موجودی قبلی
    public int PreviousStock { get; set; }
    
    // موجودی جدید
    public int NewStock { get; set; }
    
    // توضیحات
    public string? Notes { get; set; }
    
    // کاربر انجام‌دهنده عملیات
    public string? UserId { get; set; }
}
