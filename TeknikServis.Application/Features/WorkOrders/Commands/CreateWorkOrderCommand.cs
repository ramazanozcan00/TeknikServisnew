using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities.WorkOrders;

namespace TeknikServis.Application.Features.WorkOrders.Commands
{
    // Arayüzden sadece Cihazın ID'si ve Müşterinin Şikayeti (Description) gelecek
    public record CreateWorkOrderCommand(Guid DeviceId, string Description) : IRequest<Result<Guid>>;

    public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Result<Guid>>
    {
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkOrderCommandHandler(IRepository<WorkOrder> workOrderRepository, IUnitOfWork unitOfWork)
        {
            _workOrderRepository = workOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. İş emrini oluştur (Domain katmanındaki kuralımıza göre otomatik "Bekliyor" statüsünde açılacak)
            var workOrder = WorkOrder.Create(request.DeviceId, request.Description);

            // 2. Veritabanına kaydet
            await _workOrderRepository.AddAsync(workOrder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Oluşan yeni iş emrinin ID'sini dön
            return Result<Guid>.Success(workOrder.Id);
        }
    }
}