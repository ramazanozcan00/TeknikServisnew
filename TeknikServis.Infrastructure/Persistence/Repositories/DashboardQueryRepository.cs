using Microsoft.EntityFrameworkCore;
using TeknikServis.Application.Features.Dashboard.DTOs;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Infrastructure.Persistence.Repositories
{
    public class DashboardQueryRepository : IDashboardQueryRepository
    {
        private readonly AppDbContext _context;

        public DashboardQueryRepository(AppDbContext context) => _context = context;

        public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            // Veritabanını yormadan doğrudan sayma (COUNT) işlemleri yapıyoruz
            var totalCustomers = await _context.Customers.CountAsync(cancellationToken);
            var totalDevices = await _context.Devices.CountAsync(cancellationToken);

            var activeWorkOrders = await _context.WorkOrders
                .CountAsync(x => x.Status == WorkOrderStatus.Bekliyor || x.Status == WorkOrderStatus.Onarimda, cancellationToken);

            var completedWorkOrders = await _context.WorkOrders
                .CountAsync(x => x.Status == WorkOrderStatus.Tamamlandi || x.Status == WorkOrderStatus.TeslimEdildi, cancellationToken);

            return new DashboardStatsDto(totalCustomers, totalDevices, activeWorkOrders, completedWorkOrders);
        }
        // YENİ EKLENEN METOT
        public async Task<WorkOrderDetailDto?> GetWorkOrderDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Veritabanı işlemleri (EntityFramework) sadece Infrastructure'da yapılır!
            return await (from wo in _context.WorkOrders
                          join d in _context.Devices on wo.DeviceId equals d.Id
                          join c in _context.Customers on d.CustomerId equals c.Id
                          where wo.Id == id
                          select new WorkOrderDetailDto(
                              wo.Id, wo.WorkOrderNo, wo.Description, wo.TechnicianNotes, wo.TotalPrice, wo.Status, wo.CreatedAt,
                              c.FirstName + " " + c.LastName, c.PhoneNumber,
                              d.Brand, d.Model, d.SerialNumber
                          )).FirstOrDefaultAsync(cancellationToken);
        }
    }
}