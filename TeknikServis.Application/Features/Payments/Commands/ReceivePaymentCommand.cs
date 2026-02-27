using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Application.Features.Payments.Commands
{
    public record ReceivePaymentCommand(
        Guid CustomerId,
        Guid? WorkOrderId,
        decimal Amount,
        PaymentMethod Method,
        string Description
    ) : IRequest<Result<Guid>>;

    public class ReceivePaymentCommandHandler : IRequestHandler<ReceivePaymentCommand, Result<Guid>>
    {
        private readonly IRepository<Payment> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ReceivePaymentCommandHandler(IRepository<Payment> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(ReceivePaymentCommand request, CancellationToken cancellationToken)
        {
            // Yeni tahsilat kaydını oluşturuyoruz
            var payment = Payment.Create(request.CustomerId, request.WorkOrderId, request.Amount, request.Method, request.Description);

            await _repository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(payment.Id);
        }
    }
}