using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeknikServis.Infrastructure.Persistence.Identity;

namespace TeknikServis.Web.Pages.Employees
{
    // SADECE ADMÝNLER GÝREBÝLÝR!
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<EmployeeViewModel> Employees { get; set; } = new();

        // Index.cshtml.cs içindeki döngü ve ViewModel kýsmýný þu þekilde güncelle:

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Employees.Add(new EmployeeViewModel
                {
                    Id = user.Id.ToString(),
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Role = roles.Count > 0 ? roles[0] : "Rol Yok",
                    IsActive = user.IsActive // YENÝ EKLENDÝ
                });
            }
        }

        public class EmployeeViewModel
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; } // YENÝ EKLENDÝ
        }
    }
}