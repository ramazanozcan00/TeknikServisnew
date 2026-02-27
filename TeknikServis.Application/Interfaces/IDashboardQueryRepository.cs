using TeknikServis.Application.Features.Dashboard.DTOs;

namespace TeknikServis.Application.Interfaces
{
    public interface IDashboardQueryRepository
    {
        Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
    }
}