using Microsoft.EntityFrameworkCore;
using OnlineLibrary.BusinessLayer.Service;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<OnlineLibraryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Library"));
});
//Dependency Injection
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IAuthorService, AuthorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();
