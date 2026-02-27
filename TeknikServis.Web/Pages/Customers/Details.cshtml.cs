using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.Customers.DTOs;
using TeknikServis.Application.Features.Customers.Queries;
using TeknikServis.Application.Features.Devices.Commands;
using TeknikServis.Application.Features.Devices.DTOs;
using TeknikServis.Application.Features.Devices.Queries;
using TeknikServis.Application.Features.WorkOrders.Commands;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Features.WorkOrders.Queries;

namespace TeknikServis.Web.Pages.Customers
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        public DetailsModel(IMediator mediator) => _mediator = mediator;

        public CustomerDto Customer { get; set; } = null!;
        public List<DeviceDto> Devices { get; set; } = new();
        public List<WorkOrderBoardDto> WorkOrders { get; set; } = new();
        [BindProperty]
        public DeviceInputModel Input { get; set; } = new();

        // Sayfa açýldýðýnda Müþteriyi ve Cihazlarýný getirir
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var customerResult = await _mediator.Send(new GetCustomerByIdQuery(id));
            if (!customerResult.IsSuccess || customerResult.Data == null) return RedirectToPage("/Index");
            Customer = customerResult.Data;

            var devicesResult = await _mediator.Send(new GetDevicesByCustomerIdQuery(id));
            if (devicesResult.IsSuccess && devicesResult.Data != null)
            {
                Devices = devicesResult.Data;
            }
            // Müþterinin Ýþ Emirlerini (Geçmiþ ve Aktif) Getir
            var workOrdersResult = await _mediator.Send(new GetWorkOrdersByCustomerIdQuery(id));
            if (workOrdersResult.IsSuccess && workOrdersResult.Data != null)
            {
                WorkOrders = workOrdersResult.Data;
            }
            return Page();
        }

        // Yeni cihaz formundan veri geldiðinde çalýþýr
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid) return await OnGetAsync(id);

            var command = new CreateDeviceCommand(
                id, Input.Brand, Input.Model, Input.SerialNumber, Input.DeviceType, Input.Note);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Yeni cihaz baþarýyla müþteriye eklendi."; // Toast mesajýmýz!
                return RedirectToPage(new { id = id }); // Sayfayý yenile
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return await OnGetAsync(id);
        }
        // Modal üzerinden gelen Ýþ Emri kayýt isteðini yakalar
        public async Task<IActionResult> OnPostCreateWorkOrderAsync(Guid id) // id = CustomerId (URL'den gelir)
        {
            // Arýza açýklamasý boþsa kaydetme
            if (string.IsNullOrWhiteSpace(WorkOrderInput.Description))
            {
                TempData["ErrorMessage"] = "Arýza açýklamasý boþ býrakýlamaz.";
                return RedirectToPage(new { id = id });
            }

            var command = new CreateWorkOrderCommand(WorkOrderInput.DeviceId, WorkOrderInput.Description);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Ýþ emri açýldý. Lütfen fiþi yazdýrýn.";
                // Müþteri sayfasýna dönmek yerine, fiþ yazdýrma sayfasýna yönlendir!
                return RedirectToPage("/WorkOrders/Receipt", new { id = result.Data });
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage; // Eðer özel bir hata Toast'ý yapmak istersen
            }

            return RedirectToPage(new { id = id }); // Sayfayý yenile
        }

        public class DeviceInputModel
        {
            public string Brand { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string SerialNumber { get; set; } = string.Empty;
            public string DeviceType { get; set; } = string.Empty;
            public string? Note { get; set; }
        }

        // Ýþ Emri formu için verileri taþýyacak model
        [BindProperty]
        public WorkOrderInputModel WorkOrderInput { get; set; } = new();

        public class WorkOrderInputModel
        {
            public Guid DeviceId { get; set; }
            public string Description { get; set; } = string.Empty;
        }
    }
}