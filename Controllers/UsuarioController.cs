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
    [Authorize(Policy = "Administrador")] 
    public IActionResult Index()
    {
        var usuarios = _repositorioUsuario.ObtenerTodos();
        return View(usuarios);
    }

    // GET: /Usuario/Create
    [Authorize(Policy = "Administrador")] 
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Usuario/Create
    [HttpPost]
    [Authorize(Policy = "Administrador")] 
    public IActionResult Create(Usuario usuario, string password)
    {
        try
        {
            
            usuario.PasswordHash = _authService.HashPassword(password);
            
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

            _repositorioUsuario.Alta(usuario);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(usuario);
        }
    }

    // GET: /Usuario/Edit/5
    public IActionResult Edit(int id)
    {
        
        if (!User.IsInRole("Admin") && Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) != id)
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
        
         if (!User.IsInRole("Admin") && Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)) != id)
        {
            return RedirectToAction("AccesoDenegado", "Home");
        }

        try
        {
            
            if (!User.IsInRole("Admin"))
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