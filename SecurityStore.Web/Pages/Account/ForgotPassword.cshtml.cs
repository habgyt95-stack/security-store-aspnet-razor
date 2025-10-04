using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Account;

// صفحه فراموشی رمز عبور
// Forgot password page
public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ForgotPasswordModel> _logger;

    public ForgotPasswordModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ForgotPasswordModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool EmailSent { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        
        // از لحاظ امنیتی، همیشه پیام موفقیت نمایش می‌دهیم
        // حتی اگر کاربری با این ایمیل وجود نداشته باشد
        // For security, always show success message
        // even if user doesn't exist
        
        if (user != null)
        {
            // تولید توکن بازیابی رمز عبور
            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // ساخت لینک بازیابی
            // Build reset link
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { userId = user.Id, token = token },
                protocol: Request.Scheme);

            // در حالت واقعی، اینجا باید ایمیل ارسال شود
            // In production, email should be sent here
            _logger.LogInformation(
                "لینک بازیابی رمز عبور برای کاربر {Email}: {CallbackUrl}", 
                Input.Email, 
                callbackUrl);

            // نمایش لینک در لاگ برای محیط توسعه
            // Show link in log for development
            _logger.LogWarning("DEVELOPMENT MODE: Reset link: {CallbackUrl}", callbackUrl);
        }
        else
        {
            _logger.LogWarning("درخواست بازیابی رمز عبور برای ایمیل ناشناس: {Email}", Input.Email);
        }

        EmailSent = true;
        return Page();
    }
}
