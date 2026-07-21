using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

[Authorize] 
public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IAuthService _authService; 

    public UsuarioController(IRepositorioUsuario repositorioUsuario, 
                             IWebHostEnvironment webHostEnvironment, 
                             IAuthService authService)
    {
        _repositorioUsuario = repositorioUsuario;
        _webHostEnvironment = webHostEnvironment;
        _authService = authService;
    }

    // GET: /Usuario    
    [AllowAnonymous]
    public IActionResult Index()
    {
        var usuarios = _repositorioUsuario.ObtenerTodos();
        return View(usuarios);
    }

    // GET: /Usuario/Details/5
    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // GET: /Usuario/Profile
    public IActionResult Profile()
    {
        var claimId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(claimId)) return RedirectToAction("Login", "Auth");
        
        int id = Convert.ToInt32(claimId);
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        
        return View("Details", usuario);
    }

    // GET: /Usuario/Create
    [AllowAnonymous]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Usuario/Create
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Usuario usuario, string password)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errs = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.ModelErrors = "ModelState inválido: " + errs;
                return View(usuario);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "La contraseña es obligatoria.");
                ViewBag.ModelErrors = "Falta contraseña.";
                return View(usuario);
            }

            var existe = _repositorioUsuario.ObtenerPorEmail(usuario.Email);
            if (existe != null)
            {
                ModelState.AddModelError(nameof(usuario.Email), "Ya existe un usuario con ese email.");
                ViewBag.ModelErrors = "Email ya registrado.";
                return View(usuario);
            }

            if (string.IsNullOrWhiteSpace(usuario.Rol)) usuario.Rol = "Agente";

            usuario.PasswordHash = _authService.HashPassword(password);

            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath ?? "wwwroot";
                string uploadsFolder = Path.Combine(wwwRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(usuario.AvatarFile.FileName);
                string path = Path.Combine(uploadsFolder, fileName);
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    usuario.AvatarFile.CopyTo(fs);
                }
                usuario.AvatarURL = "/uploads/avatars/" + fileName;
            }

            var newId = _repositorioUsuario.Alta(usuario);

            if (newId <= 0)
            {
                ModelState.AddModelError("", "No se insertó el usuario en la base de datos.");
                ViewBag.ModelErrors = "Alta devolvió id inválido: " + newId;
                return View(usuario);
            }

            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error: " + ex.Message);
            ViewBag.ModelErrors = ex.ToString();
            return View(usuario);
        }
    }

    // GET: /Usuario/Edit/5
    public IActionResult Edit(int id)
    {
        if (!User.IsInRole("Administrador") && Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) != id)
        {
            return RedirectToAction("AccesoDenegado", "Home");
        }
        
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        
        usuario.PasswordHash = null; 
        return View(usuario);
    }

    // POST: /Usuario/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Usuario usuario, string? newPassword)
    {
        if (!User.IsInRole("Administrador") && Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) != id)
        {
            return RedirectToAction("AccesoDenegado", "Home");
        }

        try
        {
            var usuarioActual = _repositorioUsuario.ObtenerPorId(id);
            if (usuarioActual == null) return NotFound();

            if (!User.IsInRole("Administrador"))
            {
                usuario.Rol = usuarioActual.Rol; 
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                usuario.PasswordHash = _authService.HashPassword(newPassword);
            }
            else
            {
                usuario.PasswordHash = usuarioActual.PasswordHash;
            }
            
            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath ?? "wwwroot";
                string uploadsFolder = Path.Combine(wwwRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);
                
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(usuario.AvatarFile.FileName);
                string path = Path.Combine(uploadsFolder, fileName);
                
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await usuario.AvatarFile.CopyToAsync(fileStream);
                }
                usuario.AvatarURL = "/uploads/avatars/" + fileName;
            }
            else
            {
                usuario.AvatarURL = usuarioActual.AvatarURL;
            }

            usuario.UsuarioID = id;
            _repositorioUsuario.Modificar(usuario);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId) && currentUserId == id.ToString())
            {
                var usuarioActualizado = _repositorioUsuario.ObtenerPorId(id);
                if (usuarioActualizado != null)
                {
                    var claimsPrincipal = _authService.CreateClaimsPrincipal(usuarioActualizado);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                }
            }

            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Profile));
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(usuario);
        }
    }

    // GET: /Usuario/Delete/5
    [Authorize(Policy = "Administrador")] 
    public IActionResult Delete(int id)
    {
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // POST: /Usuario/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "Administrador")] 
    public IActionResult DeleteConfirmed(int id)
    {
        _repositorioUsuario.Baja(id);
        return RedirectToAction(nameof(Index));
    }
}