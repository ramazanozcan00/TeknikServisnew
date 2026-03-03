using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.SpareParts.Queries
{
    public record GetSparePartsQuery() : IRequest<Result<List<SparePartDto>>>;

    public record SparePartDto(Guid Id, string Name, string Code, decimal PurchasePrice, decimal SalePrice, int StockQuantity, string Unit);

    public class GetSparePartsQueryHandler : IRequestHandler<GetSparePartsQuery, Result<List<SparePartDto>>>
    {
        private readonly IRepository<SparePart> _repository;
        public GetSparePartsQueryHandler(IRepository<SparePart> repository) => _repository = repository;

        public async Task<Result<List<SparePartDto>>> Handle(GetSparePartsQuery request, CancellationToken cancellationToken)
        {
            var parts = await _repository.GetAllAsync(cancellationToken);
            var dtos = parts.Select(p => new SparePartDto(
                p.Id, p.Name, p.Code, p.PurchasePrice, p.SalePrice, p.StockQuantity, p.Unit
            )).OrderBy(p => p.Name).ToList();

            return Result<List<SparePartDto>>.Success(dtos);
        }
    }
}