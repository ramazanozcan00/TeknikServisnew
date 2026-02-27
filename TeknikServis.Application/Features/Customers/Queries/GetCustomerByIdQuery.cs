using Mapster;
using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Customers.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Queries
{
    public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
    {
        private readonly IRepository<Customer> _repository;
        public GetCustomerByIdQueryHandler(IRepository<Customer> repository) => _repository = repository;

        public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null) return Result<CustomerDto>.Failure("Müşteri bulunamadı.");
            return Result<CustomerDto>.Success(customer.Adapt<CustomerDto>());
        }
    }
}