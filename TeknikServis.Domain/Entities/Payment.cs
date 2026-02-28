using TeknikServis.Domain.Common;
using TeknikServis.Domain.Enums;
using System;

namespace TeknikServis.Domain.Entities
{
    public class Payment : BaseEntity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public Guid? WorkOrderId { get; private set; }

        // YENİ EKLENEN: Tahsilat Makbuz Numarası
        public string ReceiptNo { get; private set; }

        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; }
        public string Description { get; private set; }

        private Payment() { }

        public static Payment Create(Guid customerId, Guid? workOrderId, decimal amount, PaymentMethod method, string description)
        {
            // Otomatik Makbuz Numarası Üretimi (Örn: MAKBUZ-20260228-A1B2)
            string generatedNo = $"MAKBUZ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            return new Payment
            {
                CustomerId = customerId,
                WorkOrderId = workOrderId,
                ReceiptNo = generatedNo, // Numarayı atıyoruz
                Amount = amount,
                Method = method,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}