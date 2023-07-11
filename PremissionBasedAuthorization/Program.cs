using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PremissionBasedAuthorization.Data;
using PremissionBasedAuthorization.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//Code Seeding Role In DataBase  
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var LoggerFactory=services.GetRequiredService<ILoggerFactory>();
var Logger = LoggerFactory.CreateLogger("app");
try
{
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await PremissionBasedAuthorization.Seeds.DefaultRoles.seedAsync(roleManager);
    await PremissionBasedAuthorization.Seeds.DefaultUsers.SeedBasicUserAsync(userManager);
    await PremissionBasedAuthorization.Seeds.DefaultUsers.SeedSuperAdminUserAsync(userManager , roleManager);
    Logger.LogInformation("Data Seeded");
    Logger.LogInformation("Application Started");

}
catch (Exception ex)
{
    Logger.LogWarning(ex, "An error occurred while seeding data");
}

app.Run();
