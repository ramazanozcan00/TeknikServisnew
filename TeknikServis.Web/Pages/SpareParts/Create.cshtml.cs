using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TeknikServis.Application.Features.SpareParts.Commands;

namespace TeknikServis.Web.Pages.SpareParts
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        public CreateModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Parça Adý zorunludur.")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Stok Kodu/Barkod zorunludur.")]
            public string Code { get; set; } = string.Empty;

            public decimal PurchasePrice { get; set; }
            public decimal SalePrice { get; set; }
            public int StockQuantity { get; set; }
            public int CriticalStockLevel { get; set; } = 5;
            public string Unit { get; set; } = "Adet";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var command = new CreateSparePartCommand(
                Input.Name, Input.Code, Input.PurchasePrice, Input.SalePrice,
                Input.StockQuantity, Input.CriticalStockLevel, Input.Unit);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Yedek parça baþarýyla stoka eklendi.";
                return RedirectToPage("/SpareParts/Index");
            }

            ModelState.AddModelError("", result.ErrorMessage ?? "Bir hata oluþtu.");
            return Page();
        }
    }
}