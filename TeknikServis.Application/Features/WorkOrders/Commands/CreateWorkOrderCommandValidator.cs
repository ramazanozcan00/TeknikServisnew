using FluentValidation;

namespace TeknikServis.Application.Features.WorkOrders.Commands
{
    public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
    {
        public CreateWorkOrderCommandValidator()
        {
            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Cihaz seçimi zorunludur.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Arıza/Şikayet açıklaması boş bırakılamaz.")
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");
        }
    }
}