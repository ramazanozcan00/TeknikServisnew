using FluentValidation;

namespace TeknikServis.Application.Features.Devices.Commands
{
    public class CreateDeviceCommandValidator : AbstractValidator<CreateDeviceCommand>
    {
        public CreateDeviceCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Müşteri kimliği zorunludur.");
            RuleFor(x => x.Brand).NotEmpty().WithMessage("Marka giriniz.");
            RuleFor(x => x.Model).NotEmpty().WithMessage("Model giriniz.");
            RuleFor(x => x.SerialNumber).NotEmpty().WithMessage("Seri no giriniz.");
            RuleFor(x => x.DeviceType).NotEmpty().WithMessage("Cihaz türü seçiniz.");
        }
    }
}