using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrabajoFinalVacuinaciones.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly IRepositorioAlumno _repoAlumno;

        public AlumnosController(IRepositorioAlumno repoAlumno)
        {
            _repoAlumno = repoAlumno;
        }

        // GET: api/alumnos
        [HttpGet]
        public IActionResult Get([FromQuery] int pagina = 1, [FromQuery] string? q = null)
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

                return Ok(new
                {
                    PaginaActual = pagina,
                    TotalPaginas = totalPaginas,
                    Resultados = lista
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/alumnos/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var alumno = _repoAlumno.ObtenerPorId(id);
                if (alumno == null) return NotFound();
                return Ok(alumno);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/alumnos
        [HttpPost]
        public IActionResult Post([FromBody] Alumno alumno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = _repoAlumno.Alta(alumno);
                    var creado = _repoAlumno.ObtenerPorId(id);
                    return CreatedAtAction(nameof(Get), new { id = id }, creado);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/alumnos/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Alumno alumno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    alumno.AlumnoID = id;
                    _repoAlumno.Modificar(alumno);
                    var actualizado = _repoAlumno.ObtenerPorId(id);
                    return Ok(actualizado);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/alumnos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existente = _repoAlumno.ObtenerPorId(id);
                if (existente == null) return NotFound();

                _repoAlumno.Baja(id);
                return Ok(new { Mensaje = "Alumno eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
