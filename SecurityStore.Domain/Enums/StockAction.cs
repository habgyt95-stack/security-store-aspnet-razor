namespace SecurityStore.Domain.Enums;

// نوع عملیات موجودی انبار
// Stock operation type
public enum StockAction
{
    // دریافت از تامین‌کننده
    Received = 0,
    
    // فروش به مشتری
    Sold = 1,
    
    // مرجوعی از مشتری
    Returned = 2,
    
    // تنظیم دستی موجودی
    Adjusted = 3,
    
    // آسیب دیده
    Damaged = 4
}
