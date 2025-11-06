using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(IRepositorioUsuario repositorioUsuario, IAuthService authService, IWebHostEnvironment env)
    {
        _repositorioUsuario = repositorioUsuario;
        _authService = authService;
        _env = env;
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var usuario = _repositorioUsuario.ObtenerPorEmail(email);

        if (usuario == null || !_authService.VerifyPassword(password, usuario.PasswordHash))
        {
            ViewBag.Error = "Email o contraseña incorrectos.";
            return View();
        }

        var claimsPrincipal = _authService.CreateClaimsPrincipal(usuario);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }

    // GET: /Auth/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View(new Usuario());
    }

    // POST: /Auth/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Usuario model, string password)
    {
        if (!ModelState.IsValid) return View(model);

        var exists = _repositorioUsuario.ObtenerPorEmail(model.Email);
        if (exists != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Ya existe un usuario con ese email.");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("password", "La contraseña es obligatoria.");
            return View(model);
        }

        // Guardar avatar si viene
        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploads);
            var fileName = $"{System.Guid.NewGuid()}{Path.GetExtension(model.AvatarFile.FileName)}";
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.AvatarFile.CopyToAsync(stream);
            }
            model.AvatarURL = $"/uploads/avatars/{fileName}";
        }

        // Hash y guardar
        model.PasswordHash = _authService.HashPassword(password);
        if (string.IsNullOrWhiteSpace(model.Rol)) model.Rol = "Agente";

        var newId = _repositorioUsuario.Alta(model);
        if (newId <= 0)
        {
            ModelState.AddModelError("", "No se pudo crear el usuario.");
            return View(model);
        }

        // Opcional: logueo automático (descomentar si quieres auto-login)
        // var saved = _repositorioUsuario.ObtenerPorId(newId);
        // var claims = _authService.CreateClaimsPrincipal(saved);
        // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims);

        return RedirectToAction("Login", "Auth");
    }
}