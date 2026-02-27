using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeknikServis.Application.Features.Payments.DTOs;
using TeknikServis.Application.Features.Payments.Queries;

namespace TeknikServis.Web.Pages.Payments
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator) => _mediator = mediator;

        public List<PaymentDto> Payments { get; set; } = new();
        public decimal TotalCash { get; set; } // Toplam Nakit
        public decimal TotalCreditCard { get; set; } // Toplam Kredi Kartý
        public decimal TotalRevenue { get; set; } // Genel Toplam

        public async Task OnGetAsync()
        {
            var result = await _mediator.Send(new GetPaymentsQuery());
            if (result.IsSuccess && result.Data != null)
            {
                Payments = result.Data;

                // Kasa Ýstatistiklerini Hesaplýyoruz
                TotalCash = Payments.Where(p => p.Method == Domain.Enums.PaymentMethod.Nakit).Sum(p => p.Amount);
                TotalCreditCard = Payments.Where(p => p.Method == Domain.Enums.PaymentMethod.KrediKarti).Sum(p => p.Amount);
                TotalRevenue = TotalCash + TotalCreditCard + Payments.Where(p => p.Method == Domain.Enums.PaymentMethod.HavaleEft).Sum(p => p.Amount);
            }
        }
    }
}