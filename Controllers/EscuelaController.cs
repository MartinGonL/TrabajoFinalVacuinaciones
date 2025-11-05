using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class EscuelaController : Controller
{
    private readonly IRepositorioEscuela _repoEscuela;
    private readonly IRepositorioFotoEscuela _repoFoto;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EscuelaController(IRepositorioEscuela repoEscuela, IRepositorioFotoEscuela repoFoto, IWebHostEnvironment webHostEnvironment)
    {
        _repoEscuela = repoEscuela;
        _repoFoto = repoFoto;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: /Escuela
    public IActionResult Index()
    {
        var lista = _repoEscuela.ObtenerTodos();
        return View(lista);
    }

    // GET: /Escuela/Details/5
    public IActionResult Details(int id)
    {
        var escuela = _repoEscuela.ObtenerPorId(id);
        if (escuela == null) return NotFound();
        
        // Cargar fotos
        escuela.Fotos = _repoFoto.ObtenerPorEscuelaId(id).ToList();
        return View(escuela);
    }

    // GET: /Escuela/Create
    [Authorize(Roles = "Administrador, Agente")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Escuela/Create
    [HttpPost]
    [Authorize(Roles = "Administrador, Agente")]
    public IActionResult Create(Escuela escuela)
    {
        try
        {
            _repoEscuela.Alta(escuela);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(escuela);
        }
    }

    // GET: /Escuela/Edit/5
    [Authorize(Roles = "Administrador,Agente")]
    public IActionResult Edit(int id)
    {
        var escuela = _repoEscuela.ObtenerPorId(id);
        if (escuela == null) return NotFound();
        return View(escuela);
    }

    // POST: /Escuela/Edit/5
    [HttpPost]
    [Authorize(Roles = "Administrador,Agente")]
    public IActionResult Edit(int id, Escuela escuela)
    {
        try
        {
            escuela.EscuelaID = id;
            _repoEscuela.Modificar(escuela);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(escuela);
        }
    }

    // GET: /Escuela/Delete/5
    [Authorize(Roles = "Administrador")]
    public IActionResult Delete(int id)
    {
        var escuela = _repoEscuela.ObtenerPorId(id);
        if (escuela == null) return NotFound();
        return View(escuela);
    }

    // POST: /Escuela/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrador")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repoEscuela.Baja(id);
        return RedirectToAction(nameof(Index));
    }

    
    // POST: /Escuela/AgregarFoto
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public IActionResult AgregarFoto(FotoEscuela foto)
    {
        if (foto.FotoFile != null)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FotoFile.FileName);
            string path = Path.Combine(wwwRootPath, "uploads/escuelas", fileName);
            
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                foto.FotoFile.CopyTo(fileStream);
            }
            foto.FotoURL = "/uploads/escuelas/" + fileName;
            _repoFoto.Alta(foto);
        }
        return RedirectToAction(nameof(Details), new { id = foto.EscuelaID });
    }

    // POST: /Escuela/EliminarFoto/5
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public IActionResult EliminarFoto(int id)
    {
        var foto = _repoFoto.ObtenerPorId(id);
        if (foto == null) return NotFound();

        // Eliminar archivo físico
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string path = Path.Combine(wwwRootPath, foto.FotoURL.TrimStart('/'));
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        
        _repoFoto.Baja(id);
        return RedirectToAction(nameof(Details), new { id = foto.EscuelaID });
    }
    
    // --- API PARA BÚSQUEDA AJAX ---

    [HttpGet("api/escuelas/buscar")]
    [Produces("application/json")]
    public IActionResult BuscarEscuelas(string q)
    {
        var resultado = _repoEscuela.BuscarPorNombre(q);
        return Ok(resultado);
    }
}