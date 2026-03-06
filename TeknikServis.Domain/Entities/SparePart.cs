using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    public class SparePart : BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string SparePartCode { get; set; } // Eski 'Code' yerine bu kullanılıyor
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int StockQuantity { get; private set; }
        public int CriticalStockLevel { get; set; }
        public string Unit { get; set; }

        public ICollection<SparePartItem> Items { get; set; } = new List<SparePartItem>();

        private SparePart() { }

        public static SparePart Create(string name, string barcode, string sparePartCode, decimal purchasePrice, decimal salePrice, int criticalStockLevel, string unit = "Adet")
        {
            return new SparePart
            {
                Name = name,
                Barcode = barcode,
                SparePartCode = sparePartCode,
                PurchasePrice = purchasePrice,
                SalePrice = salePrice,
                StockQuantity = 0,
                CriticalStockLevel = criticalStockLevel,
                Unit = unit,
                CreatedAt = DateTime.UtcNow
            };
        }

        // HATA ÇÖZÜMÜ: Eksik olan Update metodu
        public void Update(string name, string barcode, string sparePartCode, decimal purchasePrice, decimal salePrice, int criticalStockLevel, string unit)
        {
            Name = name;
            Barcode = barcode;
            SparePartCode = sparePartCode;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            CriticalStockLevel = criticalStockLevel;
            Unit = unit;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddItem(string serialNumber, string invoiceNo)
        {
            var newItem = new SparePartItem
            {
                SerialNumber = serialNumber,
                PurchaseInvoiceNo = invoiceNo,
                SparePartId = this.Id,
                CreatedAt = DateTime.UtcNow
            };
            Items.Add(newItem);
            UpdateStockQuantity();
        }

        // HATA ÇÖZÜMÜ: Eksik olan DecreaseStock metodu (Seri nosuz kullanım için)
        public bool DecreaseStock(int amount)
        {
            if (StockQuantity >= amount)
            {
                // Seri nosuz ürünlerde en eski eklenen kullanılmamış parçaları işaretle
                var itemsToUse = Items.Where(x => !x.IsUsed).Take(amount);
                foreach (var item in itemsToUse) item.IsUsed = true;

                UpdateStockQuantity();
                return true;
            }
            return false;
        }

        public void UpdateStockQuantity()
        {
            StockQuantity = Items.Count(x => !x.IsUsed);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}