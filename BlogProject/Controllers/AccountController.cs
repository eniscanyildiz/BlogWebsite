using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Register GET
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Register POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string userName, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            password != confirmPassword)
        {
            ModelState.AddModelError("", "Lütfen tüm alanları doğru ve eksiksiz doldurun.");
            return View();
        }

        var user = new ApplicationUser { UserName = userName, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // İlk kullanıcı ise admin rolü atanabilir (opsiyonel)
            if (!_userManager.Users.Any())
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Posts");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }

    // Login GET
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Login POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userName, string password, bool rememberMe)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Kullanıcı adı ve şifre zorunludur.");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Posts");
        }

        ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
        return View();
    }

    // Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Posts");
    }
}
