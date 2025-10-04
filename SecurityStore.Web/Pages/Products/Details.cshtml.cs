using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;
using System.Text.Json;

namespace SecurityStore.Web.Pages.Products;

// صفحه جزئیات محصول
// Product details page
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Product Product { get; set; } = null!;
    public List<Product> RelatedProducts { get; set; } = new();
    public List<ProductReview> ApprovedReviews { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool UserHasPurchased { get; set; }
    public bool UserHasReviewed { get; set; }
    public bool UserCanReview { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // بارگذاری محصول با تمام اطلاعات مرتبط
        // Load product with all related information
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductReviews)
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

        // بارگذاری نظرات تأیید شده
        // Load approved reviews
        ApprovedReviews = await _context.ProductReviews
            .Where(r => r.ProductId == id && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        // محاسبه میانگین امتیاز
        // Calculate average rating
        if (ApprovedReviews.Any())
        {
            AverageRating = ApprovedReviews.Average(r => r.Rating);
            TotalReviews = ApprovedReviews.Count;
        }

        // بررسی امکان ثبت نظر توسط کاربر
        // Check if user can submit review
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // بررسی خرید محصول
                // Check if user has purchased
                UserHasPurchased = await _context.OrderItems
                    .AnyAsync(oi => oi.ProductId == id && 
                                   oi.Order.UserId == user.Id &&
                                   oi.Order.Status == Domain.Enums.OrderStatus.Delivered);

                // بررسی ثبت نظر قبلی
                // Check if user has already reviewed
                UserHasReviewed = await _context.ProductReviews
                    .AnyAsync(r => r.ProductId == id && r.UserId == user.Id);

                UserCanReview = UserHasPurchased && !UserHasReviewed;
            }
        }

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

    public async Task<IActionResult> OnPostSubmitReviewAsync(int id, int rating, string? title, string comment)
    {
        if (!User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // بررسی خرید محصول
        // Check if user has purchased
        var hasPurchased = await _context.OrderItems
            .AnyAsync(oi => oi.ProductId == id && 
                           oi.Order.UserId == user.Id &&
                           oi.Order.Status == Domain.Enums.OrderStatus.Delivered);

        if (!hasPurchased)
        {
            TempData["Error"] = "برای ثبت نظر، ابتدا باید این محصول را خریداری کرده باشید";
            return RedirectToPage(new { id });
        }

        // بررسی ثبت نظر قبلی
        // Check if already reviewed
        var hasReviewed = await _context.ProductReviews
            .AnyAsync(r => r.ProductId == id && r.UserId == user.Id);

        if (hasReviewed)
        {
            TempData["Error"] = "شما قبلاً نظر خود را درباره این محصول ثبت کرده‌اید";
            return RedirectToPage(new { id });
        }

        // ایجاد نظر جدید
        // Create new review
        var review = new ProductReview
        {
            ProductId = id,
            UserId = user.Id,
            UserName = user.FullName ?? user.Email ?? "کاربر",
            Rating = Math.Clamp(rating, 1, 5),
            Title = title,
            Comment = comment,
            IsApproved = false, // نیاز به تأیید مدیر
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();

        TempData["Success"] = "نظر شما با موفقیت ثبت شد و پس از تأیید مدیر نمایش داده خواهد شد";
        return RedirectToPage(new { id });
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
