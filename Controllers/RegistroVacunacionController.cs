using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class RegistroVacunacionController : Controller
{
    private readonly IRepositorioRegistroVacunacion _repoRegistro;
    private readonly IRepositorioAlumno _repoAlumno;
    private readonly IRepositorioVacuna _repoVacuna;
    private readonly IRepositorioEscuela _repoEscuela;

    public RegistroVacunacionController(IRepositorioRegistroVacunacion repoRegistro, IRepositorioAlumno repoAlumno, IRepositorioVacuna repoVacuna, IRepositorioEscuela repoEscuela)
    {
        _repoRegistro = repoRegistro;
        _repoAlumno = repoAlumno;
        _repoVacuna = repoVacuna;
        _repoEscuela = repoEscuela;
    }

    // GET: /RegistroVacunacion
    public IActionResult Index()
    {
        var lista = _repoRegistro.ObtenerTodos();
        return View(lista);
    }
    
    // GET: /RegistroVacunacion/Registrar/5 (id = AlumnoID)
    public IActionResult Create(int alumnoId)
    {
        var alumno = _repoAlumno.ObtenerPorId(alumnoId);
        if (alumno == null) return NotFound();

        ViewBag.Alumno = alumno;
        ViewBag.Vacunas = _repoVacuna.ObtenerTodos();
        ViewBag.Escuelas = _repoEscuela.ObtenerTodos(); // Lugares de vacunación

        var registro = new RegistroVacunacion
        {
            AlumnoID = alumnoId,
            FechaAplicacion = DateTime.Now,
            LugarVacunacion_EscuelaID = alumno.EscuelaID // Sugerir la escuela del alumno
        };
        
        return View(registro);
    }

    // POST: /RegistroVacunacion/Create
    [HttpPost]
    public IActionResult Create(RegistroVacunacion registro)
    {
        try
        {
            // Obtener el ID del agente logueado
            var agenteId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            registro.AgenteID = agenteId;

            _repoRegistro.Alta(registro);
            
            // Redirigir al detalle del alumno
            return RedirectToAction("Details", "Alumno", new { id = registro.AlumnoID });
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            ViewBag.Alumno = _repoAlumno.ObtenerPorId(registro.AlumnoID);
            ViewBag.Vacunas = _repoVacuna.ObtenerTodos();
            ViewBag.Escuelas = _repoEscuela.ObtenerTodos();
            return View(registro);
        }
    }

    // GET: /RegistroVacunacion/Delete/5
    [Authorize(Roles = "Administrador")]
    public IActionResult Delete(int id)
    {
        var registro = _repoRegistro.ObtenerPorId(id);
        if (registro == null) return NotFound();
        // Cargar datos completos para la vista de confirmación
        registro = _repoRegistro.ObtenerTodos().FirstOrDefault(r => r.RegistroID == id);
        return View(registro);
    }

    // POST: /RegistroVacunacion/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrador")]
    public IActionResult DeleteConfirmed(int id)
    {
        var registro = _repoRegistro.ObtenerPorId(id);
        if (registro == null) return NotFound();
        
        _repoRegistro.Baja(id);
        return RedirectToAction("Details", "Alumno", new { id = registro.AlumnoID });
    }
}