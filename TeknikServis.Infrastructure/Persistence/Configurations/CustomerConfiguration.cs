using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Tablo adı (Opsiyonel, yazmazsan varsayılan olarak 'Customers' olur)
            builder.ToTable("Customers");

            // Primary Key (Birincil Anahtar)
            builder.HasKey(x => x.Id);

            // Sütun özellikleri
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            // Email adresinin veritabanında tekrar etmemesi için Index atıyoruz
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}