using System.Collections.Generic;

namespace TeknikServis.Domain.Constants
{
    public static class PermissionList
    {
        // Sistemdeki tüm dinamik yetkilerin listesi
        public static List<string> GetAll()
        {
            return new List<string>
            {
                "Musteri.Goruntule", "Musteri.Ekle", "Musteri.Duzenle", "Musteri.Sil",
                "IsEmri.Goruntule", "IsEmri.Ekle", "IsEmri.Duzenle", "IsEmri.Sil",
                "Kasa.Goruntule", "Kasa.Tahsilat",
                "Calisan.Yonet"
            };
        }
    }
}