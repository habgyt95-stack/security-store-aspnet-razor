using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Domain.Enums;
using SecurityStore.Infrastructure.Data;

namespace SecurityStore.Web.Pages.Admin.Orders;

// صفحه مدیریت سفارشات
// Orders management page
[Authorize(Roles = "Admin,Manager")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 20;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Order> Orders { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int? Status { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    
    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        // ساخت کوئری سفارشات
        // Build orders query
        var query = _context.Orders.AsQueryable();

        // فیلتر براساس جستجو
        // Filter by search term
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(o => o.OrderNumber.Contains(SearchTerm));
        }

        // فیلتر براساس وضعیت
        // Filter by status
        if (Status.HasValue)
        {
            query = query.Where(o => (int)o.Status == Status.Value);
        }

        // مرتب‌سازی
        // Sort
        query = query.OrderByDescending(o => o.CreatedAt);

        // محاسبه تعداد کل صفحات
        // Calculate total pages
        var totalItems = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        // بارگذاری سفارشات با صفحه‌بندی
        // Load orders with pagination
        Orders = await query
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, int status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Status = (OrderStatus)status;
        await _context.SaveChangesAsync();

        TempData["Success"] = "وضعیت سفارش به‌روزرسانی شد";
        return RedirectToPage();
    }
}
