using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

            [Required(ErrorMessage = "Ad alaný zorunludur.")] public string FirstName { get; set; } = string.Empty;
            [Required(ErrorMessage = "Soyad alaný zorunludur.")] public string LastName { get; set; } = string.Empty;
            [Required(ErrorMessage = "E-Posta zorunludur.")][EmailAddress] public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Telefon numarasý zorunludur.")] public string PhoneNumber { get; set; } = string.Empty;

            public string? TaxNumber { get; set; }
            public string? TaxOffice { get; set; }

            // YENÝ EKLENEN ÝL / ÝLÇE ALANLARI
            [Required(ErrorMessage = "Ýl seçimi zorunludur.")] public string City { get; set; } = string.Empty;
            [Required(ErrorMessage = "Ýlçe seçimi zorunludur.")] public string District { get; set; } = string.Empty;

            public string? Address { get; set; }
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = "Müþteri bulunamadý.";
                return RedirectToPage("./Index");
            }

            // Veritabanýndaki mevcut bilgileri forma dolduruyoruz
            Input = new EditInputModel
            {
                Id = result.Data.Id,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                PhoneNumber = result.Data.PhoneNumber,
                TaxNumber = result.Data.TaxNumber,
                TaxOffice = result.Data.TaxOffice,
                City = result.Data.City ?? "",        // EKLENDÝ
                District = result.Data.District ?? "",// EKLENDÝ
                Address = result.Data.Address,
                Notes = result.Data.Notes
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // BURASI DÜZELTÝLDÝ: Kuryeye tüm bilgileri sýrasýyla (City ve District dahil) veriyoruz!
            var command = new UpdateCustomerCommand(
                Input.Id,
                Input.FirstName,
                Input.LastName,
                Input.Email,
                Input.PhoneNumber,
                Input.TaxNumber,
                Input.TaxOffice,
                Input.City,       // EKLENDÝ
                Input.District,   // EKLENDÝ
                Input.Address,
                Input.Notes
            );

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Müþteri bilgileri baþarýyla güncellendi.";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Hata oluþtu.");
            return Page();
        }
    }
}