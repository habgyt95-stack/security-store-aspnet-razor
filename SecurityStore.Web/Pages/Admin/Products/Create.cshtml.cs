using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecurityStore.Domain.Entities;
using SecurityStore.Infrastructure.Data;

namespace SecurityStore.Web.Pages.Admin.Products;

// صفحه افزودن محصول جدید
// Create product page
[Authorize(Roles = "Admin,Manager")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CategorySelectList { get; set; } = null!;

    public class InputModel
    {
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
        [Display(Name = "قیمت (تومان)")]
        public decimal Price { get; set; }

        [Display(Name = "قیمت تخفیف‌خورده (تومان)")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "موجودی الزامی است")]
        [Range(0, int.MaxValue, ErrorMessage = "موجودی باید مثبت باشد")]
        [Display(Name = "موجودی")]
        public int StockQuantity { get; set; }

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

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return Page();
        }

        // بررسی یکتا بودن Slug
        // Check slug uniqueness
        if (await _context.Products.AnyAsync(p => p.Slug == Input.Slug))
        {
            ModelState.AddModelError("Input.Slug", "این نامک قبلاً استفاده شده است");
            await LoadCategoriesAsync();
            return Page();
        }

        // بررسی یکتا بودن SKU
        // Check SKU uniqueness
        if (await _context.Products.AnyAsync(p => p.Sku == Input.Sku))
        {
            ModelState.AddModelError("Input.Sku", "این SKU قبلاً استفاده شده است");
            await LoadCategoriesAsync();
            return Page();
        }

        // ایجاد محصول جدید
        // Create new product
        var product = new Product
        {
            Name = Input.Name,
            ShortDescription = Input.ShortDescription,
            LongDescription = Input.LongDescription,
            Slug = Input.Slug,
            Sku = Input.Sku,
            Price = Input.Price,
            DiscountPrice = Input.DiscountPrice,
            StockQuantity = Input.StockQuantity,
            Brand = Input.Brand,
            IsActive = Input.IsActive,
            IsFeatured = Input.IsFeatured,
            CategoryId = Input.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "محصول با موفقیت ایجاد شد";
        return RedirectToPage("/Admin/Products/Index");
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        
        CategorySelectList = new SelectList(categories, "Id", "Name");
    }
}
