using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.SpareParts.Commands
{
    // Güncellenecek verileri taşıyan paket
    public record UpdateSparePartCommand(Guid Id, string Name, string Code, decimal PurchasePrice, decimal SalePrice, int CriticalLevel, string Unit) : IRequest<Result<bool>>;

    // Veritabanında güncellemeyi yapacak asıl işçi (Handler)
    public class UpdateSparePartCommandHandler : IRequestHandler<UpdateSparePartCommand, Result<bool>>
    {
        private readonly IRepository<SparePart> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSparePartCommandHandler(IRepository<SparePart> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateSparePartCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (part == null) return Result<bool>.Failure("Parça bulunamadı.");

            // Domain içindeki Update metodunu kullanarak verileri güncelliyoruz
            part.Update(request.Name, request.Code, request.PurchasePrice, request.SalePrice, request.CriticalLevel, request.Unit);

            _repository.Update(part);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}