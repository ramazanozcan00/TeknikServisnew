using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.Payments.Queries
{
    // İş emrinin ID'sini alıp geriye true/false (ödendi/ödenmedi) döner
    public record CheckWorkOrderPaymentQuery(Guid WorkOrderId) : IRequest<Result<bool>>;

    public class CheckWorkOrderPaymentQueryHandler : IRequestHandler<CheckWorkOrderPaymentQuery, Result<bool>>
    {
        private readonly IRepository<Payment> _paymentRepository;

        public CheckWorkOrderPaymentQueryHandler(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<bool>> Handle(CheckWorkOrderPaymentQuery request, CancellationToken cancellationToken)
        {
            // Bu iş emri ID'sine ait bir ödeme var mı diye kasaya bakıyoruz
            var payments = await _paymentRepository.FindAsync(p => p.WorkOrderId == request.WorkOrderId, cancellationToken);

            // Eğer listede eleman varsa (Any) true döner, yoksa false
            return Result<bool>.Success(payments.Any());
        }
    }
}