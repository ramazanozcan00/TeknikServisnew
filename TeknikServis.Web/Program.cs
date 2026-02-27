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

// --- ÝLK ADMIN KULLANICISINI OLUÞTURMA (SEED) ---
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var adminEmail = "admin@teknikservis.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "123456");
    }
}

// Uygulamayý baþlat
app.Run();