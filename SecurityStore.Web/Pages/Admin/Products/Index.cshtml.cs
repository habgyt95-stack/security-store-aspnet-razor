using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;

namespace SecurityStore.Web.Pages.Admin.Products;

// صفحه مدیریت محصولات
// Products management page
[Authorize(Roles = "Admin,Manager")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 20;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    
    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        // بارگذاری دسته‌بندی‌ها
        // Load categories
        Categories = await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        // ساخت کوئری محصولات
        // Build products query
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        // فیلتر براساس جستجو
        // Filter by search term
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(SearchTerm) || 
                p.Sku.Contains(SearchTerm) ||
                (p.Brand != null && p.Brand.Contains(SearchTerm)));
        }

        // فیلتر براساس دسته‌بندی
        // Filter by category
        if (CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == CategoryId.Value);
        }

        // فیلتر براساس وضعیت
        // Filter by status
        if (IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == IsActive.Value);
        }

        // مرتب‌سازی
        // Sort
        query = query.OrderByDescending(p => p.CreatedAt);

        // محاسبه تعداد کل صفحات
        // Calculate total pages
        var totalItems = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        // بارگذاری محصولات با صفحه‌بندی
        // Load products with pagination
        Products = await query
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // حذف نرم (غیرفعال کردن)
        // Soft delete (deactivate)
        product.IsActive = false;
        await _context.SaveChangesAsync();

        TempData["Success"] = "محصول با موفقیت حذف شد";
        return RedirectToPage();
    }
}
