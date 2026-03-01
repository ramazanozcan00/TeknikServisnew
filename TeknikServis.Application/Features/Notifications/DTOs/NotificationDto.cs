using System;
namespace TeknikServis.Application.Features.Notifications.DTOs
{
    public record NotificationDto(Guid Id, string Title, string Message, string Color, DateTime CreatedAt);
}