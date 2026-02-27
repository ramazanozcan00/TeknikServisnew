using Mapster;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            // GetAll ve EF Core metotları yerine sadece temiz GetByIdAsync kullanıyoruz
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (customer == null) return Result<CustomerDto>.Failure("Müşteri bulunamadı.");

            // Mapster otomatik olarak dönüştürür
            return Result<CustomerDto>.Success(customer.Adapt<CustomerDto>());
        }
    }
}