using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities.WorkOrders;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Application.Features.WorkOrders.Commands
{
    public record UpdateWorkOrderStatusCommand(Guid WorkOrderId, WorkOrderStatus NewStatus) : IRequest<Result<bool>>;

    public class UpdateWorkOrderStatusCommandHandler : IRequestHandler<UpdateWorkOrderStatusCommand, Result<bool>>
    {
        private readonly IRepository<WorkOrder> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkOrderStatusCommandHandler(IRepository<WorkOrder> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var workOrder = await _repository.GetByIdAsync(request.WorkOrderId, cancellationToken);
            if (workOrder == null) return Result<bool>.Failure("İş emri bulunamadı.");

            // Domain metodumuzu çağırarak sadece statüyü güncelliyoruz (Notlar ve fiyatı şimdilik koruyoruz)
            workOrder.UpdateProgress(request.NewStatus, workOrder.TechnicianNotes, workOrder.TotalPrice);

            _repository.Update(workOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}