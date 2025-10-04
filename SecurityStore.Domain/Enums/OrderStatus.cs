namespace SecurityStore.Domain.Enums;

// وضعیت سفارش
// Order status enumeration
public enum OrderStatus
{
    // در انتظار پرداخت
    Pending = 0,
    
    // پرداخت شده
    Paid = 1,
    
    // در حال پردازش
    Processing = 2,
    
    // ارسال شده
    Shipped = 3,
    
    // تحویل داده شده
    Delivered = 4,
    
    // لغو شده
    Cancelled = 5,
    
    // مرجوع شده
    Returned = 6
}
