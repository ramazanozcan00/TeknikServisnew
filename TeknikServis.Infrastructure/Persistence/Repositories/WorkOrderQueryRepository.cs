using Microsoft.EntityFrameworkCore;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Interfaces;

namespace TeknikServis.Infrastructure.Persistence.Repositories
{
    public class WorkOrderQueryRepository : IWorkOrderQueryRepository
    {
        private readonly AppDbContext _context;
        public WorkOrderQueryRepository(AppDbContext context) => _context = context;

        // 1. Kanban panosu için tüm aktif iş emirlerini getiren metot
        public async Task<List<WorkOrderBoardDto>> GetActiveWorkOrdersAsync(CancellationToken cancellationToken = default)
        {
            var query = from wo in _context.WorkOrders
                        join d in _context.Devices on wo.DeviceId equals d.Id
                        join c in _context.Customers on d.CustomerId equals c.Id
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

        // 2. Tek bir iş emrinin operasyon detaylarını getiren metot
        public async Task<WorkOrderDetailDto?> GetWorkOrderDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await (from wo in _context.WorkOrders
                          join d in _context.Devices on wo.DeviceId equals d.Id
                          join c in _context.Customers on d.CustomerId equals c.Id
                          where wo.Id == id
                          select new WorkOrderDetailDto (
                              wo.Id,
                              wo.WorkOrderNo,
                              wo.Description,
                              wo.TechnicianNotes,
                              wo.TotalPrice,
                              wo.Status,
                              wo.CreatedAt,
                              c.FirstName + " " + c.LastName,
                              c.PhoneNumber,
                              d.Brand,
                              d.Model,
                              d.SerialNumber
                          )).FirstOrDefaultAsync(cancellationToken);
        }

        // 3. YENİ EKLENEN METOT: Belirli bir müşterinin tüm iş emirlerini getiren metot
        public async Task<List<WorkOrderBoardDto>> GetWorkOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            // Belirli bir müşterinin tüm iş emirlerini tarihe göre yeninden eskiye sıralayarak getiriyoruz
            return await (from wo in _context.WorkOrders
                          join d in _context.Devices on wo.DeviceId equals d.Id
                          join c in _context.Customers on d.CustomerId equals c.Id
                          where c.Id == customerId
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
                          )).ToListAsync(cancellationToken);
        }
    }
}