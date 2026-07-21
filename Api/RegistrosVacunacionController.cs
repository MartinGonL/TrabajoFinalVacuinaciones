using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrabajoFinalVacuinaciones.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
    [ApiController]
    public class RegistrosVacunacionController : ControllerBase
    {
        private readonly IRepositorioRegistroVacunacion _repoRegistro;

        public RegistrosVacunacionController(IRepositorioRegistroVacunacion repoRegistro)
        {
            _repoRegistro = repoRegistro;
        }

        // GET: api/registrosvacunacion
        [HttpGet]
        public IActionResult Get([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            try
            {
                var total = _repoRegistro.ObtenerTotal();
                var totalPaginas = (int)Math.Ceiling((double)total / cantidad);
                var lista = _repoRegistro.ObtenerPaginados(pagina, cantidad);

                return Ok(new
                {
                    PaginaActual = pagina,
                    TotalPaginas = totalPaginas,
                    TotalRegistros = total,
                    Resultados = lista
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/registrosvacunacion/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var registro = _repoRegistro.ObtenerPorId(id);
                if (registro == null) return NotFound();
                return Ok(registro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/registrosvacunacion/alumno/5
        [HttpGet("alumno/{alumnoId}")]
        public IActionResult GetPorAlumno(int alumnoId)
        {
            try
            {
                var registros = _repoRegistro.ObtenerPorAlumnoId(alumnoId);
                return Ok(registros);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/registrosvacunacion
        [HttpPost]
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Post([FromBody] RegistroVacunacion registro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = _repoRegistro.Alta(registro);
                    var creado = _repoRegistro.ObtenerPorId(id);
                    return CreatedAtAction(nameof(Get), new { id = id }, creado);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/registrosvacunacion/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Put(int id, [FromBody] RegistroVacunacion registro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    registro.RegistroID = id;
                    _repoRegistro.Modificar(registro);
                    var actualizado = _repoRegistro.ObtenerPorId(id);
                    return Ok(actualizado);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/registrosvacunacion/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existente = _repoRegistro.ObtenerPorId(id);
                if (existente == null) return NotFound();

                _repoRegistro.Baja(id);
                return Ok(new { Mensaje = "Registro de vacunación eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
