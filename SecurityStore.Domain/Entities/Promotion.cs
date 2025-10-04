using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// تبلیغات و تخفیف‌ها
// Promotions and discounts
public class Promotion : BaseEntity
{
    // عنوان
    public string Title { get; set; } = string.Empty;
    
    // توضیحات
    public string? Description { get; set; }
    
    // کد تخفیف
    public string? Code { get; set; }
    
    // درصد تخفیف
    public decimal? DiscountPercentage { get; set; }
    
    // مبلغ تخفیف ثابت
    public decimal? DiscountAmount { get; set; }
    
    // حداقل مبلغ خرید
    public decimal? MinimumPurchase { get; set; }
    
    // تاریخ شروع
    public DateTime StartDate { get; set; }
    
    // تاریخ پایان
    public DateTime EndDate { get; set; }
    
    // فعال بودن
    public bool IsActive { get; set; } = true;
    
    // تعداد استفاده مجاز
    public int? MaxUsageCount { get; set; }
    
    // تعداد استفاده شده
    public int UsageCount { get; set; }
}
