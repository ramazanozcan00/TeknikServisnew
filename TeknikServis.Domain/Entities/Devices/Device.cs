using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities.Devices
{
    public class Device : BaseEntity, IAggregateRoot
    {
        // 1. İlişki (Foreign Key): Bu cihaz hangi müşterinin?
        public Guid CustomerId { get; private set; }

        // 2. Cihaz Özellikleri
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public string SerialNumber { get; private set; }
        public string DeviceType { get; private set; }
        public string? Note { get; private set; } // Opsiyonel (Çizik var vb.)

        private Device() { } // EF Core için boş yapıcı

        // Yeni cihaz oluşturma metodu
        public static Device Create(Guid customerId, string brand, string model, string serialNumber, string deviceType, string? note)
        {
            return new Device
            {
                CustomerId = customerId,
                Brand = brand,
                Model = model,
                SerialNumber = serialNumber,
                DeviceType = deviceType,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}