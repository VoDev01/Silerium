using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Silerium.Controllers;
using Silerium.Data;
using Silerium;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")
    ));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JWTAuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = JWTAuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = JWTAuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    })
    .AddCookie(options => { options.LoginPath = "/User/Login"; });

builder.Services.AddLogging(logger => logger.AddConsole());
builder.Services.AddSingleton<Logger<AdminController>>();
builder.Services.AddSingleton<Logger<UserController>>();
builder.Services.AddSingleton<Logger<CatalogController>>();

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

app.UseAuthentication();
//app.UseStatusCodePages(async context =>
//{
//    var response = context.HttpContext.Response;
//    var request = context.HttpContext.Request;

//    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
//        response.StatusCode == (int)HttpStatusCode.Forbidden)
//        response.Redirect("/User/Login?ReturnUrl=/User/Profile");
//    else
//        response.Redirect(request.Path.Value);
//});
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
