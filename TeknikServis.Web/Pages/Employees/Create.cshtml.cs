using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Web.Pages.Employees
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<string> AvailableRoles { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Veritabanýndaki tüm rolleri çekip dropdown'a gönderiyoruz
            AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email, // Sisteme Email ile giriþ yapacaklar
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName
            };

            // Kullanýcýyý belirlediðimiz þifre ile oluþtur
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                // Oluþan kullanýcýya seçilen rolü ata
                await _userManager.AddToRoleAsync(user, Input.Role);
                TempData["SuccessMessage"] = $"{Input.FirstName} {Input.LastName} sisteme baþarýyla eklendi!";
                return RedirectToPage("./Index");
            }

            // Eðer þifre zayýfsa vs. hatalarý ekrana bas
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;
            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Ad zorunludur.")] public string FirstName { get; set; } = string.Empty;
            [Required(ErrorMessage = "Soyad zorunludur.")] public string LastName { get; set; } = string.Empty;
            [Required(ErrorMessage = "Email zorunludur."), EmailAddress] public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Þifre zorunludur."), MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalý.")] public string Password { get; set; } = string.Empty;
            [Required(ErrorMessage = "Rol seçimi zorunludur.")] public string Role { get; set; } = string.Empty;
        }
    }
}