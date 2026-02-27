using FluentValidation;

namespace TeknikServis.Application.Features.Customers.Commands
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Müşteri adı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Müşteri adı en fazla 50 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Müşteri soyadı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Müşteri soyadı en fazla 50 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.");
        }
    }
}