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

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Employees.Add(new EmployeeViewModel
                {
                    Id = user.Id.ToString(), // <--- ÝÞTE HATAYI ÇÖZEN KISIM (.ToString() eklendi)
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Role = roles.Count > 0 ? roles[0] : "Rol Yok"
                });
            }
        }

        public class EmployeeViewModel
        {
            public string Id { get; set; } // Ekranda göstermek için string olarak tutuyoruz
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
        }
    }
}