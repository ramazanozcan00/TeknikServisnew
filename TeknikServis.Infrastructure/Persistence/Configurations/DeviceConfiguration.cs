using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeknikServis.Domain.Entities.Devices;

namespace TeknikServis.Infrastructure.Persistence.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Brand).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Model).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SerialNumber).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DeviceType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Note).HasMaxLength(500);

            // Seri numarası sistemde benzersiz olmalı (Aynı cihaz iki kere eklenemez)
            builder.HasIndex(x => x.SerialNumber).IsUnique();
        }
    }
}