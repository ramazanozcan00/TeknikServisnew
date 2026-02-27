using Microsoft.AspNetCore.Identity;

namespace TeknikServis.Infrastructure.Persistence.Identity
{
    // Guid kullanarak ID'lerin benzersiz metinler olmasını sağlıyoruz
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}