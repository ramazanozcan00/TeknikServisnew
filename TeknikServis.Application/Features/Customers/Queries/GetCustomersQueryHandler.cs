using Mapster;
using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Customers.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Customers.Queries
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
    {
        private readonly IRepository<Customer> _customerRepository;

        public GetCustomersQueryHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            // 1. Veritabanından tüm müşterileri çek
            var customers = await _customerRepository.GetAllAsync(cancellationToken);

            // 2. Mapster kullanarak Customer listesini -> CustomerDto listesine dönüştür
            var dtoList = customers.Adapt<List<CustomerDto>>();

            // 3. Sonucu dön
            return Result<List<CustomerDto>>.Success(dtoList);
        }
    }
}