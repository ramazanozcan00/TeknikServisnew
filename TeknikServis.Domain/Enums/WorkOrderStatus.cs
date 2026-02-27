namespace TeknikServis.Domain.Enums
{
    public enum WorkOrderStatus
    {
        Bekliyor = 1,       // Cihaz teslim alındı, sırasını bekliyor
        Onarimda = 2,       // Teknisyen cihaz üzerinde çalışıyor
        Tamamlandi = 3,     // Onarım bitti, müşteri bekleniyor
        TeslimEdildi = 4,   // Cihaz müşteriye verildi, süreç bitti
        IptalEdildi = 5     // Onarımdan vazgeçildi
    }
}