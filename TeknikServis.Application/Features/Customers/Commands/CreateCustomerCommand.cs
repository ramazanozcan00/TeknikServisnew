using MediatR;
using System;
using TeknikServis.Application.Common.Models;

namespace TeknikServis.Application.Features.Customers.Commands
{
    public record CreateCustomerCommand(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string? TaxNumber,
        string? TaxOffice,
        string? City,     // YENİ EKLENDİ
        string? District, // YENİ EKLENDİ
        string? Address,
        string? Notes
    ) : IRequest<Result<Guid>>;
}