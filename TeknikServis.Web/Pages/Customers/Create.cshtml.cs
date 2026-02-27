using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeknikServis.Application.Features.Customers.Commands;

namespace TeknikServis.Web.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;

        public CreateModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public CreateInputModel Input { get; set; } = new();

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

            // Yeni Alanlar
            public string? TaxNumber { get; set; }
            public string? TaxOffice { get; set; }
            public string? Address { get; set; }
            public string? Notes { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Formdan gelen tüm verileri Kuryeye veriyoruz
            var command = new CreateCustomerCommand(
                Input.FirstName,
                Input.LastName,
                Input.Email,
                Input.PhoneNumber,
                Input.TaxNumber,
                Input.TaxOffice,
                Input.Address,
                Input.Notes
            );

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Müþteri baþarýyla eklendi ve Cari Kod atandý.";
                return RedirectToPage("/Customers/Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Bir hata oluþtu.");
            return Page();
        }
    }
}