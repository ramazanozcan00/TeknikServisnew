using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TeknikServis.Application.Features.Customers.Commands;
using TeknikServis.Application.Interfaces;
using TeknikServis.Application.Common.Models; // MailSettings için

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

        // Sayfa durumu kontrolü için (Kod gönderildi mi?)
        [TempData]
        public string? SentVerificationCode { get; set; }

        public class CreateInputModel
        {
            [Required(ErrorMessage = "Ad alaný zorunludur.")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Soyad alaný zorunludur.")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "E-Posta zorunludur.")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Telefon numarasý zorunludur.")]
            public string PhoneNumber { get; set; } = string.Empty;

            public string? TaxNumber { get; set; }
            public string? TaxOffice { get; set; }

            [Required(ErrorMessage = "Ýl seçimi zorunludur.")]
            public string City { get; set; } = string.Empty;

            [Required(ErrorMessage = "Ýlçe seçimi zorunludur.")]
            public string District { get; set; } = string.Empty;

            public string? Address { get; set; }
            public string? Notes { get; set; }
        }

        public void OnGet() { }

        // 1. ADIM: KVKK Onay Kodu Gönder
        public async Task<IActionResult> OnPostSendCodeAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 6 Haneli rastgele kod üret
            string code = new Random().Next(100000, 999999).ToString();
            SentVerificationCode = code;

            string mailBody = $@"
                <h3>KVKK Aydýnlatma Metni Onayý</h3>
                <p>Sayýn {Input.FirstName} {Input.LastName},</p>
                <p>Teknik servis kayýt iþlemlerinizin tamamlanmasý ve KVKK kapsamýnda verilerinizin iþlenmesine onay vermeniz için doðrulama kodunuz:</p>
                <h2 style='color:blue;'>{code}</h2>
                <p>Lütfen bu kodu ilgili görevliye bildiriniz.</p>";

            try
            {
                await _emailService.SendEmailAsync(Input.Email, "KVKK Onay Kodu", mailBody);
                TempData["InfoMessage"] = "Onay kodu müþterinin e-posta adresine gönderildi.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "E-posta gönderilirken bir hata oluþtu: " + ex.Message);
            }

            return Page();
        }

        // 2. ADIM: Kodu Doðrula ve Kaydý Tamamla
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (string.IsNullOrEmpty(VerificationCodeInput) || VerificationCodeInput != SentVerificationCode)
            {
                ModelState.AddModelError(nameof(VerificationCodeInput), "Girdiðiniz onay kodu hatalý veya süresi dolmuþ.");
                return Page();
            }

            var command = new CreateCustomerCommand(
                Input.FirstName,
                Input.LastName,
                Input.Email,
                Input.PhoneNumber,
                Input.TaxNumber,
                Input.TaxOffice,
                Input.City,
                Input.District,
                Input.Address,
                Input.Notes
            );

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Müþteri KVKK onayý alýnarak baþarýyla eklendi.";
                return RedirectToPage("/Customers/Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Bir hata oluþtu.");
            return Page();
        }
    }
}