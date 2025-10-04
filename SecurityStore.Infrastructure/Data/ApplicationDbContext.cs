using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Application.Interfaces;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Infrastructure.Data;

// کانتکست دیتابیس اپلیکیشن
// Application database context
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<StockLog> StockLogs => Set<StockLog>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // پیکربندی موجودیت‌ها
        // Configure entities
        
        // Category
        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.SubCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Product
        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Sku).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.DiscountPrice).HasPrecision(18, 2);
        });
        
        // Order
        builder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.ShippingCost).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.Tax).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
        });
        
        // OrderItem
        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
        });
        
        // Promotion
        builder.Entity<Promotion>(entity =>
        {
            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.MinimumPurchase).HasPrecision(18, 2);
        });
        
        // ProductVariant
        builder.Entity<ProductVariant>(entity =>
        {
            entity.Property(e => e.PriceAdjustment).HasPrecision(18, 2);
        });
        
        // MenuItem
        builder.Entity<MenuItem>(entity =>
        {
            entity.HasOne(e => e.ParentMenuItem)
                .WithMany(e => e.SubMenuItems)
                .HasForeignKey(e => e.ParentMenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
