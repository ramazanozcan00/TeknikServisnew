using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Payments.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Payments.Queries
{
    public record GetPaymentsQuery() : IRequest<Result<List<PaymentDto>>>;

    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, Result<List<PaymentDto>>>
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Customer> _customerRepo;

        public GetPaymentsQueryHandler(IRepository<Payment> paymentRepo, IRepository<Customer> customerRepo)
        {
            _paymentRepo = paymentRepo;
            _customerRepo = customerRepo;
        }

        public async Task<Result<List<PaymentDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepo.GetAllAsync(cancellationToken);
            var customers = await _customerRepo.GetAllAsync(cancellationToken);

            // Ödemelerle Müşterileri eşleştirip DTO'ya çeviriyoruz
            var dtoList = payments.OrderByDescending(p => p.CreatedAt).Select(p => {
                var customer = customers.FirstOrDefault(c => c.Id == p.CustomerId);

                return new PaymentDto(
                    p.Id, p.CustomerId, p.WorkOrderId, p.Amount, p.Method, p.Description, p.CreatedAt,
                    customer != null ? $"{customer.FirstName} {customer.LastName}" : "Bilinmeyen Müşteri"
                );
            }).ToList();

            return Result<List<PaymentDto>>.Success(dtoList);
        }
    }
}