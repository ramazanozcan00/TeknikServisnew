using TeknikServis.Domain.Common;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities
{
    // Tahsilat ve Kasa Hareketleri
    public class Payment : BaseEntity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; } // Kimden tahsil edildi?
        public Guid? WorkOrderId { get; private set; } // Hangi iş emri için? (Boşsa genel borç ödemesidir)
        public decimal Amount { get; private set; } // Tutar
        public PaymentMethod Method { get; private set; } // Nakit, Kredi Kartı vb.
        public string Description { get; private set; } // Açıklama

        // İşlem Tarihi zaten BaseEntity'deki CreatedAt özelliğinden gelecek.

        private Payment() { }

        public static Payment Create(Guid customerId, Guid? workOrderId, decimal amount, PaymentMethod method, string description)
        {
            return new Payment
            {
                CustomerId = customerId,
                WorkOrderId = workOrderId,
                Amount = amount,
                Method = method,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}