using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Payments.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Payments.Queries
{
    public record GetPaymentByIdQuery(Guid Id) : IRequest<Result<PaymentDto>>;

    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Customer> _customerRepo;

        public GetPaymentByIdQueryHandler(IRepository<Payment> paymentRepo, IRepository<Customer> customerRepo)
        {
            _paymentRepo = paymentRepo;
            _customerRepo = customerRepo;
        }

        public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var p = await _paymentRepo.GetByIdAsync(request.Id, cancellationToken);
            if (p == null) return Result<PaymentDto>.Failure("Tahsilat bulunamadı.");

            var customer = await _customerRepo.GetByIdAsync(p.CustomerId, cancellationToken);

            var dto = new PaymentDto(
                p.Id, p.CustomerId, p.WorkOrderId, p.ReceiptNo ?? "ESKİ-KAYIT",
                p.Amount, p.Method, p.Description, p.CreatedAt,
                customer != null ? $"{customer.FirstName} {customer.LastName}" : "Bilinmeyen"
            );

            return Result<PaymentDto>.Success(dto);
        }
    }
}