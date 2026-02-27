using Mapster;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.SpareParts.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;

namespace TeknikServis.Application.Features.SpareParts.Queries
{
    // Arama kelimesi alabilen liste talebimiz
    public record GetSparePartsQuery(string? SearchTerm = null) : IRequest<Result<List<SparePartDto>>>;

    public class GetSparePartsQueryHandler : IRequestHandler<GetSparePartsQuery, Result<List<SparePartDto>>>
    {
        private readonly IRepository<SparePart> _repository;

        public GetSparePartsQueryHandler(IRepository<SparePart> repository) => _repository = repository;

        public async Task<Result<List<SparePartDto>>> Handle(GetSparePartsQuery request, CancellationToken cancellationToken)
        {
            // EF Core olmadan temiz okuma işlemi
            var parts = await _repository.GetAllAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                parts = parts.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Code.ToLower().Contains(search)).ToList();
            }

            // Mapster ile otomatik DTO dönüştürme ve İsme göre sıralama
            var dtoList = parts.OrderBy(p => p.Name).Adapt<List<SparePartDto>>();
            return Result<List<SparePartDto>>.Success(dtoList);
        }
    }
}