using Mapster;
using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Features.Devices.DTOs;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities.Devices;

namespace TeknikServis.Application.Features.Devices.Queries
{
    // Bize Müşteri ID'si lazım
    public record GetDevicesByCustomerIdQuery(Guid CustomerId) : IRequest<Result<List<DeviceDto>>>;

    public class GetDevicesByCustomerIdQueryHandler : IRequestHandler<GetDevicesByCustomerIdQuery, Result<List<DeviceDto>>>
    {
        private readonly IRepository<Device> _repository;
        public GetDevicesByCustomerIdQueryHandler(IRepository<Device> repository) => _repository = repository;

        public async Task<Result<List<DeviceDto>>> Handle(GetDevicesByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            // Sadece bu müşteriye ait cihazları bul
            var devices = await _repository.FindAsync(x => x.CustomerId == request.CustomerId, cancellationToken);
            return Result<List<DeviceDto>>.Success(devices.Adapt<List<DeviceDto>>());
        }
    }
}