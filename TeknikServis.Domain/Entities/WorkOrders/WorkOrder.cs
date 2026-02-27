using TeknikServis.Domain.Common;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities.WorkOrders
{
    public class WorkOrder : BaseEntity, IAggregateRoot
    {
        public Guid DeviceId { get; private set; }

        // YENİ EKLENEN: İş Emri / Fiş Numarası
        public string WorkOrderNo { get; private set; }

        public string Description { get; private set; }
        public string? TechnicianNotes { get; private set; }
        public decimal? TotalPrice { get; private set; }
        public WorkOrderStatus Status { get; private set; }

        private WorkOrder() { }

        public static WorkOrder Create(Guid deviceId, string description)
        {
            // Benzersiz numara üretimi (Örn: TS-20260227-1A4B)
            string generatedNo = $"TS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            return new WorkOrder
            {
                DeviceId = deviceId,
                WorkOrderNo = generatedNo, // Atamayı yapıyoruz
                Description = description,
                Status = WorkOrderStatus.Bekliyor,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateProgress(WorkOrderStatus status, string? technicianNotes, decimal? totalPrice)
        {
            Status = status;
            TechnicianNotes = technicianNotes;
            TotalPrice = totalPrice;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}