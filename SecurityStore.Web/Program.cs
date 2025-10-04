using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Application.Interfaces;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;
using SecurityStore.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// اضافه کردن سرویس‌های پایه
// Add base services
builder.Services.AddRazorPages();

// اضافه کردن دیتابیس
// Add database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SecurityStore.Infrastructure")));

// اضافه کردن Identity
// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // تنظیمات رمز عبور
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    
    // تنظیمات کاربر
    options.User.RequireUniqueEmail = true;
    
    // تنظیمات قفل شدن
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// تنظیمات کوکی
// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// اضافه کردن Session
// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ثبت سرویس‌ها
// Register services
builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<IPaymentService, FakePaymentService>();

var app = builder.Build();

// مقداردهی اولیه دیتابیس
// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbInitializer.InitializeAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "خطا در مقداردهی اولیه دیتابیس - Error initializing database");
    }
}

// پیکربندی HTTP request pipeline
// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
