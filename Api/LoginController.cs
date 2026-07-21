using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TrabajoFinalVacuinaciones.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IRepositorioUsuario _repoUsuario;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public LoginController(IRepositorioUsuario repoUsuario, IAuthService authService, IConfiguration config)
        {
            _repoUsuario = repoUsuario;
            _authService = authService;
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest("Debe ingresar email y contraseña");
                }

                var usuario = _repoUsuario.ObtenerPorEmail(model.Email);
                if (usuario == null || !_authService.VerifyPassword(model.Password, usuario.PasswordHash))
                {
                    return BadRequest("Email o clave incorrecta");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "ClaveSuperSecretaDeVacunacionDeMinimo32CaracteresDeLargo!"));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}")
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"] ?? "VacunacionEscolarApi",
                    audience: _config["Jwt:Audience"] ?? "VacunacionEscolarApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddHours(4),
                    signingCredentials: credenciales
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    Token = tokenString,
                    Usuario = new
                    {
                        usuario.UsuarioID,
                        usuario.Email,
                        usuario.Nombre,
                        usuario.Apellido,
                        usuario.Rol
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
