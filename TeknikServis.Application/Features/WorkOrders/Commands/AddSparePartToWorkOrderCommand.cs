using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;
using TeknikServis.Domain.Entities.WorkOrders;

namespace TeknikServis.Application.Features.WorkOrders.Commands
{
    public record AddSparePartToWorkOrderCommand(Guid WorkOrderId, Guid SparePartId, int Quantity) : IRequest<Result<bool>>;

    public class AddSparePartToWorkOrderCommandHandler : IRequestHandler<AddSparePartToWorkOrderCommand, Result<bool>>
    {
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<SparePart> _sparePartRepository;
        private readonly IRepository<WorkOrderSparePart> _usedPartsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddSparePartToWorkOrderCommandHandler(
            IRepository<WorkOrder> workOrderRepository,
            IRepository<SparePart> sparePartRepository,
            IRepository<WorkOrderSparePart> usedPartsRepository,
            IUnitOfWork unitOfWork)
        {
            _workOrderRepository = workOrderRepository;
            _sparePartRepository = sparePartRepository;
            _usedPartsRepository = usedPartsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AddSparePartToWorkOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Yedek parçayı bul
            var sparePart = await _sparePartRepository.GetByIdAsync(request.SparePartId, cancellationToken);
            if (sparePart == null) return Result<bool>.Failure("Seçilen yedek parça bulunamadı.");

            // 2. Stok düşme kontrolü (Domain varlığı içindeki yeteneği kullanıyoruz)
            if (!sparePart.DecreaseStock(request.Quantity))
            {
                return Result<bool>.Failure($"Yetersiz stok! Depoda sadece {sparePart.StockQuantity} adet var.");
            }

            // 3. İş emrini bul
            var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
            if (workOrder == null) return Result<bool>.Failure("İş emri bulunamadı.");

            // 4. Kullanılan parçayı kayıt defterine (ara tabloya) ekle
            var usedPart = WorkOrderSparePart.Create(
                request.WorkOrderId,
                request.SparePartId,
                sparePart.Name,
                request.Quantity,
                sparePart.SalePrice);

            await _usedPartsRepository.AddAsync(usedPart, cancellationToken);

            // 5. İş emrinin toplam tutarını ve notunu otomatik güncelle
            decimal newTotal = (workOrder.TotalPrice ?? 0) + (request.Quantity * sparePart.SalePrice);
            string extraNote = $"\n- [SİSTEM] {request.Quantity} adet {sparePart.Name} kullanıldı.";
            string newNote = string.IsNullOrWhiteSpace(workOrder.TechnicianNotes) ? extraNote.Trim() : workOrder.TechnicianNotes + extraNote;

            workOrder.UpdateProgress(workOrder.Status, newNote, newTotal);

            // 6. Tüm değişiklikleri kaydet
            _sparePartRepository.Update(sparePart);
            _workOrderRepository.Update(workOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}