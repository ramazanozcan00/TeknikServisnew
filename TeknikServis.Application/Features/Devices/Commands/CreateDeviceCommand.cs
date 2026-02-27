using MediatR;
using TeknikServis.Application.Common.Models;
using TeknikServis.Application.Interfaces;
using TeknikServis.Domain.Entities.Devices;

namespace TeknikServis.Application.Features.Devices.Commands
{
    // Arayüzden gelecek veriler
    public record CreateDeviceCommand(
        Guid CustomerId,
        string Brand,
        string Model,
        string SerialNumber,
        string DeviceType,
        string? Note) : IRequest<Result<Guid>>;

    // Verileri işleyip kaydeden işçi
    public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, Result<Guid>>
    {
        private readonly IRepository<Device> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeviceCommandHandler(IRepository<Device> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = Device.Create(request.CustomerId, request.Brand, request.Model, request.SerialNumber, request.DeviceType, request.Note);

            await _repository.AddAsync(device, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(device.Id);
        }
    }
}