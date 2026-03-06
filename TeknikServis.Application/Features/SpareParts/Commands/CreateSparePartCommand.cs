using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.SpareParts.Commands
{
    public class CreateSparePartCommandHandler : IRequestHandler<CreateSparePartCommand, Result<Guid>>
    {
        private readonly IRepository<SparePart> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSparePartCommandHandler(IRepository<SparePart> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateSparePartCommand request, CancellationToken cancellationToken)
        {
            var sparePart = SparePart.Create(
                request.Name,
                request.Barcode,
                request.SparePartCode,
                request.PurchasePrice,
                request.SalePrice,
                request.CriticalStockLevel
            );

            // Seri numaralarını AddItem metodu ile ekle
            if (request.SerialNumbers != null)
            {
                foreach (var sn in request.SerialNumbers.Where(s => !string.IsNullOrEmpty(s)))
                {
                    sparePart.AddItem(sn, request.PurchaseInvoiceNo);
                }
            }

            await _repository.AddAsync(sparePart);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(sparePart.Id);
        }
    }
}