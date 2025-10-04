using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// دسته‌بندی محصولات
// Product category entity
public class Category : BaseEntity
{
    // نام دسته‌بندی
    public string Name { get; set; } = string.Empty;
    
    // توضیحات
    public string? Description { get; set; }
    
    // نامک URL (برای آدرس دوستانه)
    public string Slug { get; set; } = string.Empty;
    
    // آیکون یا تصویر
    public string? Icon { get; set; }
    
    // شماره ترتیب نمایش
    public int DisplayOrder { get; set; }
    
    // فعال بودن دسته‌بندی
    public bool IsActive { get; set; } = true;
    
    // دسته‌بندی والد (برای ساختار سلسله مراتبی)
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    
    // لیست محصولات این دسته‌بندی
    public ICollection<Product> Products { get; set; } = new List<Product>();
    
    // زیر دسته‌بندی‌ها
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}
