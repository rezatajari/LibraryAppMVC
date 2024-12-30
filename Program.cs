using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Repositories;
using LibraryAppMVC.Services;
using LibraryAppMVC.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using YourProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryDB>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDB")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.SignIn.RequireConfirmedEmail = true; // Enable email confirmation
    options.Password.RequiredLength = 4; // Set password policies as needed
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<LibraryDB>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailSender, EmailSender>();

// Configure Serilog
Log.Logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
 .MinimumLevel.Override("System", LogEventLevel.Warning)
 .MinimumLevel.Information()
 .WriteTo.Console()
 .WriteTo.MSSqlServer(
    connectionString: builder.Configuration.GetConnectionString("LibraryDB"),
    sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
   .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BookValidator>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();