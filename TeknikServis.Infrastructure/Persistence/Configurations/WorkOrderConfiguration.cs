using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeknikServis.Domain.Entities.WorkOrders;

namespace TeknikServis.Infrastructure.Persistence.Configurations
{
    public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.TechnicianNotes).HasMaxLength(2000);

            // Decimal (Para) alanları için virgülden sonra kaç hane olacağını belirtmek zorunludur
            // 18 toplam rakam, 2 tanesi virgülden sonra (Örn: 1500.50)
            builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
            // Numarayı zorunlu yap ve benzersiz (Unique) olmasını sağla
            builder.Property(x => x.WorkOrderNo).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.WorkOrderNo).IsUnique();
        }
    }
}