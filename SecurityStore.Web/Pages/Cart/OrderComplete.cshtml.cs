using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Domain.Enums;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Cart;

// صفحه تکمیل سفارش
// Order complete page
[Authorize]
public class OrderCompleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderCompleteModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Order Order { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // دریافت سفارش با آیتم‌های آن
        // Get order with its items
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order == null)
        {
            return NotFound();
        }

        Order = order;

        // پاک کردن سبد خرید
        // Clear shopping cart
        HttpContext.Session.Remove("Cart");

        return Page();
    }

    public string GetStatusText()
    {
        return Order.Status switch
        {
            OrderStatus.Pending => "در انتظار پردازش",
            OrderStatus.Processing => "در حال پردازش",
            OrderStatus.Shipped => "ارسال شده",
            OrderStatus.Delivered => "تحویل داده شده",
            OrderStatus.Cancelled => "لغو شده",
            _ => "نامشخص"
        };
    }
}
