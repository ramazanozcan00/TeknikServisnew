using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Customers.DTOs;

namespace TeknikServis.Application.Features.Customers.Queries
{
    // SearchTerm (Arama Kelimesi) parametresi eklendi. Varsayılanı boş (null) bırakıldı.
    public record GetCustomersQuery(string? SearchTerm = null) : IRequest<Result<List<CustomerDto>>>;
}