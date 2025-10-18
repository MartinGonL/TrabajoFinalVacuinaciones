using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AuthController : Controller
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IAuthService _authService; // <-- Inyectamos el servicio

    // Inyectamos ambos servicios
    public AuthController(IRepositorioUsuario repositorioUsuario, IAuthService authService)
    {
        _repositorioUsuario = repositorioUsuario;
        _authService = authService;
    }

    // GET: /Auth/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var usuario = _repositorioUsuario.ObtenerPorEmail(email);

        // Usamos el servicio para verificar la contraseña
        if (usuario == null || !_authService.VerifyPassword(password, usuario.PasswordHash))
        {
            ViewBag.Error = "Email o contraseña incorrectos.";
            return View();
        }

        // Usamos el servicio para crear los claims
        var claimsPrincipal = _authService.CreateClaimsPrincipal(usuario);

        // Logueamos al usuario con los claims generados
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal);

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }
}