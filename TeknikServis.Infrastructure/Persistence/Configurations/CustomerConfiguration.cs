using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerCode).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.CustomerCode).IsUnique(); // Cari kod benzersiz olmalı

            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.TaxNumber).HasMaxLength(50);
            builder.Property(x => x.TaxOffice).HasMaxLength(100);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(1000);
        }
    }
}