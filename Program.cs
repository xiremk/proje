using IdentityServerProject;
using IdentityServerProject.Data;
using IdentityServerProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MyExcelUploader.Services; // Doðru namespace'i buraya yazýn
using OfficeOpenXml;  // EPPlus kütüphanesi için gerekli using direktifi


var builder = WebApplication.CreateBuilder(args);


// EPPlus lisansýný kabul et
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Eðer ticari amaçla kullanýyorsanýz, LicenseContext.Commercial olarak ayarlayýn.



// Add services to the container.
builder.Services.AddControllersWithViews();


//
builder.Services.AddScoped<ExcelService>();

builder.Services.AddTransient<ExcelService>();

//





builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryPersistedGrants()
    .AddInMemoryClients(IdentityServerConfig.GetClients())
    .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
    .AddAspNetIdentity<ApplicationUser>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:7249";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

//

// CloudAMQP URL'nizi burada kullanýn
string cloudAMQPUrl = "amqps://ocsqrzxi:roPyk0zLnDBX5sGsNWtPoOx9JkARUkY6@cougar.rmq.cloudamqp.com/ocsqrzxi";

// RabbitMQService ve RabbitMQConsumer'ý ekle
builder.Services.AddHostedService<RabbitMQConsumer>();

builder.Services.AddSingleton(new  RabbitMQService(cloudAMQPUrl));








//

static async Task SeedData(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Rolleri oluþtur
        string[] roleNames = { "admin", "manager", "user" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin kullanýcýsýný oluþtur
        var adminUser = new ApplicationUser
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            FirstName = "Admin",
            LastName = "User"
        };

        string adminPassword = "Admin@123";

        var user = await userManager.FindByEmailAsync("admin@admin.com");

        if (user == null)
        {
            var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
            if (createAdminUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }
    }
}

var app = builder.Build();

// Seed data metodunu program.cs içine ekleyelim
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
