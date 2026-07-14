using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class AlumnoController : Controller
{
    private readonly IRepositorioAlumno _repoAlumno;
    private readonly IRepositorioEscuela _repoEscuela;

    public AlumnoController(IRepositorioAlumno repoAlumno, IRepositorioEscuela repoEscuela)
    {
        _repoAlumno = repoAlumno;
        _repoEscuela = repoEscuela;
    }

    // GET: /Alumno?pagina=1
    public IActionResult Index(int pagina = 1)
    {
        int cantidadPorPagina = 10;
        var total = _repoAlumno.ObtenerTotal();
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / cantidadPorPagina);
        ViewBag.PaginaActual = pagina;

        var lista = _repoAlumno.ObtenerPaginados(pagina, cantidadPorPagina);
        return View(lista);
    }

    // GET: /Alumno/Details/5
    public IActionResult Details(int id)
    {
        var alumno = _repoAlumno.ObtenerPorId(id);
        if (alumno == null) return NotFound();
        
        // Aquí podría cargar registros de vacunación
        // var registros = _repoRegistro.ObtenerPorAlumnoId(id);
        // ViewBag.Registros = registros;
        
        return View(alumno);
    }

    // GET: /Alumno/Create
    public IActionResult Create()
    {
        ViewBag.Escuelas = _repoEscuela.ObtenerTodos();
        return View();
    }

    // POST: /Alumno/Create
    [HttpPost]
    public IActionResult Create(Alumno alumno)
    {
        try
        {
            _repoAlumno.Alta(alumno);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            ViewBag.Escuelas = _repoEscuela.ObtenerTodos();
            return View(alumno);
        }
    }

    // GET: /Alumno/Edit/5
    public IActionResult Edit(int id)
    {
        var alumno = _repoAlumno.ObtenerPorId(id);
        if (alumno == null) return NotFound();
        
        ViewBag.Escuelas = _repoEscuela.ObtenerTodos();
        return View(alumno);
    }

    // POST: /Alumno/Edit/5
    [HttpPost]
    public IActionResult Edit(int id, Alumno alumno)
    {
        try
        {
            alumno.AlumnoID = id;
            _repoAlumno.Modificar(alumno);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            ViewBag.Escuelas = _repoEscuela.ObtenerTodos();
            return View(alumno);
        }
    }

    // GET: /Alumno/Delete/5
    [Authorize(Roles = "Administrador")]
    public IActionResult Delete(int id)
    {
        var alumno = _repoAlumno.ObtenerPorId(id);
        if (alumno == null) return NotFound();
        return View(alumno);
    }

    // POST: /Alumno/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrador")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repoAlumno.Baja(id);
        return RedirectToAction(nameof(Index));
    }

}