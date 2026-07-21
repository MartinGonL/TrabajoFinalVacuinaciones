using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrabajoFinalVacuinaciones.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
    [ApiController]
    public class EscuelasController : ControllerBase
    {
        private readonly IRepositorioEscuela _repoEscuela;

        public EscuelasController(IRepositorioEscuela repoEscuela)
        {
            _repoEscuela = repoEscuela;
        }

        // GET: api/escuelas
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var escuelas = _repoEscuela.ObtenerTodos();
                return Ok(escuelas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/escuelas/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var escuela = _repoEscuela.ObtenerPorId(id);
                if (escuela == null) return NotFound();
                return Ok(escuela);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/escuelas/buscar?q=nombre
        [HttpGet("buscar")]
        public IActionResult Buscar([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q)) return Ok(_repoEscuela.ObtenerTodos());
                var resultado = _repoEscuela.BuscarPorNombre(q);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/escuelas
        [HttpPost]
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Post([FromBody] Escuela escuela)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = _repoEscuela.Alta(escuela);
                    var creada = _repoEscuela.ObtenerPorId(id);
                    return CreatedAtAction(nameof(Get), new { id = id }, creada);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/escuelas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Put(int id, [FromBody] Escuela escuela)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    escuela.EscuelaID = id;
                    _repoEscuela.Modificar(escuela);
                    var actualizada = _repoEscuela.ObtenerPorId(id);
                    return Ok(actualizada);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/escuelas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existente = _repoEscuela.ObtenerPorId(id);
                if (existente == null) return NotFound();

                _repoEscuela.Baja(id);
                return Ok(new { Mensaje = "Escuela eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
