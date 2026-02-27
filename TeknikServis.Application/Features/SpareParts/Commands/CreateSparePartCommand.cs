using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.SpareParts.Commands
{
    public record CreateSparePartCommand(
        string Name,
        string Code,
        decimal PurchasePrice,
        decimal SalePrice,
        int StockQuantity,
        int CriticalStockLevel,
        string Unit
    ) : IRequest<Result<Guid>>;

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
                request.Name, request.Code, request.PurchasePrice, request.SalePrice,
                request.StockQuantity, request.CriticalStockLevel, request.Unit
            );

            await _repository.AddAsync(sparePart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(sparePart.Id);
        }
    }
}