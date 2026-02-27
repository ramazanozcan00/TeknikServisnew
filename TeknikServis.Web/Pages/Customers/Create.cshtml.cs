using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeknikServis.Application.Features.Customers.Commands;

namespace TeknikServis.Web.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;

        // Dependency Injection ile MediatR'ý çaðýrýyoruz
        public CreateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        // HTML formundan gelecek verileri bu modele baðlayacaðýz ([BindProperty] bunu saðlar)
        [BindProperty]
        public CustomerInputModel Input { get; set; } = new();

        // Sayfa ilk açýldýðýnda çalýþýr (Boþ form gösterir)
        public void OnGet()
        {
        }

        // Formdaki "Kaydet" butonuna basýldýðýnda çalýþýr
        public async Task<IActionResult> OnPostAsync()
        {
            // Eðer form kurallarýmýza uymazsa (örn: boþ isim), ayný sayfayý hatalarla geri döndür
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Arayüzden gelen verileri, Application katmanýndaki Command paketimize yüklüyoruz
            var command = new CreateCustomerCommand(
                Input.FirstName,
                Input.LastName,
                Input.Email,
                Input.PhoneNumber,
                Input.TaxNumber);

            // MediatR kuryemizi yola çýkarýyoruz!
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Müþteri baþarýyla sisteme kaydedildi."; // YENÝ EKLENEN
                return RedirectToPage("/Customers/Index");
            }

            // Eðer veritabaný kayýt aþamasýnda özel bir hata dönerse, ekranda göster
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return Page();
        }

        // Sadece formda veri taþýmak için kullandýðýmýz basit bir sýnýf
        public class CustomerInputModel
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string TaxNumber { get; set; } = string.Empty;
        }
    }
}