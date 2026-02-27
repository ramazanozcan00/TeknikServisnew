namespace TeknikServis.Application.Features.WorkOrders.DTOs
{
    // Ekranda teknisyene göstereceğimiz birleştirilmiş veriler
    public record WorkOrderBoardDto(
        Guid Id,
        Guid DeviceId,
        string WorkOrderNo,
        string CustomerFullName,
        string DeviceInfo, // Örn: Apple iPhone 13
        string Description,
        TeknikServis.Domain.Enums.WorkOrderStatus Status,
        DateTime CreatedAt);
}