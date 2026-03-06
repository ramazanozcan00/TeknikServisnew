using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TeknikServis.Application.Features.SpareParts.Commands;

namespace TeknikServis.Web.Pages.SpareParts
{
    [Authorize(Roles = "Admin,Teknisyen")]
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        public CreateModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Command parametreleri domaindeki yeni alanlara göre güncellendi
            var command = new CreateSparePartCommand(
                Input.Name,
                Input.Barcode,
                Input.SparePartCode,
                Input.PurchasePrice,
                Input.SalePrice,
                Input.CriticalStockLevel,
                Input.PurchaseInvoiceNo,
                Input.SerialNumbers
            );

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Yeni parça ve seri numaralarý baþarýyla kaydedildi.";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Hata oluþtu.");
            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Ürün adý zorunludur.")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Barkod zorunludur.")]
            public string Barcode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Stok kodu zorunludur.")]
            public string SparePartCode { get; set; } = string.Empty;

            public decimal PurchasePrice { get; set; }
            public decimal SalePrice { get; set; }

            [Display(Name = "Kritik Stok Seviyesi")]
            public int CriticalStockLevel { get; set; } = 5;

            [Required(ErrorMessage = "Fatura numarasý zorunludur.")]
            public string PurchaseInvoiceNo { get; set; } = string.Empty;

            public List<string> SerialNumbers { get; set; } = new();
            public string Unit { get; set; } = "Adet";
        }
    }
}