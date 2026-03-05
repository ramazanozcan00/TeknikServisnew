using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using TeknikServis.Application.Interfaces;
using TeknikServis.Infrastructure.Persistence;
using TeknikServis.Infrastructure.Persistence.Repositories;
using TeknikServis.Application.Features.Customers.Commands;
using TeknikServis.Infrastructure.Persistence.Identity;
using TeknikServis.Application.Common.Models;
using TeknikServis.Infrastructure.Services;

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
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// --- 4. BAÐIMLILIK ENJEKSÝYONU (DEPENDENCY INJECTION) ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWorkOrderQueryRepository, WorkOrderQueryRepository>();
builder.Services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
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

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// --- ÝLK ADMIN KULLANICISINI VE ROLLERÝ OLUÞTURMA (SEED) ---
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    // 1. Temel Rolleri Oluþtur
    string[] roller = { "Admin", "Teknisyen", "Sekreter" };
    foreach (var rolAdi in roller)
    {
        if (!await roleManager.RoleExistsAsync(rolAdi))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = rolAdi });
        }
    }

    // 2. DÜÐÜMÜ ÇÖZEN KISIM: Eski yetkisiz admin hesabýný tamamen siliyoruz!
    var oldAdmin = await userManager.FindByEmailAsync("admin@teknikservis.com");
    if (oldAdmin != null)
    {
        await userManager.DeleteAsync(oldAdmin); // Eski hesabý acýmadan sil
    }

    // 3. Yepyeni ve Tam Yetkili Admin'i yaratýyoruz
    var newAdmin = new ApplicationUser
    {
        UserName = "admin@teknikservis.com",
        Email = "admin@teknikservis.com",
        FirstName = "Sistem",
        LastName = "Yöneticisi"
    };

    var createResult = await userManager.CreateAsync(newAdmin, "Admin123!");

    // 4. Yeni admine zorla "Admin" rolünü atýyoruz
    if (createResult.Succeeded)
    {
        await userManager.AddToRoleAsync(newAdmin, "Admin");
    }
}

app.Run();