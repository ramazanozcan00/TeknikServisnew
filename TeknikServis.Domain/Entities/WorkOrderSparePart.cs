using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    // İş Emrinde Kullanılan Yedek Parçalar
    public class WorkOrderSparePart : BaseEntity
    {
        public Guid WorkOrderId { get; private set; }
        public Guid SparePartId { get; private set; }
        public string PartName { get; private set; } // O günkü parça adı (Geçmişe dönük raporlar için)
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; } // O günkü birim satış fiyatı

        private WorkOrderSparePart() { }

        public static WorkOrderSparePart Create(Guid workOrderId, Guid sparePartId, string partName, int quantity, decimal unitPrice)
        {
            return new WorkOrderSparePart
            {
                WorkOrderId = workOrderId,
                SparePartId = sparePartId,
                PartName = partName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}