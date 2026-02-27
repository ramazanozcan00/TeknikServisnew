using System;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Application.Features.Payments.DTOs
{
    public record PaymentDto(
        Guid Id,
        Guid CustomerId,
        Guid? WorkOrderId,
        decimal Amount,
        PaymentMethod Method,
        string Description,
        DateTime CreatedAt,
        // Ekranda id yerine isimleri göstermek için eklediğimiz alanlar
        string CustomerFullName = "",
        string WorkOrderNo = "");
}