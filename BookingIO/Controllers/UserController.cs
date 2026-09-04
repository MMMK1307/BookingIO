using BookingIO.Data;
using BookingIO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

    public IActionResult LoginPage()
    {
        return View("Login");
    }
    public IActionResult RegisterPage()
    {
        return View("Register");
    }

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

    public async Task<IActionResult> Register(RegisterUserModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Login));

        var user = new User();

        await _userManager.CreateAsync(user);

        return RedirectToAction("Index", "Space");
    }
}
