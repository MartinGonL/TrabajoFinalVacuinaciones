using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 

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
            // DEBUG: volcar keys recibidas por el servidor (ver salida de la app con dotnet run)
            Console.WriteLine("POST /Usuario/Create recibida. Form keys:");
            foreach (var k in Request.Form.Keys)
            {
                Console.WriteLine($"  {k} = {Request.Form[k]}");
            }

            // Validación básica
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

            // Email único
            var existe = _repositorioUsuario.ObtenerPorEmail(usuario.Email);
            if (existe != null)
            {
                ModelState.AddModelError(nameof(usuario.Email), "Ya existe un usuario con ese email.");
                ViewBag.ModelErrors = "Email ya registrado.";
                return View(usuario);
            }

            // Role por defecto
            if (string.IsNullOrWhiteSpace(usuario.Rol)) usuario.Rol = "Agente";

            // Hashear contraseña
            usuario.PasswordHash = _authService.HashPassword(password);

            // Guardar avatar (si viene)
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

            // Insertar en BD
            var newId = _repositorioUsuario.Alta(usuario);

            // Comprobación simple del resultado
            if (newId <= 0)
            {
                ModelState.AddModelError("", "No se insertó el usuario en la base de datos.");
                ViewBag.ModelErrors = "Alta devolvió id inválido: " + newId;
                return View(usuario);
            }

            // Éxito
            Console.WriteLine($"Usuario creado con ID {newId}");
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            // Mostrar la excepción en la vista para depuración temporal
            Console.WriteLine("Excepción en Usuario/Create: " + ex);
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
    public IActionResult Edit(int id, Usuario usuario, string? newPassword)
    {
        
         if (!User.IsInRole("Administrador") && Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) != id)
        {
            return RedirectToAction("AccesoDenegado", "Home");
        }

        try
        {
            
            if (!User.IsInRole("Administrador"))
            {
                var usuarioActual = _repositorioUsuario.ObtenerPorId(id);
                usuario.Rol = usuarioActual.Rol; 
            }

            
            if (!string.IsNullOrEmpty(newPassword))
            {
                usuario.PasswordHash = _authService.HashPassword(newPassword);
            }
            
            
            if (usuario.AvatarFile != null)
            {
                
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(usuario.AvatarFile.FileName);
                string path = Path.Combine(wwwRootPath, "uploads/avatars", fileName);
                
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    usuario.AvatarFile.CopyTo(fileStream);
                }
                usuario.AvatarURL = "/uploads/avatars/" + fileName;
            }

            usuario.UsuarioID = id;
            _repositorioUsuario.Modificar(usuario);
            
            return RedirectToAction(nameof(Index));
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