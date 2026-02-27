using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // OnGet yerine OnPostAsync yapýyoruz çünkü layout'tan POST isteði geliyor
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Auth/Login");
        }
    }
}