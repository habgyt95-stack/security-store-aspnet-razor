using SecurityStore.Domain.Common;

namespace SecurityStore.Domain.Entities;

// گزارش حسابرسی
// Audit log entity
public class AuditLog : BaseEntity
{
    // شناسه کاربر
    public string? UserId { get; set; }
    
    // نام کاربری
    public string? UserName { get; set; }
    
    // نوع عملیات (Create, Update, Delete)
    public string Action { get; set; } = string.Empty;
    
    // نام موجودیت
    public string EntityName { get; set; } = string.Empty;
    
    // شناسه موجودیت
    public string? EntityId { get; set; }
    
    // مقادیر قدیم (JSON)
    public string? OldValues { get; set; }
    
    // مقادیر جدید (JSON)
    public string? NewValues { get; set; }
    
    // آدرس IP
    public string? IpAddress { get; set; }
    
    // User Agent
    public string? UserAgent { get; set; }
}
