using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    public class SparePart : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; } // Parça Adı (Örn: iPhone 13 Batarya)
        public string Code { get; private set; } // Stok Kodu veya Barkod (Örn: IPH13-BAT)
        public decimal PurchasePrice { get; private set; } // Alış Fiyatı (Maliyeti görmek için)
        public decimal SalePrice { get; private set; } // Satış Fiyatı (Müşteriye yansıyacak)
        public int StockQuantity { get; private set; } // Mevcut Stok Miktarı
        public int CriticalStockLevel { get; private set; } // Uyarı Seviyesi (Örn: 5'in altına düşerse uyar)
        public string Unit { get; private set; } // Birim (Adet, Metre, Gram vb.)

        private SparePart() { }

        public static SparePart Create(string name, string code, decimal purchasePrice, decimal salePrice, int initialStock, int criticalStockLevel, string unit = "Adet")
        {
            return new SparePart
            {
                Name = name,
                Code = code,
                PurchasePrice = purchasePrice,
                SalePrice = salePrice,
                StockQuantity = initialStock,
                CriticalStockLevel = criticalStockLevel,
                Unit = unit,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string name, string code, decimal purchasePrice, decimal salePrice, int criticalStockLevel, string unit)
        {
            Name = name;
            Code = code;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            CriticalStockLevel = criticalStockLevel;
            Unit = unit;
            UpdatedAt = DateTime.UtcNow;
        }

        // Stok Ekleme Metodu (Depoya yeni mal geldiğinde)
        public void AddStock(int amount)
        {
            if (amount > 0)
            {
                StockQuantity += amount;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        // Stok Düşme Metodu (Cihaz tamirinde kullanıldığında)
        public bool DecreaseStock(int amount)
        {
            if (amount > 0 && StockQuantity >= amount)
            {
                StockQuantity -= amount;
                UpdatedAt = DateTime.UtcNow;
                return true; // Başarıyla düşüldü
            }
            return false; // Yetersiz stok
        }
    }
}