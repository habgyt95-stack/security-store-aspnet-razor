using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Admin;

// داشبورد مدیریت
// Admin dashboard
[Authorize(Roles = "Admin,Manager")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<Order> RecentOrders { get; set; } = new();

    public async Task OnGetAsync()
    {
        // محاسبه شاخص‌های کلیدی
        // Calculate KPIs
        TotalProducts = await _context.Products.CountAsync();
        TotalOrders = await _context.Orders.CountAsync();
        
        var customers = await _userManager.GetUsersInRoleAsync("Customer");
        TotalCustomers = customers.Count;
        
        TotalRevenue = await _context.Orders
            .Where(o => o.Status == Domain.Enums.OrderStatus.Paid)
            .SumAsync(o => o.TotalAmount);

        // بارگذاری سفارشات اخیر
        // Load recent orders
        RecentOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .ToListAsync();
    }
}
