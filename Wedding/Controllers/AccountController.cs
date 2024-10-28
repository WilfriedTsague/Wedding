using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[AllowAnonymous]// Permet aux utilisateurs anonymes d'accéder aux actions suivantes
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(string username, string password)
    {
        var user = new IdentityUser { UserName = username, Email = username };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Redirige l'utilisateur après la création réussie
            return RedirectToAction("Index", "Home");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(password, error.Description);
            }
        }

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Recherche de l'utilisateur par nom d'utilisateur
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe incorrect.");
            return View();
        }

        // Tentative de connexion
        var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Rediriger vers la page d'accueil après une connexion réussie
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe incorrect.");
            return View();
        }
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
