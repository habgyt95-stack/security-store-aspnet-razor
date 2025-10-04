using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// انواع محصول (مثلاً رنگ، سایز و غیره)
// Product variants (e.g., color, size, etc.)
public class ProductVariant : BaseEntity
{
    // نام ویژگی (مثلاً: رنگ)
    public string AttributeName { get; set; } = string.Empty;
    
    // مقدار ویژگی (مثلاً: سیاه)
    public string AttributeValue { get; set; } = string.Empty;
    
    // قیمت اضافی (اگر این نوع گران‌تر باشد)
    public decimal? PriceAdjustment { get; set; }
    
    // موجودی این نوع
    public int StockQuantity { get; set; }
    
    // شناسه محصول
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
