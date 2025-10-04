using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Account;

// صفحه ورود
// Login page
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        // پاک کردن کوکی‌های احراز هویت قبلی
        // Clear existing authentication cookies
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            // ورود کاربر
            // Sign in user
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("کاربر وارد شد - User logged in");
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("حساب کاربر قفل شده است - User account locked out");
                ModelState.AddModelError(string.Empty, "حساب کاربری شما به دلیل تلاش‌های ناموفق متعدد قفل شده است.");
                return Page();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "ایمیل یا رمز عبور نامعتبر است.");
                return Page();
            }
        }

        return Page();
    }
}
