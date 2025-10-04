using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// فایل رسانه‌ای (برای مدیریت تصاویر و فایل‌ها)
// Media file entity (for managing images and files)
public class MediaFile : BaseEntity
{
    // نام فایل
    public string FileName { get; set; } = string.Empty;
    
    // مسیر فایل
    public string FilePath { get; set; } = string.Empty;
    
    // نوع MIME
    public string ContentType { get; set; } = string.Empty;
    
    // حجم فایل (بایت)
    public long FileSize { get; set; }
    
    // متن جایگزین
    public string? AltText { get; set; }
    
    // عنوان
    public string? Title { get; set; }
    
    // توضیحات
    public string? Description { get; set; }
    
    // کاربر آپلودکننده
    public string? UploadedBy { get; set; }
}
