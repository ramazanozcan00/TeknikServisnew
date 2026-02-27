using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.Customers.DTOs;
using TeknikServis.Application.Features.Customers.Queries;

namespace TeknikServis.Web.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Arayüzde listelenecek müþteriler
        public List<CustomerDto> Customers { get; set; } = new();

        // Arama kutusundan gelen deðeri tutacak özellik (URL'den GET ile gelebilmesi için SupportsGet = true yapýyoruz)
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            // SearchTerm (Arama kelimesi) dolu da olsa boþ da olsa Query'e gönderiyoruz
            var result = await _mediator.Send(new GetCustomersQuery(SearchTerm));

            if (result.IsSuccess && result.Data != null)
            {
                Customers = result.Data;
            }
        }
    }
}