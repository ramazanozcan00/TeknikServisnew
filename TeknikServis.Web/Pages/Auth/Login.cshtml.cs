using Microsoft.AspNetCore.Authorization; // 1. BU SATIRI EKLE
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Web.Pages.Auth
{
    [AllowAnonymous] // 2. BU ETÝKETÝ EKLE (Sonsuz döngüyü çözen anahtar budur)
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public class LoginInputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

            // YENÝ EKLENEN SATIR: Tasarýmdaki onay kutusunu burasý yakalayacak
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // true yerine Input.RememberMe yazdýk
            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                false);

            if (result.Succeeded)
                return RedirectToPage("/Index"); // Veya "/WorkOrders/Index"

            ModelState.AddModelError("", "Email veya þifre hatalý");
            return Page();
        }
    }
}