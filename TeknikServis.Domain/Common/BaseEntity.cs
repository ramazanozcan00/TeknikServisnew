namespace TeknikServis.Domain.Common
{
    // abstract yapıyoruz çünkü bu sınıf tek başına kullanılamaz, diğer sınıflar bundan miras alacak.
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid(); // Otomatik benzersiz ID üretir
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; } // ? işareti boş (null) olabileceğini gösterir
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } // Soft Delete (Veriyi silme, sadece gizle)
        public byte[] RowVersion { get; set; } = Array.Empty<byte>(); // Aynı anda iki kişi güncellerse çakışmayı önler
    }
}