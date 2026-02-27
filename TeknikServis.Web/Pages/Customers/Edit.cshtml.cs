using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.Customers.Commands;
using TeknikServis.Application.Features.Customers.Queries;

namespace TeknikServis.Web.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;
        public EditModel(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public EditCustomerInputModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            if (!result.IsSuccess || result.Data == null) return RedirectToPage("/Index");

            Input = new EditCustomerInputModel
            {
                Id = result.Data.Id,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                PhoneNumber = result.Data.PhoneNumber
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _mediator.Send(new UpdateCustomerCommand(
                Input.Id, Input.FirstName, Input.LastName, Input.Email, Input.PhoneNumber));

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Müþteri bilgileri baþarýyla güncellendi."; // YENÝ EKLENEN
                return RedirectToPage("/Customers/Index");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return Page();
        }

        public class EditCustomerInputModel
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
        }
    }
}