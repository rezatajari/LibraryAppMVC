using LibraryAppMVC.Data;
using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Repositories;
using LibraryAppMVC.Services;
using LibraryAppMVC.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryDB>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDB")));

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
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();