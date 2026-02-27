namespace TeknikServis.Application.Features.Dashboard.DTOs
{
    // Ekranda göstereceğimiz 4 temel istatistik
    public record DashboardStatsDto(
        int TotalCustomers,
        int TotalDevices,
        int ActiveWorkOrders, // Bekleyen ve Onarımda olanlar
        int CompletedWorkOrders // Tamamlanan ve Teslim Edilenler
    );
}