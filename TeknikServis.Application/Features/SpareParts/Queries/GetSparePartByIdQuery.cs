using Mapster; // Senin projendeki profesyonel eşleştirici
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.SpareParts.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.SpareParts.Queries
{
    public record GetSparePartByIdQuery(Guid Id) : IRequest<Result<SparePartDto>>;

    public class GetSparePartByIdQueryHandler : IRequestHandler<GetSparePartByIdQuery, Result<SparePartDto>>
    {
        private readonly IRepository<SparePart> _repository;
        public GetSparePartByIdQueryHandler(IRepository<SparePart> repository) => _repository = repository;

        public async Task<Result<SparePartDto>> Handle(GetSparePartByIdQuery request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (part == null) return Result<SparePartDto>.Failure("Parça bulunamadı.");

            // Manuel yazmak yerine senin Mapster altyapını kullanıyoruz!
            var dto = part.Adapt<SparePartDto>();

            return Result<SparePartDto>.Success(dto);
        }
    }
}