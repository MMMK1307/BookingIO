using BookingIO.Common;
using BookingIO.Common.AuthRoles;
using BookingIO.Models;
using BookingIO.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using UserModel = BookingIO.Models.User;

namespace BookingIO.Controllers;
public class UserController : BaseController
{
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserService _userService;

    public UserController(
        UserManager<UserModel> userManager,
        SignInManager<UserModel> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserService userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userService = userService;
    }

    [HttpGet]
    [Route("/User/Login")]
    public IActionResult Login()
    {
        var model = new LoginModel();
        return View("Login", model);
    }

    [HttpPost]
    [Route("/User/Login")]
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
    [Route("/User/Register")]
    public IActionResult Register()
    {
        return View("Register");
    }

    [HttpPost]
    [Route("/User/Register")]
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
    [Route("/User/Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    [Route("/User")]
    public async Task<IActionResult> Index()
    {
        var models = await _userService.GetAll();
        return View("List", models);
    }

    [HttpGet]
    [Route("/User/{userId}")]
    public async Task<IActionResult> Edit([FromRoute] Guid userId)
    {
        var model = await _userService.GetById(userId);

        if (model == null) return NotFound();

        RegisterUserModel registerModel = new() {
            Id = userId,
            Name = model.FullName,
            Login = model.UserName ?? "",
            Email = model.Email ?? "",
            Password = "",
        };

        ViewData["userRoles"] = await _userManager.GetRolesAsync(model);
        ViewData["roles"] = Roles.All;

        return View("Edit", registerModel);
    }

    [HttpPost]
    [Route("/User/{userId}")]
    public async Task<IActionResult> Edit([FromRoute] Guid userId, RegisterUserModel editModel)
    {
        var model = await _userService.GetById(userId);

        if (model is null) return NotFound();

        var success = await _userService.Update(model, editModel);

        if(!success)
        {
            NotifyError(UserErrors.SaveFailed);
        } else
        {
            NotifySuccess("Salvo com Sucesso");
        }

        return RedirectToAction(nameof(Edit), userId);
    }

    [HttpPost]
    [Route("/User/{userId}/AddRole")]
    public async Task<IActionResult> AddRole([FromRoute] Guid userId, [FromForm] string role)
    {
        var user = await _userService.GetById(userId);

        if (user is null) return NotFound();

        var response = await _userManager.AddToRoleAsync(user, role);

        if (!response.Succeeded)
        {
            NotifyError(UserErrors.RoleFailed);
            return BadRequest();
        }

        NotifySuccess("Salvo com Sucesso");
        return Ok();
    }

    [HttpPost]
    [Route("/User/{userId}/RemoveRole")]
    public async Task<IActionResult> RemoveRole([FromRoute] Guid userId, [FromForm] string role)
    {
        var user = await _userService.GetById(userId);

        if (user is null) return NotFound();

        var response = await _userManager.RemoveFromRoleAsync(user, role);

        if (!response.Succeeded) {
            NotifyError(UserErrors.RoleFailed);
            return BadRequest();
        }

        NotifySuccess("Removido");
        return Ok();
    }
}
