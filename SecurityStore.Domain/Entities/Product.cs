using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// موجودیت محصول
// Product entity
public class Product : BaseEntity
{
    // نام محصول
    public string Name { get; set; } = string.Empty;
    
    // توضیحات کوتاه
    public string? ShortDescription { get; set; }
    
    // توضیحات کامل
    public string? LongDescription { get; set; }
    
    // نامک URL
    public string Slug { get; set; } = string.Empty;
    
    // شناسه محصول (SKU)
    public string Sku { get; set; } = string.Empty;
    
    // قیمت
    public decimal Price { get; set; }
    
    // قیمت تخفیف‌خورده
    public decimal? DiscountPrice { get; set; }
    
    // موجودی
    public int StockQuantity { get; set; }
    
    // تصویر اصلی
    public string? MainImage { get; set; }
    
    // برند
    public string? Brand { get; set; }
    
    // فعال بودن
    public bool IsActive { get; set; } = true;
    
    // ویژه بودن (Featured)
    public bool IsFeatured { get; set; }
    
    // تعداد بازدید
    public int ViewCount { get; set; }
    
    // دسته‌بندی
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    // تصاویر محصول
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    
    // انواع محصول (variants)
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    
    // گزارش‌های موجودی
    public ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
    
    // آیتم‌های سفارش
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    // نظرات محصول
    public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
}
