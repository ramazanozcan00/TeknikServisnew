using TeknikServis.Application.Features.WorkOrders.DTOs;

namespace TeknikServis.Application.Interfaces
{
    // Özel okuma (Query) sözleşmemiz
    public interface IWorkOrderQueryRepository
    {
        Task<List<WorkOrderBoardDto>> GetActiveWorkOrdersAsync(CancellationToken cancellationToken = default);
    }
}