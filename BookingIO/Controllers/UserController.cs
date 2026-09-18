using BookingIO.Data;
using BookingIO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using UserModel = BookingIO.Models.User;

namespace BookingIO.Controllers;
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _dbContext;

    public UserController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var model = new LoginModel();
        return View("Login", model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Username,
            model.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        if (result.IsLockedOut)
            return View("Lockout");

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View("Register");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Register));

        var user = UserModel.Create(model);

        await _userManager.CreateAsync(user, model.Password);

        return RedirectToAction("Index", "Space");
    }
}
