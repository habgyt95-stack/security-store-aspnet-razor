using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// نظرات محصول
// Product reviews
public class ProductReview : BaseEntity
{
    // شناسه محصول
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    // شناسه کاربر
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    
    // امتیاز (1 تا 5)
    public int Rating { get; set; }
    
    // عنوان نظر
    public string? Title { get; set; }
    
    // متن نظر
    public string Comment { get; set; } = string.Empty;
    
    // تأیید شده توسط مدیر
    public bool IsApproved { get; set; }
    
    // نمایش نام کاربر
    public bool ShowUserName { get; set; } = true;
}
