using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    public class Customer : BaseEntity, IAggregateRoot
    {
        public string CustomerCode { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? TaxNumber { get; private set; }
        public string? TaxOffice { get; private set; }
        public string? Address { get; private set; }
        public string? Notes { get; private set; }

        private Customer() { } // EF Core için boş yapıcı

        public static Customer Create(string firstName, string lastName, string email, string phoneNumber, string? taxNumber = null, string? taxOffice = null, string? address = null, string? notes = null)
        {
            // Otomatik Cari Kodu Üretimi
            string generatedCode = $"CARI-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            return new Customer
            {
                CustomerCode = generatedCode,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                TaxNumber = taxNumber,
                TaxOffice = taxOffice,
                Address = address,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string firstName, string lastName, string email, string phoneNumber, string? taxNumber, string? taxOffice, string? address, string? notes)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            TaxNumber = taxNumber;
            TaxOffice = taxOffice;
            Address = address;
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}