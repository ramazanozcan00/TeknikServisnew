using MediatR;
using System;
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
    public record GetWorkOrderDetailQuery(Guid Id) : IRequest<Result<WorkOrderDetailDto>>;

    public class GetWorkOrderDetailQueryHandler : IRequestHandler<GetWorkOrderDetailQuery, Result<WorkOrderDetailDto>>
    {
        // İhtiyacımız olan 3 tabloyu da ayrı ayrı içeri alıyoruz
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Customer> _customerRepository;

        public GetWorkOrderDetailQueryHandler(
            IRepository<WorkOrder> workOrderRepository,
            IRepository<Device> deviceRepository,
            IRepository<Customer> customerRepository)
        {
            _workOrderRepository = workOrderRepository;
            _deviceRepository = deviceRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Result<WorkOrderDetailDto>> Handle(GetWorkOrderDetailQuery request, CancellationToken cancellationToken)
        {
            var workOrder = await _workOrderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (workOrder == null) return Result<WorkOrderDetailDto>.Failure("İş emri bulunamadı.");

            // İş emrinden Cihazı, Cihazdan da Müşteriyi manuel buluyoruz (En güvenli yöntem)
            var device = await _deviceRepository.GetByIdAsync(workOrder.DeviceId, cancellationToken);
            var customer = device != null ? await _customerRepository.GetByIdAsync(device.CustomerId, cancellationToken) : null;

            // DTO'nun beklediği 15 parametreyi eksiksiz şekilde veriyoruz
            var dto = new WorkOrderDetailDto(
                workOrder.Id,
                workOrder.WorkOrderNo,
                customer?.Id ?? Guid.Empty, // 3. Parametre: CustomerId (Tahsilat için gerekliydi)
                customer != null ? $"{customer.FirstName} {customer.LastName}" : "Bilinmeyen",
                customer?.PhoneNumber ?? "",
                workOrder.DeviceId,
                device?.Brand ?? "",
                device?.Model ?? "",
                device?.SerialNumber ?? "",
                $"{device?.DeviceType} - {device?.Brand} {device?.Model}",
                workOrder.Description,
                workOrder.TechnicianNotes,
                workOrder.TotalPrice,
                workOrder.Status,
                workOrder.CreatedAt
            );

            return Result<WorkOrderDetailDto>.Success(dto);
        }
    }
}