using Mapster;
using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Customers.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;
using System.Linq; // Standart LINQ için gerekli
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
            // 1. Veritabanından tüm müşterileri çek (EF Core metotları olmadan)
            var customers = await _customerRepository.GetAllAsync(cancellationToken);

            // 2. Arama kelimesi varsa listeyi bellekte filtrele
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                customers = customers.Where(c =>
                    c.FirstName.ToLower().Contains(searchTerm) ||
                    c.LastName.ToLower().Contains(searchTerm) ||
                    c.PhoneNumber.Contains(searchTerm) ||
                    (c.CustomerCode != null && c.CustomerCode.ToLower().Contains(searchTerm))
                ).ToList();
            }

            // 3. Yeniden eskiye sırala ve Mapster kullanarak DTO'ya dönüştür
            var dtoList = customers.OrderByDescending(c => c.CreatedAt).Adapt<List<CustomerDto>>();

            // 4. Sonucu dön
            return Result<List<CustomerDto>>.Success(dtoList);
        }
    }
}