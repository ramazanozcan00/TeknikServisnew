using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    // Customer bir BaseEntity'dir ve Ana Varlıktır (IAggregateRoot)
    public class Customer : BaseEntity, IAggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string TaxNumber { get; private set; }

        // Entity Framework'ün (veritabanı aracımızın) hata vermemesi için boş bir yapıcı metod (constructor)
        private Customer()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            TaxNumber = string.Empty;
        }

        // Yeni bir müşteri oluştururken kullanılacak metod (Buna Encapsulation - Kapsülleme denir)
        public static Customer Create(string firstName, string lastName, string email, string phone, string taxNumber)
        {
            return new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone,
                TaxNumber = taxNumber,
                CreatedAt = DateTime.UtcNow // Oluşturulma zamanı şimdiki zaman olsun
            };
        }

        // Müşteri bilgilerini güncellemek için metod
        public void Update(string firstName, string lastName, string email, string phone)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            UpdatedAt = DateTime.UtcNow; // Güncellenme zamanını ayarla
        }
    }
}