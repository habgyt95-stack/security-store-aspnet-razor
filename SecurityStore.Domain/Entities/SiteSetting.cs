using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// تنظیمات سایت
// Site settings
public class SiteSetting : BaseEntity
{
    // کلید تنظیم
    public string Key { get; set; } = string.Empty;
    
    // مقدار
    public string Value { get; set; } = string.Empty;
    
    // توضیحات
    public string? Description { get; set; }
    
    // گروه (برای دسته‌بندی تنظیمات)
    public string? Group { get; set; }
}
