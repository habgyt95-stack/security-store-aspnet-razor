using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;

namespace SecurityStore.Web.Pages;

// صفحه اصلی
// Home page
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Product> FeaturedProducts { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        // بارگذاری محصولات ویژه
        // Load featured products
        FeaturedProducts = await _context.Products
            .Where(p => p.IsFeatured && p.IsActive)
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .Take(6)
            .ToListAsync();

        // بارگذاری دسته‌بندی‌ها
        // Load categories
        Categories = await _context.Categories
            .Where(c => c.IsActive && c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }
}
