using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Infrastructure.Data;

// مقداردهی اولیه دیتابیس
// Database initializer
public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // اطمینان از ساخت دیتابیس
        // Ensure database is created
        await context.Database.MigrateAsync();
        
        // ایجاد نقش‌ها
        // Create roles
        await SeedRolesAsync(roleManager);
        
        // ایجاد کاربران پیش‌فرض
        // Create default users
        await SeedUsersAsync(userManager);
        
        // ایجاد دسته‌بندی‌ها
        // Create categories
        await SeedCategoriesAsync(context);
        
        // ایجاد محصولات نمونه
        // Create sample products
        await SeedProductsAsync(context);
        
        // ایجاد تنظیمات سایت
        // Create site settings
        await SeedSiteSettingsAsync(context);
        
        // ایجاد منوها
        // Create menus
        await SeedMenuItemsAsync(context);
    }
    
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // ایجاد نقش‌های سیستم
        // Create system roles
        string[] roles = { "Admin", "Manager", "Customer" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        // ایجاد کاربر ادمین
        // Create admin user
        if (await userManager.FindByEmailAsync("admin@securitystore.com") == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@securitystore.com",
                Email = "admin@securitystore.com",
                EmailConfirmed = true,
                FullName = "مدیر سیستم - System Admin"
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        
        // ایجاد کاربر مدیر
        // Create manager user
        if (await userManager.FindByEmailAsync("manager@securitystore.com") == null)
        {
            var managerUser = new ApplicationUser
            {
                UserName = "manager@securitystore.com",
                Email = "manager@securitystore.com",
                EmailConfirmed = true,
                FullName = "مدیر فروشگاه - Store Manager"
            };
            
            var result = await userManager.CreateAsync(managerUser, "Manager@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }
    }
    
    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;
        
        var categories = new List<Category>
        {
            new Category
            {
                Name = "دوربین مداربسته - CCTV Cameras",
                Slug = "cctv-cameras",
                Description = "انواع دوربین‌های مداربسته و سیستم‌های نظارتی",
                DisplayOrder = 1,
                Icon = "bi-camera-video"
            },
            new Category
            {
                Name = "دزدگیر - Alarm Systems",
                Slug = "alarm-systems",
                Description = "سیستم‌های هشدار و دزدگیر",
                DisplayOrder = 2,
                Icon = "bi-bell"
            },
            new Category
            {
                Name = "سنسورها - Sensors",
                Slug = "sensors",
                Description = "انواع سنسورهای امنیتی",
                DisplayOrder = 3,
                Icon = "bi-wifi"
            },
            new Category
            {
                Name = "کنترل تردد - Access Control",
                Slug = "access-control",
                Description = "سیستم‌های کنترل دسترسی و تردد",
                DisplayOrder = 4,
                Icon = "bi-door-open"
            },
            new Category
            {
                Name = "لوازم جانبی - Accessories",
                Slug = "accessories",
                Description = "لوازم جانبی و تجهیزات مکمل",
                DisplayOrder = 5,
                Icon = "bi-tools"
            }
        };
        
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;
        
        var cctvCategory = await context.Categories.FirstAsync(c => c.Slug == "cctv-cameras");
        var alarmCategory = await context.Categories.FirstAsync(c => c.Slug == "alarm-systems");
        var sensorCategory = await context.Categories.FirstAsync(c => c.Slug == "sensors");
        
        var products = new List<Product>
        {
            new Product
            {
                Name = "دوربین مداربسته 2 مگاپیکسل - 2MP CCTV Camera",
                ShortDescription = "دوربین مداربسته با کیفیت 1080p و دید در شب",
                LongDescription = "دوربین مداربسته با کیفیت Full HD، دید در شب تا 30 متر، مقاوم در برابر آب و هوا، قابلیت اتصال به DVR",
                Slug = "2mp-cctv-camera",
                Sku = "CAM-2MP-001",
                Price = 1500000,
                StockQuantity = 50,
                CategoryId = cctvCategory.Id,
                Brand = "Hikvision",
                IsFeatured = true
            },
            new Product
            {
                Name = "دوربین مداربسته 4 مگاپیکسل - 4MP CCTV Camera",
                ShortDescription = "دوربین مداربسته با کیفیت 4K و دید در شب",
                LongDescription = "دوربین مداربسته با کیفیت 4K، دید در شب تا 40 متر، مقاوم در برابر آب و هوا، قابلیت اتصال شبکه",
                Slug = "4mp-cctv-camera",
                Sku = "CAM-4MP-001",
                Price = 2500000,
                DiscountPrice = 2200000,
                StockQuantity = 30,
                CategoryId = cctvCategory.Id,
                Brand = "Dahua",
                IsFeatured = true
            },
            new Product
            {
                Name = "دستگاه دزدگیر اماکن - Building Alarm System",
                ShortDescription = "سیستم دزدگیر کامل برای اماکن مسکونی و تجاری",
                LongDescription = "دستگاه دزدگیر با 8 زون، قابلیت اتصال به تلفن همراه، سیرن داخلی و خارجی، باتری پشتیبان",
                Slug = "building-alarm-system",
                Sku = "ALM-BLD-001",
                Price = 3500000,
                StockQuantity = 20,
                CategoryId = alarmCategory.Id,
                Brand = "DSC"
            },
            new Product
            {
                Name = "سنسور حرکتی - Motion Sensor",
                ShortDescription = "سنسور تشخیص حرکت با فناوری PIR",
                LongDescription = "سنسور حرکتی مادون قرمز، برد تشخیص تا 12 متر، قابل تنظیم حساسیت",
                Slug = "motion-sensor",
                Sku = "SNS-MOT-001",
                Price = 250000,
                StockQuantity = 100,
                CategoryId = sensorCategory.Id,
                Brand = "Paradox"
            },
            new Product
            {
                Name = "سنسور درب و پنجره - Door/Window Sensor",
                ShortDescription = "سنسور مغناطیسی برای درب و پنجره",
                LongDescription = "سنسور تشخیص باز شدن درب و پنجره، نصب آسان، باتری طولانی مدت",
                Slug = "door-window-sensor",
                Sku = "SNS-DOR-001",
                Price = 150000,
                StockQuantity = 150,
                CategoryId = sensorCategory.Id,
                Brand = "Ajax"
            }
        };
        
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedSiteSettingsAsync(ApplicationDbContext context)
    {
        if (await context.SiteSettings.AnyAsync())
            return;
        
        var settings = new List<SiteSetting>
        {
            new SiteSetting
            {
                Key = "SiteName",
                Value = "فروشگاه امنیتی - Security Store",
                Group = "General"
            },
            new SiteSetting
            {
                Key = "SiteDescription",
                Value = "فروشگاه آنلاین تجهیزات امنیتی، دوربین مداربسته، دزدگیر و سنسور",
                Group = "General"
            },
            new SiteSetting
            {
                Key = "ContactEmail",
                Value = "info@securitystore.com",
                Group = "Contact"
            },
            new SiteSetting
            {
                Key = "ContactPhone",
                Value = "021-12345678",
                Group = "Contact"
            },
            new SiteSetting
            {
                Key = "ShippingCost",
                Value = "50000",
                Group = "Shipping"
            },
            new SiteSetting
            {
                Key = "FreeShippingThreshold",
                Value = "5000000",
                Group = "Shipping"
            }
        };
        
        await context.SiteSettings.AddRangeAsync(settings);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedMenuItemsAsync(ApplicationDbContext context)
    {
        if (await context.MenuItems.AnyAsync())
            return;
        
        var menuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Title = "خانه - Home",
                Url = "/",
                DisplayOrder = 1,
                Icon = "bi-house"
            },
            new MenuItem
            {
                Title = "محصولات - Products",
                Url = "/Products",
                DisplayOrder = 2,
                Icon = "bi-box"
            },
            new MenuItem
            {
                Title = "درباره ما - About",
                Url = "/About",
                DisplayOrder = 3,
                Icon = "bi-info-circle"
            },
            new MenuItem
            {
                Title = "تماس با ما - Contact",
                Url = "/Contact",
                DisplayOrder = 4,
                Icon = "bi-telephone"
            }
        };
        
        await context.MenuItems.AddRangeAsync(menuItems);
        await context.SaveChangesAsync();
    }
}
