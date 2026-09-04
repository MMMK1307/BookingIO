using BookingIO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers;
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
            lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        if (result.IsLockedOut)
            return View("Lockout");

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }
}
