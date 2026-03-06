using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    public class SparePartItem : BaseEntity
    {
        public string SerialNumber { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public bool IsUsed { get; set; } = false;
        public Guid SparePartId { get; set; }
        public SparePart SparePart { get; set; }
    }
}