using LibraryAppMVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<BookService, BookService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// TODO: implementation IBookServeces
// TODO: implementation BookServeces
// TODO: implementation LibraryController