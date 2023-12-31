using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Silerium.Controllers;
using Silerium.Data;
using Silerium;
using Silerium.Middlewares;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authorization;
using Silerium.PermissionAuth;
using Microsoft.AspNetCore.Mvc.Razor;
using Silerium.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Add(new Silerium.ViewLocationsExpanders.AdminLocationExpander());
    options.ViewLocationExpanders.Add(new Silerium.ViewLocationsExpanders.UserLocationExpander());
});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
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
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.Cookie = new CookieBuilder
        {
            Name = "UserAuthCookie"
        };
        options.Cookie.MaxAge = TimeSpan.FromDays(7);
    });

builder.Services.AddLogging(logger => logger.AddConsole());
builder.Services.AddSingleton<Logger<AdminController>>();
builder.Services.AddSingleton<Logger<UserController>>();
builder.Services.AddSingleton<Logger<CatalogController>>();

var app = builder.Build();

app.UseSession();
app.UseForwardedHeaders(options: new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseJWTAuthorizationMiddleware();

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
app.UseAuthorization();

//app.UseHandleDuplicateRequestsMiddleware();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpLogging();

app.Run();
