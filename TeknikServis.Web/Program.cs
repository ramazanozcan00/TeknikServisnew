using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using TeknikServis.Application.Interfaces;
using TeknikServis.Infrastructure.Persistence;
using TeknikServis.Infrastructure.Persistence.Repositories;
using TeknikServis.Application.Features.Customers.Commands; // MediatR'ýn katmaný bulmasý için
using TeknikServis.Infrastructure.Persistence.Identity;

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERÝTABANI BAÐLANTISINI TANITMA ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. IDENTITY AYARLARI ---
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// --- 3. COOKIE (GÝRÝÞ) AYARLARI ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login"; // Giriþ yapýlmamýþsa buraya at
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8 saat baðlý kal
    options.SlidingExpiration = true;
});

// --- 4. BAÐIMLILIK ENJEKSÝYONU (DEPENDENCY INJECTION) ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWorkOrderQueryRepository, WorkOrderQueryRepository>();
builder.Services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>(); // YENÝ EKLENDÝ
// --- 5. MEDIATR TANITMA ---
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly));

// --- 6. WEB ARAYÜZÜ SERVÝSLERÝ ---
builder.Services.AddRazorPages();

var app = builder.Build();

// --- UYGULAMA ÇALIÞMA KURALLARI (MIDDLEWARE) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity için ÞART: önce Authentication, sonra Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// --- ÝLK ADMIN KULLANICISINI VE ROLLERÝ OLUÞTURMA (SEED) ---

// --- ÝLK ADMIN KULLANICISINI VE ROLLERÝ OLUÞTURMA (SEED) ---

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    // 1. Sistemdeki Temel Rolleri Oluþtur
    string[] roller = { "Admin", "Teknisyen", "Sekreter" };
    foreach (var rolAdi in roller)
    {
        if (!await roleManager.RoleExistsAsync(rolAdi))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = rolAdi });
        }
    }

    // 2. Admin kullanýcýsýný bul, yoksa oluþtur
    var adminUser = await userManager.FindByEmailAsync("admin@teknikservis.com");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin@teknikservis.com",
            Email = "admin@teknikservis.com",
            FirstName = "Sistem",
            LastName = "Yöneticisi"
        };
        await userManager.CreateAsync(adminUser, "Admin123!");
    }

    // 3. Kullanýcýya "Admin" rolü atanmamýþsa KESÝNLÝKLE ata
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Uygulamayý baþlat
app.Run();