using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeknikServis.Application.Features.Payments.Commands;
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

        // Tahsilat Formu Ýçin Model
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

            // Form açýldýðýnda Tutar ve Açýklama kýsýmlarýný otomatik dolduruyoruz
            PaymentInput = new PaymentInputModel
            {
                WorkOrderId = Detail.Id,
                CustomerId = Detail.CustomerId,
                Amount = Detail.TotalPrice ?? 0, // Eðer fiyat girilmemiþse 0 gelir
                Description = $"{Detail.WorkOrderNo} numaralý servis hizmeti tahsilatý."
            };

            return Page();
        }

        // Tahsilat Al Butonuna Basýldýðýnda Çalýþacak Metot
        public async Task<IActionResult> OnPostReceivePaymentAsync()
        {
            var command = new ReceivePaymentCommand(
                PaymentInput.CustomerId,
                PaymentInput.WorkOrderId,
                PaymentInput.Amount,
                PaymentInput.Method,
                PaymentInput.Description
            );

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Tahsilat baþarýyla alýndý ve kasaya iþlendi!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Tahsilat alýnýrken bir hata oluþtu.";
            }

            // Sayfayý mesajla birlikte yenile
            return RedirectToPage(new { id = PaymentInput.WorkOrderId });
        }
    }
}