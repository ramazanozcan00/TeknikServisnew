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

        [BindProperty] public InputModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _mediator.Send(new CreateSparePartCommand(Input.Name, Input.Code, Input.PurchasePrice, Input.SalePrice, Input.InitialStock, Input.CriticalLevel, Input.Unit));
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Yeni parça stoklara baþarýyla eklendi.";
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Hata oluþtu.");
            return Page();
        }

        public class InputModel
        {
            [Required] public string Name { get; set; } = string.Empty;
            [Required] public string Code { get; set; } = string.Empty;
            public decimal PurchasePrice { get; set; }
            public decimal SalePrice { get; set; }
            public int InitialStock { get; set; } = 0;
            public int CriticalLevel { get; set; } = 5;
            public string Unit { get; set; } = "Adet";
        }
    }
}