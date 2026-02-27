using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Infrastructure.Persistence.Configurations
{
    public class SparePartConfiguration : IEntityTypeConfiguration<SparePart>
    {
        public void Configure(EntityTypeBuilder<SparePart> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

            // Stok Kodu benzersiz (Unique) olmalı ki aynı barkoddan iki tane olmasın
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Code).IsUnique();

            // Fiyat sütunları (Kuruşlu hesaplamalar için decimal ayarı)
            builder.Property(x => x.PurchasePrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.SalePrice).HasColumnType("decimal(18,2)");

            builder.Property(x => x.Unit).HasMaxLength(20);
        }
    }
}