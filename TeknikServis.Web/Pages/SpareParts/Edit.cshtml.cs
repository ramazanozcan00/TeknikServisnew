using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TeknikServis.Application.Features.SpareParts.Commands;
using TeknikServis.Application.Features.SpareParts.Queries;

namespace TeknikServis.Web.Pages.SpareParts
{
    [Authorize(Roles = "Admin,Teknisyen")]
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;
        public EditModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _mediator.Send(new GetSparePartByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = "Güncellenecek parça bulunamadý.";
                return RedirectToPage("./Index");
            }

            // DTO'dan gelen tüm verileri forma basýyoruz
            Input = new InputModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Code = result.Data.Code,
                PurchasePrice = result.Data.PurchasePrice,
                SalePrice = result.Data.SalePrice,
                CriticalLevel = result.Data.CriticalStockLevel, // Mapster ile gelen güncel alan
                Unit = result.Data.Unit
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _mediator.Send(new UpdateSparePartCommand(
                Input.Id, Input.Name, Input.Code, Input.PurchasePrice, Input.SalePrice, Input.CriticalLevel, Input.Unit));

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Parça / Stok bilgileri baþarýyla güncellendi.";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Güncelleme sýrasýnda hata oluþtu.");
            return Page();
        }

        public class InputModel
        {
            public Guid Id { get; set; }

            [Required(ErrorMessage = "Parça Adý zorunludur.")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Stok Kodu zorunludur.")]
            public string Code { get; set; } = string.Empty;

            public decimal PurchasePrice { get; set; }

            [Required(ErrorMessage = "Satýþ fiyatý zorunludur.")]
            public decimal SalePrice { get; set; }

            public int CriticalLevel { get; set; }

            public string Unit { get; set; } = "Adet";
        }
    }
}