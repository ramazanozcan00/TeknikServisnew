using System;

namespace TeknikServis.Application.Features.Customers.DTOs
{
    public record CustomerDto(
        Guid Id,
        string CustomerCode,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string? TaxNumber,
        string? TaxOffice,
        string? City,     // EKLENDİ
        string? District, // EKLENDİ
        string? Address,
        string? Notes,
        DateTime CreatedAt);
}