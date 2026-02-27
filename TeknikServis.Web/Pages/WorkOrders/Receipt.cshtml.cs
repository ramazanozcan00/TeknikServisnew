using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeknikServis.Infrastructure.Persistence;

namespace TeknikServis.Web.Pages.WorkOrders
{
    public class ReceiptModel : PageModel
    {
        private readonly AppDbContext _context;
        public ReceiptModel(AppDbContext context) => _context = context;

        public dynamic ReceiptData { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // LINQ ile yazdýrma için gerekli veriyi hýzlýca topluyoruz
            var data = await (from wo in _context.WorkOrders
                              join d in _context.Devices on wo.DeviceId equals d.Id
                              join c in _context.Customers on d.CustomerId equals c.Id
                              where wo.Id == id
                              select new
                              {
                                  wo.WorkOrderNo,
                                  wo.CreatedAt,
                                  wo.Description,
                                  CustomerName = c.FirstName + " " + c.LastName,
                                  c.PhoneNumber,
                                  DeviceInfo = d.Brand + " " + d.Model,
                                  d.SerialNumber
                              }).FirstOrDefaultAsync();

            if (data == null) return NotFound();

            ReceiptData = data;
            return Page();
        }
    }
}