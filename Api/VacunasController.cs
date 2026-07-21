using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrabajoFinalVacuinaciones.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
    [ApiController]
    public class VacunasController : ControllerBase
    {
        private readonly IRepositorioVacuna _repoVacuna;

        public VacunasController(IRepositorioVacuna repoVacuna)
        {
            _repoVacuna = repoVacuna;
        }

        // GET: api/vacunas
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var vacunas = _repoVacuna.ObtenerTodos();
                return Ok(vacunas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/vacunas/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var vacuna = _repoVacuna.ObtenerPorId(id);
                if (vacuna == null) return NotFound();
                return Ok(vacuna);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/vacunas
        [HttpPost]
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Post([FromBody] Vacuna vacuna)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = _repoVacuna.Alta(vacuna);
                    var creada = _repoVacuna.ObtenerPorId(id);
                    return CreatedAtAction(nameof(Get), new { id = id }, creada);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/vacunas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Agente")]
        public IActionResult Put(int id, [FromBody] Vacuna vacuna)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    vacuna.VacunaID = id;
                    _repoVacuna.Modificar(vacuna);
                    var actualizada = _repoVacuna.ObtenerPorId(id);
                    return Ok(actualizada);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/vacunas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existente = _repoVacuna.ObtenerPorId(id);
                if (existente == null) return NotFound();

                _repoVacuna.Baja(id);
                return Ok(new { Mensaje = "Vacuna eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
