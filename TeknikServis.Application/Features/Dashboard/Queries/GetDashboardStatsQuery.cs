using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Dashboard.DTOs;
using TeknikServis.Application.Interfaces;

namespace TeknikServis.Application.Features.Dashboard.Queries
{
    public record GetDashboardStatsQuery() : IRequest<Result<DashboardStatsDto>>;

    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
    {
        private readonly IDashboardQueryRepository _repository;
        public GetDashboardStatsQueryHandler(IDashboardQueryRepository repository) => _repository = repository;

        public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var stats = await _repository.GetStatsAsync(cancellationToken);
            return Result<DashboardStatsDto>.Success(stats);
        }
    }
}