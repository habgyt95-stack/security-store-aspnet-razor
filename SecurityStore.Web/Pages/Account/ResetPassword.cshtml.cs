using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Account;

// صفحه بازیابی رمز عبور
// Reset password page
public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordModel> _logger;

    public ResetPasswordModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool PasswordReset { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? userId, string? token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            ErrorMessage = "لینک بازیابی رمز عبور نامعتبر است";
            return Page();
        }

        Input.UserId = userId;
        Input.Token = token;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByIdAsync(Input.UserId);
        if (user == null)
        {
            ErrorMessage = "کاربر یافت نشد";
            _logger.LogWarning("تلاش برای بازیابی رمز عبور با شناسه کاربر نامعتبر: {UserId}", Input.UserId);
            return Page();
        }

        // بازیابی رمز عبور
        // Reset password
        var result = await _userManager.ResetPasswordAsync(user, Input.Token, Input.Password);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("رمز عبور کاربر {Email} با موفقیت تغییر کرد", user.Email);
            PasswordReset = true;
            return Page();
        }

        // اگر خطا داشت
        // If error occurred
        foreach (var error in result.Errors)
        {
            if (error.Code == "InvalidToken")
            {
                ErrorMessage = "لینک بازیابی رمز عبور منقضی شده یا نامعتبر است. لطفاً دوباره درخواست دهید.";
            }
            else
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        _logger.LogWarning("خطا در بازیابی رمز عبور برای کاربر {Email}", user.Email);
        return Page();
    }
}
