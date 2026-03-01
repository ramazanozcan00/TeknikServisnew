using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Web.Pages.Auth
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        // RoleManager'ý kaldýrdýk çünkü artýk giriþ ekranýnda zorla rol atamýyoruz
        public LoginModel(SignInManager<ApplicationUser> signInManager,
                          UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new();

        public class LoginInputModel
        {
            [Required(ErrorMessage = "Email zorunludur.")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Þifre zorunludur.")]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // GÝRÝÞ YAPAN PERSONEL AKTÝF MÝ KONTROLÜ
            if (user != null && !user.IsActive)
            {
                // E-posta yerine direkt "Admin" yetkisine sahip mi diye bakýyoruz.
                // Bu sayede e-postasý ne olursa olsun, Adminler kendini kilitlese bile girebilecek.
                var isUserAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                if (!isUserAdmin)
                {
                    ModelState.AddModelError("", "Hesabýnýz pasife alýnmýþtýr. Sisteme giriþ yapamazsýnýz.");
                    return Page();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                false);

            if (result.Succeeded)
                return RedirectToPage("/Index");

            ModelState.AddModelError("", "Email veya þifre hatalý");
            return Page();
        }
    }
}