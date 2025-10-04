using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// تصاویر محصول
// Product image entity
public class ProductImage : BaseEntity
{
    // مسیر تصویر
    public string ImageUrl { get; set; } = string.Empty;
    
    // متن جایگزین
    public string? AltText { get; set; }
    
    // ترتیب نمایش
    public int DisplayOrder { get; set; }
    
    // شناسه محصول
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
