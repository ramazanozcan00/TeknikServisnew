using MediatR;
using TeknikServis.Application.Common.Models;

namespace TeknikServis.Application.Features.Customers.Commands
{
    // Bu bir MediatR isteğidir (IRequest) ve geriye Result içinde Müşterinin ID'sini (Guid) döner.
    public record CreateCustomerCommand(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string TaxNumber) : IRequest<Result<Guid>>;
}