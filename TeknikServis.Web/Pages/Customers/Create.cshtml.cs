using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeknikServis.Application.Features.Customers.Commands;
using TeknikServis.Application.Interfaces;

namespace TeknikServis.Web.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;

        public CreateModel(IMediator mediator, IEmailService emailService)
        {
            _mediator = mediator;
            _emailService = emailService;
        }

        [BindProperty]
        public CreateInputModel Input { get; set; } = new();

        [BindProperty]
        public string? VerificationCodeInput { get; set; }

        [TempData]
        public string? SentVerificationCode { get; set; }

        public class CreateInputModel
        {
            [Required(ErrorMessage = "Ad zorunludur.")] public string FirstName { get; set; } = string.Empty;
            [Required(ErrorMessage = "Soyad zorunludur.")] public string LastName { get; set; } = string.Empty;
            [Required(ErrorMessage = "E-posta zorunludur.")][EmailAddress(ErrorMessage = "Geçerli bir mail giriniz.")] public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Telefon zorunludur.")] public string PhoneNumber { get; set; } = string.Empty;
            [Required(ErrorMessage = "Ýl zorunludur.")] public string City { get; set; } = string.Empty;
            [Required(ErrorMessage = "Ýlçe zorunludur.")] public string District { get; set; } = string.Empty;
            public string? TaxNumber { get; set; }
            public string? TaxOffice { get; set; }
            public string? Address { get; set; }
            public string? Notes { get; set; }
        }

        public void OnGet() { }

        // Kodu Gönder
        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            ModelState.Remove("Input.City");
            ModelState.Remove("Input.District");
            ModelState.Remove("VerificationCodeInput");

            if (!ModelState.IsValid) return Page();

            string code = new Random().Next(100000, 999999).ToString();
            SentVerificationCode = code;

            try
            {
                await _emailService.SendEmailAsync(Input.Email, "KVKK Onay Kodu", $"<h3>Kodunuz: {code}</h3>");
                TempData["InfoMessage"] = "Onay kodu mail adresine gönderildi.";
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "E-posta gönderilemedi.");
            }
            return Page();
        }

        // Müþteriyi Kaydet
        public async Task<IActionResult> OnPostSaveCustomerAsync()
        {
            TempData.Keep("SentVerificationCode");

            if (!ModelState.IsValid) return Page();

            if (string.IsNullOrEmpty(VerificationCodeInput) || VerificationCodeInput != SentVerificationCode)
            {
                ModelState.AddModelError("VerificationCodeInput", "Girdiðiniz kod hatalý.");
                return Page();
            }

            var command = new CreateCustomerCommand(
                Input.FirstName, Input.LastName, Input.Email, Input.PhoneNumber,
                Input.TaxNumber, Input.TaxOffice, Input.City, Input.District, Input.Address, Input.Notes
            );

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Müþteri baþarýyla kaydedildi.";
                return RedirectToPage("/Customers/Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Hata oluþtu.");
            return Page();
        }
    }
}