using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Customers.DTOs;

namespace TeknikServis.Application.Features.Customers.Queries
{
    // Bu komut geriye bir CustomerDto listesi dönecek
    public record GetCustomersQuery() : IRequest<Result<List<CustomerDto>>>;
}