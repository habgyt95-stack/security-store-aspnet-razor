using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecurityStore.Application.Interfaces;
using SecurityStore.Domain.Entities;
using SecurityStore.Domain.Enums;
using SecurityStore.Infrastructure.Data;
using SecurityStore.Infrastructure.Identity;
using SecurityStore.Web.Pages.Products;

namespace SecurityStore.Web.Pages.Cart;

// صفحه تسویه حساب
// Checkout page
[Authorize]
public class CheckoutModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentService _paymentService;

    public CheckoutModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IPaymentService paymentService)
    {
        _context = context;
        _userManager = userManager;
        _paymentService = paymentService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<CartItem> CartItems { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "آدرس ارسال الزامی است")]
        [Display(Name = "آدرس ارسال")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "شهر الزامی است")]
        [Display(Name = "شهر")]
        public string ShippingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "کد پستی الزامی است")]
        [Display(Name = "کد پستی")]
        public string ShippingPostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "تلفن تماس الزامی است")]
        [Display(Name = "تلفن تماس")]
        public string ShippingPhone { get; set; } = string.Empty;

        [Display(Name = "یادداشت")]
        public string? Notes { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        LoadCart();

        if (!CartItems.Any())
        {
            return RedirectToPage("/Cart/Index");
        }

        // پیش‌پر کردن اطلاعات کاربر
        // Pre-fill user information
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            Input.ShippingAddress = user.Address ?? "";
            Input.ShippingCity = user.City ?? "";
            Input.ShippingPostalCode = user.PostalCode ?? "";
            Input.ShippingPhone = user.PhoneNumber ?? "";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadCart();

        if (!CartItems.Any())
        {
            return RedirectToPage("/Cart/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // ایجاد سفارش
        // Create order
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = user.Id,
            Status = OrderStatus.Pending,
            SubTotal = SubTotal,
            ShippingCost = ShippingCost,
            Discount = 0,
            Tax = 0,
            TotalAmount = Total,
            ShippingAddress = Input.ShippingAddress,
            ShippingCity = Input.ShippingCity,
            ShippingPostalCode = Input.ShippingPostalCode,
            ShippingPhone = Input.ShippingPhone,
            Notes = Input.Notes,
            PaymentMethod = "Online"
        };

        // افزودن آیتم‌های سفارش
        // Add order items
        foreach (var cartItem in CartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                ProductName = cartItem.ProductName,
                UnitPrice = cartItem.Price,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.Price * cartItem.Quantity
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // پردازش پرداخت
        // Process payment
        var paymentRequest = new PaymentRequest
        {
            Amount = Total,
            OrderNumber = order.OrderNumber,
            CustomerEmail = user.Email ?? "",
            Description = $"پرداخت سفارش {order.OrderNumber}"
        };

        var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);

        if (paymentResult.Success)
        {
            // به‌روزرسانی سفارش
            // Update order
            order.Status = OrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            order.TransactionId = paymentResult.TransactionId;

            // کاهش موجودی محصولات
            // Reduce product stock
            foreach (var item in CartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    
                    // ثبت گزارش موجودی
                    // Log stock change
                    _context.StockLogs.Add(new StockLog
                    {
                        ProductId = product.Id,
                        Action = StockAction.Sold,
                        Quantity = -item.Quantity,
                        PreviousStock = product.StockQuantity + item.Quantity,
                        NewStock = product.StockQuantity,
                        Notes = $"فروش از طریق سفارش {order.OrderNumber}",
                        UserId = user.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            // پاک کردن سبد خرید
            // Clear cart
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "سفارش شما با موفقیت ثبت شد";
            return RedirectToPage("/Cart/OrderComplete", new { id = order.Id });
        }
        else
        {
            TempData["Error"] = paymentResult.Message ?? "خطا در پردازش پرداخت";
            return Page();
        }
    }

    private void LoadCart()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        if (!string.IsNullOrEmpty(cartJson))
        {
            CartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        SubTotal = CartItems.Sum(c => c.Price * c.Quantity);
        ShippingCost = SubTotal >= 5000000 ? 0 : 50000;
        Total = SubTotal + ShippingCost;
    }

    private string GenerateOrderNumber()
    {
        // تولید شماره سفارش منحصر به فرد
        // Generate unique order number
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
    }
}
