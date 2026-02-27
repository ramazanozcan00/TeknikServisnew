using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeknikServis.Application.Features.SpareParts.DTOs;
using TeknikServis.Application.Features.SpareParts.Queries;

namespace TeknikServis.Web.Pages.SpareParts
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator) => _mediator = mediator;

        public List<SparePartDto> SpareParts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var result = await _mediator.Send(new GetSparePartsQuery(SearchTerm));
            if (result.IsSuccess && result.Data != null) SpareParts = result.Data;
        }
    }
}