using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecurityStore.Web.Pages.Products;
using System.Text.Json;

namespace SecurityStore.Web.Pages.Cart;

// صفحه سبد خرید
// Shopping cart page
public class IndexModel : PageModel
{
    public List<CartItem> CartItems { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }

    public void OnGet()
    {
        LoadCart();
    }

    public IActionResult OnPostUpdateQuantity(int productId, string action)
    {
        // دریافت سبد خرید
        // Get cart
        var cartJson = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cartJson))
        {
            return RedirectToPage();
        }

        var cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);

        if (item != null)
        {
            if (action == "increase")
            {
                item.Quantity++;
            }
            else if (action == "decrease" && item.Quantity > 1)
            {
                item.Quantity--;
            }

            // ذخیره سبد خرید
            // Save cart
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int productId)
    {
        // دریافت سبد خرید
        // Get cart
        var cartJson = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cartJson))
        {
            return RedirectToPage();
        }

        var cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        cart.RemoveAll(c => c.ProductId == productId);

        // ذخیره سبد خرید
        // Save cart
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

        TempData["Success"] = "محصول از سبد خرید حذف شد";
        return RedirectToPage();
    }

    public IActionResult OnPostClear()
    {
        // پاک کردن سبد خرید
        // Clear cart
        HttpContext.Session.Remove("Cart");
        TempData["Success"] = "سبد خرید پاک شد";
        return RedirectToPage();
    }

    private void LoadCart()
    {
        // بارگذاری سبد خرید از Session
        // Load cart from session
        var cartJson = HttpContext.Session.GetString("Cart");
        if (!string.IsNullOrEmpty(cartJson))
        {
            CartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        // محاسبه مجموع
        // Calculate totals
        SubTotal = CartItems.Sum(c => c.Price * c.Quantity);
        
        // هزینه ارسال (رایگان برای خریدهای بالای 5 میلیون)
        // Shipping cost (free for purchases over 5 million)
        ShippingCost = SubTotal >= 5000000 ? 0 : 50000;
        
        Total = SubTotal + ShippingCost - Discount;
    }
}
