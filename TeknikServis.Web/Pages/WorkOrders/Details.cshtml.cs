using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeknikServis.Application.Features.SpareParts.Queries;
using TeknikServis.Application.Features.WorkOrders.Commands;
using TeknikServis.Application.Features.WorkOrders.DTOs;
using TeknikServis.Application.Features.WorkOrders.Queries;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Web.Pages.WorkOrders
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        public DetailsModel(IMediator mediator) => _mediator = mediator;

        public WorkOrderDetailDto Detail { get; set; } = null!;

        // Ana form (Durum, Fiyat, Not güncelleme) için Input
        [BindProperty]
        public InputModel Input { get; set; } = new();

        // Yedek parça ekleme formu için Input
        [BindProperty]
        public AddPartInputModel PartInput { get; set; } = new();

        // Depodaki ürünleri tutacaðýmýz liste (Açýlýr menü için)
        public List<SelectListItem> AvailableSpareParts { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // 1. Ýþ Emri Detaylarýný Getir
            var result = await _mediator.Send(new GetWorkOrderDetailQuery(id));
            if (!result.IsSuccess || result.Data == null) return RedirectToPage("/WorkOrders/Index");

            Detail = result.Data;

            // 2. Formu mevcut verilerle doldur
            Input = new InputModel
            {
                Id = Detail.Id,
                Status = Detail.Status,
                TechnicianNotes = Detail.TechnicianNotes,
                TotalPrice = Detail.TotalPrice
            };

            // Parça ekleme formu için ID'yi ayarla
            PartInput.WorkOrderId = Detail.Id;

            // 3. Depodaki Yedek Parçalarý Getir (Sadece stoðu 0'dan büyük olanlar)
            var partsResult = await _mediator.Send(new GetSparePartsQuery());
            if (partsResult.IsSuccess && partsResult.Data != null)
            {
                AvailableSpareParts = partsResult.Data
                    .Where(p => p.StockQuantity > 0)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.Name} (Stok: {p.StockQuantity} | Fiyat: {p.SalePrice:C2})"
                    }).ToList();
            }

            return Page();
        }

        // --- ANA FORM KAYDETME (DURUM / NOT / FÝYAT) ---
        public async Task<IActionResult> OnPostAsync()
        {
            var command = new UpdateWorkOrderDetailsCommand(Input.Id, Input.Status, Input.TechnicianNotes, Input.TotalPrice);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Ýþ emri detaylarý baþarýyla kaydedildi.";
                return RedirectToPage(new { id = Input.Id });
            }

            TempData["ErrorMessage"] = "Bir hata oluþtu.";
            return await OnGetAsync(Input.Id);
        }

        // --- YENÝ EKLENEN: YEDEK PARÇA EKLEME BUTONU ---
        public async Task<IActionResult> OnPostAddPartAsync()
        {
            var command = new AddSparePartToWorkOrderCommand(PartInput.WorkOrderId, PartInput.SparePartId, PartInput.Quantity);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Parça baþarýyla eklendi, stoktan düþüldü ve fiyata yansýtýldý!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Yedek parça eklenirken hata oluþtu.";
            }

            // Sayfayý yenile ki yeni tutar ve teknisyen notlarý ekrana gelsin
            return RedirectToPage(new { id = PartInput.WorkOrderId });
        }

        public class InputModel
        {
            public Guid Id { get; set; }
            public WorkOrderStatus Status { get; set; }
            public string? TechnicianNotes { get; set; }
            public decimal? TotalPrice { get; set; }
        }

        public class AddPartInputModel
        {
            public Guid WorkOrderId { get; set; }
            public Guid SparePartId { get; set; }
            public int Quantity { get; set; } = 1;
        }
    }
}