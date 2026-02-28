using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // HATA BURADAYDI: WorkOrderBoardDto yerine WorkOrderDetailDto yapýldý
        public List<WorkOrderDetailDto> WorkOrders { get; set; } = new();

        [BindProperty]
        public DeviceInputModel Input { get; set; } = new();

        [BindProperty]
        public WorkOrderInputModel WorkOrderInput { get; set; } = new();

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

            var workOrdersResult = await _mediator.Send(new GetWorkOrdersByCustomerIdQuery(id));
            if (workOrdersResult.IsSuccess && workOrdersResult.Data != null)
            {
                WorkOrders = workOrdersResult.Data;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid) return await OnGetAsync(id);

            var command = new CreateDeviceCommand(
                id, Input.Brand, Input.Model, Input.SerialNumber, Input.DeviceType, Input.Note);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Yeni cihaz baþarýyla müþteriye eklendi.";
                return RedirectToPage(new { id = id });
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostCreateWorkOrderAsync(Guid id)
        {
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
                return RedirectToPage("/WorkOrders/Receipt", new { id = result.Data });
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToPage(new { id = id });
        }

        public class DeviceInputModel
        {
            public string Brand { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string SerialNumber { get; set; } = string.Empty;
            public string DeviceType { get; set; } = string.Empty;
            public string? Note { get; set; }
        }

        public class WorkOrderInputModel
        {
            public Guid DeviceId { get; set; }
            public string Description { get; set; } = string.Empty;
        }
    }
}