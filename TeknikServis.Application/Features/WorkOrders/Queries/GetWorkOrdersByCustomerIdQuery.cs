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
    public record GetWorkOrdersByCustomerIdQuery(Guid CustomerId) : IRequest<Result<List<WorkOrderDetailDto>>>;

    public class GetWorkOrdersByCustomerIdQueryHandler : IRequestHandler<GetWorkOrdersByCustomerIdQuery, Result<List<WorkOrderDetailDto>>>
    {
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Customer> _customerRepository;

        public GetWorkOrdersByCustomerIdQueryHandler(
            IRepository<WorkOrder> workOrderRepository,
            IRepository<Device> deviceRepository,
            IRepository<Customer> customerRepository)
        {
            _workOrderRepository = workOrderRepository;
            _deviceRepository = deviceRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Result<List<WorkOrderDetailDto>>> Handle(GetWorkOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null) return Result<List<WorkOrderDetailDto>>.Failure("Müşteri bulunamadı.");

            var allDevices = await _deviceRepository.GetAllAsync(cancellationToken);
            var customerDeviceIds = allDevices.Where(d => d.CustomerId == request.CustomerId).Select(d => d.Id).ToList();

            var allWorkOrders = await _workOrderRepository.GetAllAsync(cancellationToken);
            var customerWorkOrders = allWorkOrders.Where(w => customerDeviceIds.Contains(w.DeviceId)).ToList();

            var dtoList = new List<WorkOrderDetailDto>();

            foreach (var w in customerWorkOrders)
            {
                var device = allDevices.FirstOrDefault(d => d.Id == w.DeviceId);

                dtoList.Add(new WorkOrderDetailDto(
                    w.Id,
                    w.WorkOrderNo,
                    customer.Id, // YENİ EKLENEN 3. PARAMETRE BURADA
                    $"{customer.FirstName} {customer.LastName}",
                    customer.PhoneNumber,
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

            return Result<List<WorkOrderDetailDto>>.Success(dtoList.OrderByDescending(x => x.CreatedAt).ToList());
        }
    }
}