using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities.WorkOrders;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Application.Features.WorkOrders.Commands
{
    public record UpdateWorkOrderDetailsCommand(Guid Id, WorkOrderStatus Status, string? TechnicianNotes, decimal? TotalPrice) : IRequest<Result<bool>>;

    public class UpdateWorkOrderDetailsCommandHandler : IRequestHandler<UpdateWorkOrderDetailsCommand, Result<bool>>
    {
        private readonly IRepository<WorkOrder> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkOrderDetailsCommandHandler(IRepository<WorkOrder> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateWorkOrderDetailsCommand request, CancellationToken cancellationToken)
        {
            var workOrder = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (workOrder == null) return Result<bool>.Failure("Kayıt bulunamadı.");

            // Domain varlığımızdaki metodu çağırıyoruz
            workOrder.UpdateProgress(request.Status, request.TechnicianNotes, request.TotalPrice);

            _repository.Update(workOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}