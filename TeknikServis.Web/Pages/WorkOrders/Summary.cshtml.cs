using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeknikServis.Application.Features.Payments.Commands;
using TeknikServis.Application.Features.Payments.Queries;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Features.WorkOrders.Queries;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Web.Pages.WorkOrders
{
    public class SummaryModel : PageModel
    {
        private readonly IMediator _mediator;
        public SummaryModel(IMediator mediator) => _mediator = mediator;

        public WorkOrderDetailDto Detail { get; set; } = null!;

        // YENÝ EKLENEN: Ödeme yapýldý mý bilgisini tutan deðiþken
        public bool IsPaid { get; set; } = false;

        [BindProperty]
        public PaymentInputModel PaymentInput { get; set; } = new();

        public class PaymentInputModel
        {
            public Guid WorkOrderId { get; set; }
            public Guid CustomerId { get; set; }
            [Required(ErrorMessage = "Tutar girilmesi zorunludur.")]
            public decimal Amount { get; set; }
            public PaymentMethod Method { get; set; } = PaymentMethod.Nakit;
            public string Description { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _mediator.Send(new GetWorkOrderDetailQuery(id));
            if (!result.IsSuccess || result.Data == null)
                return RedirectToPage("/Customers/Index");

            Detail = result.Data;

            // YENÝ EKLENEN: Kasada bu iþ emrinin ödemesi var mý diye kontrol et
            var paymentCheckResult = await _mediator.Send(new CheckWorkOrderPaymentQuery(Detail.Id));
            if (paymentCheckResult.IsSuccess)
            {
                IsPaid = paymentCheckResult.Data;
            }

            PaymentInput = new PaymentInputModel
            {
                WorkOrderId = Detail.Id,
                CustomerId = Detail.CustomerId,
                Amount = Detail.TotalPrice ?? 0,
                Description = $"{Detail.WorkOrderNo} numaralý servis hizmeti tahsilatý."
            };

            return Page();
        }

        public async Task<IActionResult> OnPostReceivePaymentAsync()
        {
            var command = new ReceivePaymentCommand(PaymentInput.CustomerId, PaymentInput.WorkOrderId, PaymentInput.Amount, PaymentInput.Method, PaymentInput.Description);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Tahsilat baþarýyla alýndý!";
                // Özet sayfasýna dönmek yerine direkt FÝÞ YAZDIRMA SAYFASINA yönlendir!
                return RedirectToPage("/Payments/Receipt", new { id = result.Data });
            }
            else TempData["ErrorMessage"] = result.ErrorMessage ?? "Tahsilat alýnýrken bir hata oluþtu.";

            return RedirectToPage(new { id = PaymentInput.WorkOrderId });
        }
    }
}