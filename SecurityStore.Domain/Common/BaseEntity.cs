namespace SecurityStore.Domain.Common;

// کلاس پایه برای تمام موجودیت‌های دامنه
// Base class for all domain entities
public abstract class BaseEntity
{
    public int Id { get; set; }
    
    // تاریخ ایجاد رکورد
    // Record creation date
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // تاریخ آخرین به‌روزرسانی
    // Last update date
    public DateTime? UpdatedAt { get; set; }
}
