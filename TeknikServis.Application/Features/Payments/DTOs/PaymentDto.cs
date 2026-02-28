using System;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Application.Features.Payments.DTOs
{
    public record PaymentDto(
        Guid Id,
        Guid CustomerId,
        Guid? WorkOrderId,
        string ReceiptNo, // YENİ EKLENDİ
        decimal Amount,
        PaymentMethod Method,
        string Description,
        DateTime CreatedAt,
        string CustomerFullName = "",
        string WorkOrderNo = "");
}