using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/alumnos")]
[Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
public class ApiAlumnosController : ControllerBase
{
    private readonly IRepositorioAlumno _repoAlumno;

    public ApiAlumnosController(IRepositorioAlumno repoAlumno)
    {
        _repoAlumno = repoAlumno;
    }

    [HttpGet]
    [Produces("application/json")]
    [Authorize(Roles = "Administrador,Agente")]
    public IActionResult ObtenerAlumnosApi([FromQuery] int pagina = 1, [FromQuery] string? q = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var termino = q.Trim().ToLower();
                var resultados = _repoAlumno.ObtenerTodos()
                    .Where(a => a.Nombre.ToLower().Contains(termino) || 
                                a.Apellido.ToLower().Contains(termino) || 
                                a.DNI.Contains(termino))
                    .Take(10)
                    .ToList();
                return Ok(resultados);
            }

            int cantidadPorPagina = 10;
            var total = _repoAlumno.ObtenerTotal();
            var totalPaginas = (int)Math.Ceiling((double)total / cantidadPorPagina);
            var lista = _repoAlumno.ObtenerPaginados(pagina, cantidadPorPagina);
            
            return Ok(new {
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                Resultados = lista
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [Authorize(Roles = "Administrador,Agente")]
    public IActionResult ObtenerAlumnoApi(int id)
    {
        var alumno = _repoAlumno.ObtenerPorId(id);
        if (alumno == null) return NotFound();
        return Ok(alumno);
    }
    
    [HttpPost]
    [Produces("application/json")]
    [Authorize(Roles = "Administrador,Agente")]
    public IActionResult CrearAlumnoApi([FromBody] Alumno alumno)
    {
        try
        {
            var id = _repoAlumno.Alta(alumno);
            var nuevoAlumno = _repoAlumno.ObtenerPorId(id);
            return CreatedAtAction(nameof(ObtenerAlumnoApi), new { id = id }, nuevoAlumno);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Produces("application/json")]
    [Authorize(Roles = "Administrador,Agente")]
    public IActionResult EditarAlumnoApi(int id, [FromBody] Alumno alumno)
    {
        try
        {
            alumno.AlumnoID = id;
            _repoAlumno.Modificar(alumno);
            var alumnoActualizado = _repoAlumno.ObtenerPorId(id);
            return Ok(alumnoActualizado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Produces("application/json")]
    [Authorize(Roles = "Administrador")]
    public IActionResult EliminarAlumnoApi(int id)
    {
        try
        {
            _repoAlumno.Baja(id);
            return Ok(new { Mensaje = "Alumno eliminado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
