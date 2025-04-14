using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Home/SignIn";
})
.AddOpenIdConnect("AzureAd", options =>
{
    options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}";
    options.ClientId = builder.Configuration["AzureAd:ClientId"]!;
    options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"]!;
    options.CallbackPath = builder.Configuration["AzureAd:CallbackPath"] ?? "/signin-oidc";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
})
.AddOpenIdConnect("AzureAdB2C", options =>
{
    var tenant = builder.Configuration["AzureAdB2C:TenantName"];
    var policy = builder.Configuration["AzureAdB2C:SignUpSignInPolicyId"];
    options.Authority = $"https://{tenant}.b2clogin.com/{tenant}.onmicrosoft.com/{policy}/v2.0/";
    // options.Authority = $"https://{tenant}.b2clogin.com/{tenant}.onmicrosoft.com/tfp/{tenant}.onmicrosoft.com/{policy}/v2.0/";
    options.ClientId = builder.Configuration["AzureAdB2C:ClientId"];
    options.ClientSecret = builder.Configuration["AzureAdB2C:ClientSecret"];
    options.CallbackPath = builder.Configuration["AzureAdB2C:CallbackPath"] ?? "/signin-oidc";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
});

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

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
app.MapRazorPages();

app.Run();
