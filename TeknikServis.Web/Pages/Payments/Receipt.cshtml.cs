using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using TeknikServis.Application.Features.Payments.DTOs;
using TeknikServis.Application.Features.Payments.Queries;

namespace TeknikServis.Web.Pages.Payments
{
    public class ReceiptModel : PageModel
    {
        private readonly IMediator _mediator;
        public ReceiptModel(IMediator mediator) => _mediator = mediator;

        public PaymentDto Payment { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _mediator.Send(new GetPaymentByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
                return RedirectToPage("/Payments/Index");

            Payment = result.Data;
            return Page();
        }
    }
}