using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/login")]
public class ApiLoginController : ControllerBase
{
    private readonly IRepositorioUsuario _repoUsuario;
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public ApiLoginController(IRepositorioUsuario repoUsuario, IAuthService authService, IConfiguration config)
    {
        _repoUsuario = repoUsuario;
        _authService = authService;
        _config = config;
    }

    [HttpPost]
    [Produces("application/json")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { Error = "Email y contraseña son requeridos" });
        }

        var usuario = _repoUsuario.ObtenerPorEmail(request.Email);

        if (usuario == null || !_authService.VerifyPassword(request.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { Error = "Credenciales incorrectas" });
        }

        var jwtKey = _config["Jwt:Key"] ?? "ClaveSuperSecretaDeVacunacionDeMinimo32CaracteresDeLargo!";
        var jwtIssuer = _config["Jwt:Issuer"] ?? "VacunacionEscolarApi";
        var jwtAudience = _config["Jwt:Audience"] ?? "VacunacionEscolarApiUsers";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim(ClaimTypes.GivenName, usuario.Nombre),
            new Claim(ClaimTypes.Surname, usuario.Apellido)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new
        {
            Token = tokenString,
            Usuario = new
            {
                UsuarioID = usuario.UsuarioID,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Rol = usuario.Rol
            }
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
