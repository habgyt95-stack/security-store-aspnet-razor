using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;

namespace SecurityStore.Application.Interfaces;

// رابط دیتابیس اپلیکیشن
// Application database interface
public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<StockLog> StockLogs { get; }
    DbSet<Promotion> Promotions { get; }
    DbSet<SiteSetting> SiteSettings { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MediaFile> MediaFiles { get; }
    DbSet<AuditLog> AuditLogs { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
