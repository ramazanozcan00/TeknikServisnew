using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Features.WorkOrders.Queries;

namespace TeknikServis.Web.Pages.WorkOrders
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator) => _mediator = mediator;

        public List<WorkOrderBoardDto> WorkOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var result = await _mediator.Send(new GetActiveWorkOrdersQuery());
            if (result.IsSuccess && result.Data != null)
            {
                WorkOrders = result.Data;
            }
        }
        // OnGetAsync metodunun hemen altýna ekle
        public async Task<IActionResult> OnPostChangeStatusAsync(Guid id, TeknikServis.Domain.Enums.WorkOrderStatus status)
        {
            var command = new TeknikServis.Application.Features.WorkOrders.Commands.UpdateWorkOrderStatusCommand(id, status);
            await _mediator.Send(command);
            return RedirectToPage(); // Panoyu yenile
        }
    }
}