using Microsoft.AspNetCore.Identity;

namespace SecurityStore.Infrastructure.Identity;

// کاربر اپلیکیشن
// Application user
public class ApplicationUser : IdentityUser
{
    // نام کامل
    public string? FullName { get; set; }
    
    // آدرس
    public string? Address { get; set; }
    
    // شهر
    public string? City { get; set; }
    
    // کد پستی
    public string? PostalCode { get; set; }
    
    // تاریخ ثبت‌نام
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
