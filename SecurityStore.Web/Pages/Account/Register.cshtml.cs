using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecurityStore.Infrastructure.Identity;

namespace SecurityStore.Web.Pages.Account;

// صفحه ثبت‌نام
// Register page
public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
        [Display(Name = "نام و نام خانوادگی")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            // ایجاد کاربر جدید
            // Create new user
            var user = new ApplicationUser 
            { 
                UserName = Input.Email, 
                Email = Input.Email,
                FullName = Input.FullName,
                EmailConfirmed = true // در محیط واقعی باید تأیید شود
            };
            
            var result = await _userManager.CreateAsync(user, Input.Password);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("کاربر جدید ایجاد شد - New user created");

                // اضافه کردن به نقش مشتری
                // Add to Customer role
                await _userManager.AddToRoleAsync(user, "Customer");

                // ورود خودکار
                // Auto sign in
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}
