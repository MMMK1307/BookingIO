using BookingIO.Common;
using BookingIO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using UserModel = BookingIO.Models.User;

namespace BookingIO.Controllers;
public class UserController : BaseController
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
        {
            return RedirectToAction("Index", "Home");
        }

        NotifyWarning(UserErrors.LoginFailed);

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

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction("Login", "User");
        }

        foreach(var error in result.Errors)
        {
            NotifyError("Erro Ao Cadastrar", UserErrors.Translate(error.Code));
        }

        NotifySuccess("Login Realizado com Sucesso");

        return View("Register", model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
