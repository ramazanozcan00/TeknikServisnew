using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            // Tutar alanı kuruşlu hesaplar için decimal(18,2) olmalı
            builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Method).IsRequired();
        }
    }
}