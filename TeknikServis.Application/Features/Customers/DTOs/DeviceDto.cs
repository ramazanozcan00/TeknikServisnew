namespace TeknikServis.Application.Features.Devices.DTOs
{
    public record DeviceDto(Guid Id, string Brand, string Model, string SerialNumber, string DeviceType, string? Note, DateTime CreatedAt);
}