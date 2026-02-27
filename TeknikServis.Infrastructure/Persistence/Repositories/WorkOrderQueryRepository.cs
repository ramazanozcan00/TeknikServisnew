using Microsoft.EntityFrameworkCore;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Interfaces;

namespace TeknikServis.Infrastructure.Persistence.Repositories
{
    public class WorkOrderQueryRepository : IWorkOrderQueryRepository
    {
        private readonly AppDbContext _context;
        public WorkOrderQueryRepository(AppDbContext context) => _context = context;

        public async Task<List<WorkOrderBoardDto>> GetActiveWorkOrdersAsync(CancellationToken cancellationToken = default)
        {
            // LINQ Join ile Müşteri, Cihaz ve İş Emrini birleştiriyoruz
            var query = from wo in _context.WorkOrders
                        join d in _context.Devices on wo.DeviceId equals d.Id
                        join c in _context.Customers on d.CustomerId equals c.Id
                        // İptal edilenleri veya Teslim edilenleri bu panoda göstermiyoruz
                        where wo.Status != Domain.Enums.WorkOrderStatus.TeslimEdildi &&
                              wo.Status != Domain.Enums.WorkOrderStatus.IptalEdildi
                        orderby wo.CreatedAt descending
                        select new WorkOrderBoardDto(
                            wo.Id,
                            wo.DeviceId,
                            wo.WorkOrderNo,
                            c.FirstName + " " + c.LastName,
                            d.Brand + " " + d.Model,
                            wo.Description,
                            wo.Status,
                            wo.CreatedAt
                        );

            return await query.ToListAsync(cancellationToken);
        }
    }
}