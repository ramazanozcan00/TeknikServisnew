using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeknikServis.Application.Features.Customers.Commands;
using TeknikServis.Application.Features.Customers.Queries;

namespace TeknikServis.Web.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;

        public EditModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public EditInputModel Input { get; set; } = new();

        public class EditInputModel
        {
            public Guid Id { get; set; }

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Sayfa açýldýðýnda müþterinin mevcut bilgilerini veritabanýndan çekiyoruz
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
            {
                return RedirectToPage("/Customers/Index");
            }

            var customer = result.Data;

            // Çektiðimiz bilgileri formun içine (Input) dolduruyoruz
            Input = new EditInputModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                TaxNumber = customer.TaxNumber,
                TaxOffice = customer.TaxOffice,
                Address = customer.Address,
                Notes = customer.Notes
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Formdaki güncel verileri alýp Komut (Command) aracýlýðýyla gönderiyoruz
            var command = new UpdateCustomerCommand(
                Input.Id,
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
                TempData["SuccessMessage"] = "Müþteri bilgileri baþarýyla güncellendi.";
                return RedirectToPage("/Customers/Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Bir hata oluþtu.");
            return Page();
        }
    }
}