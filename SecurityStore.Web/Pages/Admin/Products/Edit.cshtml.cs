using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;

namespace SecurityStore.Web.Pages.Admin.Products;

// صفحه ویرایش محصول
// Edit product page
[Authorize(Roles = "Admin,Manager")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Product Product { get; set; } = null!;
    public SelectList CategorySelectList { get; set; } = null!;

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام محصول الزامی است")]
        [Display(Name = "نام محصول")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "توضیحات کوتاه")]
        public string? ShortDescription { get; set; }

        [Display(Name = "توضیحات کامل")]
        public string? LongDescription { get; set; }

        [Required(ErrorMessage = "نامک الزامی است")]
        [Display(Name = "نامک (Slug)")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU الزامی است")]
        [Display(Name = "SKU")]
        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "قیمت الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "قیمت باید مثبت باشد")]
        [Display(Name = "قیمت")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "قیمت تخفیف باید مثبت باشد")]
        [Display(Name = "قیمت تخفیف‌خورده")]
        public decimal? DiscountPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "موجودی باید مثبت باشد")]
        [Display(Name = "موجودی")]
        public int StockQuantity { get; set; }

        [Display(Name = "تصویر اصلی")]
        public string? MainImage { get; set; }

        [Display(Name = "برند")]
        public string? Brand { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ویژه")]
        public bool IsFeatured { get; set; }

        [Required(ErrorMessage = "دسته‌بندی الزامی است")]
        [Display(Name = "دسته‌بندی")]
        public int CategoryId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // دریافت محصول
        // Get product
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        Product = product;

        // پر کردن فرم با اطلاعات محصول
        // Fill form with product data
        Input = new InputModel
        {
            Id = product.Id,
            Name = product.Name,
            ShortDescription = product.ShortDescription,
            LongDescription = product.LongDescription,
            Slug = product.Slug,
            Sku = product.Sku,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            StockQuantity = product.StockQuantity,
            MainImage = product.MainImage,
            Brand = product.Brand,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId
        };

        await LoadCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // بارگذاری مجدد دسته‌بندی‌ها در صورت خطا
            // Reload categories on error
            var product = await _context.Products.FindAsync(Input.Id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
            await LoadCategoriesAsync();
            return Page();
        }

        // پیدا کردن محصول
        // Find product
        var existingProduct = await _context.Products.FindAsync(Input.Id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        // بررسی تکراری نبودن Slug (به جز محصول فعلی)
        // Check slug uniqueness (except current product)
        var slugExists = await _context.Products
            .AnyAsync(p => p.Slug == Input.Slug && p.Id != Input.Id);
        if (slugExists)
        {
            ModelState.AddModelError("Input.Slug", "این نامک قبلاً استفاده شده است");
            Product = existingProduct;
            await LoadCategoriesAsync();
            return Page();
        }

        // بررسی تکراری نبودن SKU (به جز محصول فعلی)
        // Check SKU uniqueness (except current product)
        var skuExists = await _context.Products
            .AnyAsync(p => p.Sku == Input.Sku && p.Id != Input.Id);
        if (skuExists)
        {
            ModelState.AddModelError("Input.Sku", "این SKU قبلاً استفاده شده است");
            Product = existingProduct;
            await LoadCategoriesAsync();
            return Page();
        }

        // ثبت موجودی قبلی برای لاگ
        // Record previous stock for logging
        var previousStock = existingProduct.StockQuantity;

        // بروزرسانی محصول
        // Update product
        existingProduct.Name = Input.Name;
        existingProduct.ShortDescription = Input.ShortDescription;
        existingProduct.LongDescription = Input.LongDescription;
        existingProduct.Slug = Input.Slug;
        existingProduct.Sku = Input.Sku;
        existingProduct.Price = Input.Price;
        existingProduct.DiscountPrice = Input.DiscountPrice;
        existingProduct.StockQuantity = Input.StockQuantity;
        existingProduct.MainImage = Input.MainImage;
        existingProduct.Brand = Input.Brand;
        existingProduct.IsActive = Input.IsActive;
        existingProduct.IsFeatured = Input.IsFeatured;
        existingProduct.CategoryId = Input.CategoryId;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        // ثبت لاگ تغییر موجودی در صورت تغییر
        // Log stock change if changed
        if (previousStock != Input.StockQuantity)
        {
            var stockLog = new StockLog
            {
                ProductId = existingProduct.Id,
                Action = SecurityStore.Domain.Enums.StockAction.Adjusted,
                Quantity = Input.StockQuantity - previousStock,
                PreviousStock = previousStock,
                NewStock = Input.StockQuantity,
                Notes = "ویرایش دستی توسط مدیر",
                UserId = User.Identity?.Name,
                CreatedAt = DateTime.UtcNow
            };
            _context.StockLogs.Add(stockLog);
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "محصول با موفقیت بروزرسانی شد";
        return RedirectToPage("/Admin/Products/Index");
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        CategorySelectList = new SelectList(categories, "Id", "Name");
    }
}
