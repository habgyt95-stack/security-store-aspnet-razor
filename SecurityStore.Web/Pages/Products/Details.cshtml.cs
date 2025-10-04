using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;
using System.Text.Json;

namespace SecurityStore.Web.Pages.Products;

// صفحه جزئیات محصول
// Product details page
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Product Product { get; set; } = null!;
    public List<Product> RelatedProducts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // بارگذاری محصول با تمام اطلاعات مرتبط
        // Load product with all related information
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        Product = product;

        // افزایش تعداد بازدید
        // Increment view count
        product.ViewCount++;
        await _context.SaveChangesAsync();

        // بارگذاری محصولات مرتبط (همان دسته‌بندی)
        // Load related products (same category)
        RelatedProducts = await _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
            .OrderByDescending(p => p.ViewCount)
            .Take(4)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, int quantity = 1)
    {
        // بارگذاری محصول
        // Load product
        var product = await _context.Products.FindAsync(id);
        if (product == null || !product.IsActive)
        {
            return NotFound();
        }

        // بررسی موجودی
        // Check stock
        if (product.StockQuantity < quantity)
        {
            TempData["Error"] = "موجودی کافی نیست";
            return RedirectToPage(new { id });
        }

        // دریافت سبد خرید از Session
        // Get cart from session
        var cartJson = HttpContext.Session.GetString("Cart");
        var cart = string.IsNullOrEmpty(cartJson) 
            ? new List<CartItem>() 
            : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();

        // بررسی آیا محصول قبلاً در سبد خرید هست
        // Check if product is already in cart
        var existingItem = cart.FirstOrDefault(c => c.ProductId == id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = id,
                ProductName = product.Name,
                Price = product.DiscountPrice ?? product.Price,
                Quantity = quantity,
                ImageUrl = product.MainImage
            });
        }

        // ذخیره سبد خرید در Session
        // Save cart to session
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

        TempData["Success"] = "محصول به سبد خرید اضافه شد";
        return RedirectToPage("/Cart/Index");
    }
}

// کلاس کمکی برای آیتم سبد خرید
// Helper class for cart item
public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}
