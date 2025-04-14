using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollabWithEntraId_POC.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CollabWithEntraId_POC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public List<Claim> UserClaims => User.Claims.ToList();

    public IActionResult Index()
    {
        return View(UserClaims);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    public IActionResult SignIn(string provider = "AzureAd")
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = "/" },
            provider
        );
    }

    public new IActionResult SignOut()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, "AzureAd", "AzureAdB2C", "Cookies");
    }
}
