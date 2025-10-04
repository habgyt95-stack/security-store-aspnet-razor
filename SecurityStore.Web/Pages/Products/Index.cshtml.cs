using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;

namespace SecurityStore.Web.Pages.Products;

// صفحه لیست محصولات
// Products listing page
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 9;

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
    public string SortBy { get; set; } = "newest";
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    
    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        // بارگذاری دسته‌بندی‌ها
        // Load categories
        Categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        // ساخت کوئری محصولات
        // Build products query
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        // فیلتر براساس جستجو
        // Filter by search term
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(SearchTerm) || 
                p.ShortDescription!.Contains(SearchTerm) ||
                p.Brand!.Contains(SearchTerm));
        }

        // فیلتر براساس دسته‌بندی
        // Filter by category
        if (CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == CategoryId.Value);
        }

        // مرتب‌سازی
        // Sort
        query = SortBy switch
        {
            "price-low" => query.OrderBy(p => p.Price),
            "price-high" => query.OrderByDescending(p => p.Price),
            "popular" => query.OrderByDescending(p => p.ViewCount),
            _ => query.OrderByDescending(p => p.CreatedAt) // newest
        };

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
}
