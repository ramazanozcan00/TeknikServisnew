using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TeknikServis.Application.Features.Notifications.Commands;

namespace TeknikServis.Web.Pages.Notifications
{
    [Authorize(Roles = "Admin")] // SADECE ADMÝN YAYINLAYABÝLÝR
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        public CreateModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _mediator.Send(new CreateNotificationCommand(Input.Title, Input.Message, Input.Color));

            TempData["SuccessMessage"] = "Duyuru tüm personele baþarýyla yayýnlandý!";
            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Color { get; set; } = "bg-primary";
        }
    }
}