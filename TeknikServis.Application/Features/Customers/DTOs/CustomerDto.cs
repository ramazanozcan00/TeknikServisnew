namespace TeknikServis.Application.Features.Customers.DTOs
{
    // Record kullanarak sadece veri taşıyacağımızı belirtiyoruz
    public record CustomerDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string TaxNumber);
}