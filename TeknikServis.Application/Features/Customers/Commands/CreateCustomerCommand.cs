using MediatR;
using System;
using TeknikServis.Application.Common.Models;

namespace TeknikServis.Application.Features.Customers.Commands
{
    // Yeni alanları komuta ekliyoruz
    public record CreateCustomerCommand(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string? TaxNumber,
        string? TaxOffice,
        string? Address,
        string? Notes
    ) : IRequest<Result<Guid>>;
}