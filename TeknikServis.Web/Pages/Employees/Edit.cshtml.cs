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
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        // Rolleri ekrana taþýyacak liste
        public List<string> AvailableRoles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return RedirectToPage("./Index");

            var roles = await _userManager.GetRolesAsync(user);

            Input = new InputModel
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "",
                IsActive = user.IsActive
            };

            // Ekleme sayfasýnda (Create) kusursuz çalýþan Rol çekme kodunun aynýsý!
            AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;
                return Page();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Email = Input.Email;
            user.UserName = Input.Email;
            user.IsActive = Input.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors) ModelState.AddModelError(string.Empty, error.Description);
                AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, Input.Password);
                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors) ModelState.AddModelError(string.Empty, error.Description);
                    AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()!;
                    return Page();
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(Input.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!string.IsNullOrEmpty(Input.Role))
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                }
            }

            TempData["SuccessMessage"] = "Personel bilgileri baþarýyla güncellendi!";
            return RedirectToPage("./Index");
        }

        public class InputModel
        {
            public string Id { get; set; } = string.Empty;

            [Required(ErrorMessage = "Ad zorunludur.")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Soyad zorunludur.")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email zorunludur."), EmailAddress]
            public string Email { get; set; } = string.Empty;

            [MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalý.")]
            public string? Password { get; set; }

            [Required(ErrorMessage = "Rol seçimi zorunludur.")]
            public string Role { get; set; } = string.Empty;

            public bool IsActive { get; set; }
        }
    }
}