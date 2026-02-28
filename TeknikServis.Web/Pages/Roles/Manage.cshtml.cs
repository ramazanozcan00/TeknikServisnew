using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Web.Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class ManageModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public ManageModel(RoleManager<ApplicationRole> roleManager) => _roleManager = roleManager;

        public string RoleName { get; set; } = string.Empty;

        [BindProperty]
        public List<PermissionViewModel> Permissions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return RedirectToPage("./Index");

            RoleName = role.Name!;
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var allPermissions = Domain.Constants.PermissionList.GetAll();

            foreach (var permission in allPermissions)
            {
                Permissions.Add(new PermissionViewModel
                {
                    PermissionName = permission,
                    // Eðer bu rolün veritabanýnda bu yetkisi varsa kutucuðu iþaretli (true) getir
                    IsSelected = existingClaims.Any(c => c.Type == "Permission" && c.Value == permission)
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            // 1. Rolün eski yetkilerinin hepsini temizle
            foreach (var claim in existingClaims.Where(c => c.Type == "Permission"))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // 2. Ekranda iþaretlenmiþ (true) olan yeni yetkileri veritabanýna ekle
            foreach (var perm in Permissions.Where(p => p.IsSelected))
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", perm.PermissionName));
            }

            TempData["SuccessMessage"] = $"{role.Name} rolünün yetkileri baþarýyla güncellendi!";
            return RedirectToPage("./Index");
        }

        public class PermissionViewModel
        {
            public string PermissionName { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
        }
    }
}