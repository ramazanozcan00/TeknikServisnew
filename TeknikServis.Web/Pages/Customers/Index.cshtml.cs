using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.Customers.Commands;
using TeknikServis.Application.Features.Customers.DTOs;
using TeknikServis.Application.Features.Customers.Queries;

namespace TeknikServis.Web.Pages.Customers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Arayüzde göstereceðimiz liste
        public List<CustomerDto> Customers { get; set; } = new();

        // Sayfa her açýldýðýnda verileri çeker
        public async Task OnGetAsync()
        {
            var query = new GetCustomersQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess && result.Data != null)
            {
                Customers = result.Data;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var command = new DeleteCustomerCommand(id);
            await _mediator.Send(command);

            TempData["SuccessMessage"] = "Müþteri sistemden silindi."; // YENÝ EKLENEN
            return RedirectToPage();
        }
    }
}