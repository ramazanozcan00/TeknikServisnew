using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities;
using TeknikServis.Domain.Entities.Devices;
using TeknikServis.Domain.Entities.WorkOrders;

namespace TeknikServis.Application.Features.WorkOrders.Queries
{
    public record GetWorkOrdersQuery(string? SearchTerm = null) : IRequest<Result<List<WorkOrderDetailDto>>>;

    public class GetWorkOrdersQueryHandler : IRequestHandler<GetWorkOrdersQuery, Result<List<WorkOrderDetailDto>>>
    {
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Customer> _customerRepository;

        public GetWorkOrdersQueryHandler(
            IRepository<WorkOrder> workOrderRepository,
            IRepository<Device> deviceRepository,
            IRepository<Customer> customerRepository)
        {
            _workOrderRepository = workOrderRepository;
            _deviceRepository = deviceRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Result<List<WorkOrderDetailDto>>> Handle(GetWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            // Tüm verileri bellekten güvenli şekilde çekiyoruz
            var workOrders = await _workOrderRepository.GetAllAsync(cancellationToken);
            var devices = await _deviceRepository.GetAllAsync(cancellationToken);
            var customers = await _customerRepository.GetAllAsync(cancellationToken);

            var dtoList = new List<WorkOrderDetailDto>();

            foreach (var w in workOrders)
            {
                var device = devices.FirstOrDefault(d => d.Id == w.DeviceId);
                var customer = device != null ? customers.FirstOrDefault(c => c.Id == device.CustomerId) : null;

                // Arama filtresi
                bool match = true;
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var search = request.SearchTerm.ToLower();
                    match = w.WorkOrderNo.ToLower().Contains(search) ||
                            (customer != null && customer.FirstName.ToLower().Contains(search)) ||
                            (customer != null && customer.LastName.ToLower().Contains(search));
                }

                if (match)
                {
                    dtoList.Add(new WorkOrderDetailDto(
                        w.Id,
                        w.WorkOrderNo,
                        customer?.Id ?? Guid.Empty, // 3. Parametre eklendi
                        customer != null ? $"{customer.FirstName} {customer.LastName}" : "Bilinmeyen",
                        customer?.PhoneNumber ?? "",
                        w.DeviceId,
                        device?.Brand ?? "",
                        device?.Model ?? "",
                        device?.SerialNumber ?? "",
                        $"{device?.DeviceType} - {device?.Brand} {device?.Model}",
                        w.Description,
                        w.TechnicianNotes,
                        w.TotalPrice,
                        w.Status,
                        w.CreatedAt
                    ));
                }
            }

            return Result<List<WorkOrderDetailDto>>.Success(dtoList.OrderByDescending(x => x.CreatedAt).ToList());
        }
    }
}