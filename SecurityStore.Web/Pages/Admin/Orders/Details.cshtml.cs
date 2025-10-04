using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Domain.Enums;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Admin.Orders;

// صفحه جزئیات سفارش برای مدیر
// Order details page for admin
[Authorize(Roles = "Admin,Manager")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DetailsModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Order Order { get; set; } = null!;
    public ApplicationUser? Customer { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // دریافت سفارش با تمام اطلاعات
        // Get order with all details
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        Order = order;

        // دریافت اطلاعات مشتری
        // Get customer information
        Customer = await _userManager.FindByIdAsync(order.UserId);

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        // بروزرسانی وضعیت سفارش
        // Update order status
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        // اگر وضعیت به تحویل شده تغییر کرد
        // If status changed to delivered
        if (status == OrderStatus.Delivered && !order.PaidAt.HasValue)
        {
            order.PaidAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "وضعیت سفارش با موفقیت بروزرسانی شد";
        return RedirectToPage(new { id });
    }
}
