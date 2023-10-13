using Contacts.Data;
using Contacts.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Contacts.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false; // Set this to false to allow non-unique email addresses
    options.Password.RequireDigit = false;           // Do not require a digit
    options.Password.RequireLowercase = false;       // Do not require a lowercase letter
    options.Password.RequireUppercase = false;       // Do not require an uppercase letter
    options.Password.RequireNonAlphanumeric = false; // Do not require a special character
    options.Password.RequiredLength = 1;
}).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews().AddRazorPagesOptions(options =>
{
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
});
builder.Services.AddRazorPages();




builder.Services.AddSignalR();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}





app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// SignalR hub mapping
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ReloadContactGrid>("/ReloadContactGrid"); // Map your SignalR hub

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Contacts}/{action=Index}/{id?}");

    endpoints.MapRazorPages();

});





app.Run();
