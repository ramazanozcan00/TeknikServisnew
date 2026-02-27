using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Features.WorkOrders.DTOs;

namespace TeknikServis.Application.Interfaces
{
    public interface IWorkOrderQueryRepository
    {
        // 1. Kanban panosundaki aktif iş emirlerini getiren metot
        Task<List<WorkOrderBoardDto>> GetActiveWorkOrdersAsync(CancellationToken cancellationToken = default);

        // 2. İş emrinin operasyon detaylarını getiren metot (Önceki adımda eklemiştik)
        Task<WorkOrderDetailDto?> GetWorkOrderDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<WorkOrderBoardDto>> GetWorkOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}