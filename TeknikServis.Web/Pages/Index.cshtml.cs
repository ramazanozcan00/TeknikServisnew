using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.Dashboard.DTOs;
using TeknikServis.Application.Features.Dashboard.Queries;

namespace TeknikServis.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator) => _mediator = mediator;

        public DashboardStatsDto Stats { get; set; } = new(0, 0, 0, 0);

        public async Task OnGetAsync()
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            if (result.IsSuccess && result.Data != null)
            {
                Stats = result.Data;
            }
        }
    }
}