namespace TeknikServis.Application.Features.WorkOrders.DTOs
{
    public record WorkOrderDetailDto(
        Guid Id,
        string WorkOrderNo,
        Guid CustomerId, // BU SATIRIN OLDUĞUNDAN EMİN OLALIM
        string CustomerFullName,
        string CustomerPhone,
        Guid DeviceId,
        string DeviceBrand,
        string DeviceModel,
        string DeviceSerial,
        string DeviceInfo,
        string Description,
        string? TechnicianNotes,
        decimal? TotalPrice,
        TeknikServis.Domain.Enums.WorkOrderStatus Status,
        DateTime CreatedAt);
}