using Microsoft.EntityFrameworkCore;
using Silerium.Controllers;
using Silerium.Data;
using Silerium.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")
    ));

builder.Services.AddLogging(logger => logger.AddConsole());
builder.Services.AddSingleton<Logger<AdminController>>();

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

app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/Catalog/Products/", StringComparison.OrdinalIgnoreCase), appbuilder =>
{
    app.UseProductsSortingMiddleware(builder.Configuration.GetConnectionString("Default"));
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
