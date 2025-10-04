using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Domain.Enums;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Profile;

// صفحه سفارشات من
// My orders page
[Authorize]
public class MyOrdersModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private const int PageSize = 10;

    public MyOrdersModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Order> Orders { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    
    public int TotalPages { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // دریافت تعداد کل سفارشات
        // Get total orders count
        var totalOrders = await _context.Orders
            .Where(o => o.UserId == user.Id)
            .CountAsync();

        TotalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

        // دریافت سفارشات با صفحه‌بندی
        // Get orders with pagination
        Orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Page();
    }

    public string GetStatusText(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "در انتظار پردازش",
            OrderStatus.Processing => "در حال پردازش",
            OrderStatus.Shipped => "ارسال شده",
            OrderStatus.Delivered => "تحویل داده شده",
            OrderStatus.Cancelled => "لغو شده",
            _ => "نامشخص"
        };
    }

    public string GetStatusBadgeClass(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "warning",
            OrderStatus.Processing => "info",
            OrderStatus.Shipped => "primary",
            OrderStatus.Delivered => "success",
            OrderStatus.Cancelled => "danger",
            _ => "secondary"
        };
    }
}
