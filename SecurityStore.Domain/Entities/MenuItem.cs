using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// آیتم منو
// Menu item entity
public class MenuItem : BaseEntity
{
    // عنوان
    public string Title { get; set; } = string.Empty;
    
    // لینک
    public string Url { get; set; } = string.Empty;
    
    // ترتیب نمایش
    public int DisplayOrder { get; set; }
    
    // آیکون
    public string? Icon { get; set; }
    
    // باز شدن در تب جدید
    public bool OpenInNewTab { get; set; }
    
    // فعال بودن
    public bool IsActive { get; set; } = true;
    
    // والد (برای منوی چندسطحی)
    public int? ParentMenuItemId { get; set; }
    public MenuItem? ParentMenuItem { get; set; }
    
    // زیرمنوها
    public ICollection<MenuItem> SubMenuItems { get; set; } = new List<MenuItem>();
}
