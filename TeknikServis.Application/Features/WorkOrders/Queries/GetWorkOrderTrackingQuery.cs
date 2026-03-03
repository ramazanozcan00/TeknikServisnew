using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities.WorkOrders;
using TeknikServis.Domain.Entities.Devices;
using TeknikServis.Domain.Entities;
using TeknikServis.Domain.Enums;
using System;

namespace TeknikServis.Application.Features.WorkOrders.Queries
{
    // 1. Dışarıdan gelen talep (Sadece Takip Numarası)
    public record GetWorkOrderTrackingQuery(string WorkOrderNo) : IRequest<Result<WorkOrderTrackingDto>>;

    // 2. Müşteriye gösterilecek kısıtlı ve güvenli veri paketi
    public record WorkOrderTrackingDto(
        string WorkOrderNo,
        string DeviceName,
        string FaultDescription,
        WorkOrderStatus Status,
        string CustomerNameMasked, // KVKK gereği gizlenmiş isim
        DateTime CreatedAt,
        decimal TotalPrice);

    // 3. Veritabanında arama yapan kurye
    public class GetWorkOrderTrackingQueryHandler : IRequestHandler<GetWorkOrderTrackingQuery, Result<WorkOrderTrackingDto>>
    {
        private readonly IRepository<WorkOrder> _workOrderRepo;
        private readonly IRepository<Device> _deviceRepo;
        private readonly IRepository<Customer> _customerRepo;

        public GetWorkOrderTrackingQueryHandler(
            IRepository<WorkOrder> workOrderRepo,
            IRepository<Device> deviceRepo,
            IRepository<Customer> customerRepo)
        {
            _workOrderRepo = workOrderRepo;
            _deviceRepo = deviceRepo;
            _customerRepo = customerRepo;
        }

        public async Task<Result<WorkOrderTrackingDto>> Handle(GetWorkOrderTrackingQuery request, CancellationToken cancellationToken)
        {
            // İş emrini takip numarasına göre bul
            var workOrders = await _workOrderRepo.GetAllAsync(cancellationToken);
            var workOrder = workOrders.FirstOrDefault(w => w.WorkOrderNo != null && w.WorkOrderNo.ToUpper() == request.WorkOrderNo.ToUpper());

            if (workOrder == null)
                return Result<WorkOrderTrackingDto>.Failure("Bu numaraya ait bir servis kaydı bulunamadı. Lütfen numarayı kontrol ediniz.");

            // Cihaz ve Müşteri bilgilerini çek
            var device = await _deviceRepo.GetByIdAsync(workOrder.DeviceId, cancellationToken);
            var customer = await _customerRepo.GetByIdAsync(device.CustomerId, cancellationToken);

            // Müşteri ismini gizle (Örn: Ramazan Özcan -> R*** Ö***)
            string maskedName = "Bilinmeyen";
            if (customer != null)
            {
                string first = !string.IsNullOrEmpty(customer.FirstName) ? customer.FirstName.Substring(0, 1) + "***" : "";
                string last = !string.IsNullOrEmpty(customer.LastName) ? customer.LastName.Substring(0, 1) + "***" : "";
                maskedName = $"{first} {last}".Trim();
            }

            string deviceName = device != null ? $"{device.Brand} {device.Model}" : "Bilinmeyen Cihaz";

            var dto = new WorkOrderTrackingDto(
                workOrder.WorkOrderNo!,
                deviceName,
                workOrder.Description, // 1. HATA ÇÖZÜMÜ: FaultDescription yerine Description kullanıldı
                workOrder.Status,
                maskedName,
                workOrder.CreatedAt,
                workOrder.TotalPrice ?? 0m // 2. HATA ÇÖZÜMÜ: Eğer fiyat null ise ekrana 0 TL gönder (decimal? -> decimal çevirisi)
            );

            return Result<WorkOrderTrackingDto>.Success(dto);
        }
    }
}