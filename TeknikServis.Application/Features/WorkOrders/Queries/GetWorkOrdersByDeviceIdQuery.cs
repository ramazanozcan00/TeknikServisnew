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
    public record GetWorkOrdersByDeviceIdQuery(Guid DeviceId) : IRequest<Result<List<WorkOrderDetailDto>>>;

    public class GetWorkOrdersByDeviceIdQueryHandler : IRequestHandler<GetWorkOrdersByDeviceIdQuery, Result<List<WorkOrderDetailDto>>>
    {
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Customer> _customerRepository;

        public GetWorkOrdersByDeviceIdQueryHandler(
            IRepository<WorkOrder> workOrderRepository,
            IRepository<Device> deviceRepository,
            IRepository<Customer> customerRepository)
        {
            _workOrderRepository = workOrderRepository;
            _deviceRepository = deviceRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Result<List<WorkOrderDetailDto>>> Handle(GetWorkOrdersByDeviceIdQuery request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
            if (device == null) return Result<List<WorkOrderDetailDto>>.Failure("Cihaz bulunamadı.");

            var customer = await _customerRepository.GetByIdAsync(device.CustomerId, cancellationToken);
            var allWorkOrders = await _workOrderRepository.GetAllAsync(cancellationToken);

            var deviceWorkOrders = allWorkOrders.Where(w => w.DeviceId == request.DeviceId).ToList();
            var dtoList = new List<WorkOrderDetailDto>();

            foreach (var w in deviceWorkOrders)
            {
                dtoList.Add(new WorkOrderDetailDto(
                    w.Id,
                    w.WorkOrderNo,
                    customer?.Id ?? Guid.Empty, // YENİ EKLENEN 3. PARAMETRE BURADA
                    customer != null ? $"{customer.FirstName} {customer.LastName}" : "Bilinmeyen",
                    customer?.PhoneNumber ?? "",
                    w.DeviceId,
                    device.Brand,
                    device.Model,
                    device.SerialNumber,
                    $"{device.DeviceType} - {device.Brand} {device.Model}",
                    w.Description,
                    w.TechnicianNotes,
                    w.TotalPrice,
                    w.Status,
                    w.CreatedAt
                ));
            }

            return Result<List<WorkOrderDetailDto>>.Success(dtoList.OrderByDescending(x => x.CreatedAt).ToList());
        }
    }
}