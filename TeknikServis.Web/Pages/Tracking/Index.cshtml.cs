using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TeknikServis.Application.Features.WorkOrders.Queries;

namespace TeknikServis.Web.Pages.Tracking
{
    [AllowAnonymous] // ÇOK ÖNEMLÝ: Bu sayede sisteme giriþ yapmayan müþteriler sayfayý görebilir!
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator) => _mediator = mediator;

        [BindProperty(SupportsGet = true)]
        public string? WorkOrderNo { get; set; }

        public WorkOrderTrackingDto? TrackingResult { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Eðer URL'den veya formdan bir takip numarasý geldiyse sorgula
            if (!string.IsNullOrWhiteSpace(WorkOrderNo))
            {
                var result = await _mediator.Send(new GetWorkOrderTrackingQuery(WorkOrderNo.Trim()));

                if (result.IsSuccess && result.Data != null)
                {
                    TrackingResult = result.Data;
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Kayýt bulunamadý.";
                }
            }
            return Page();
        }
    }
}