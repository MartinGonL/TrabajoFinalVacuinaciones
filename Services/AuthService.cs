using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AuthService : IAuthService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public ClaimsPrincipal CreateClaimsPrincipal(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
            new Claim(ClaimTypes.GivenName, usuario.Nombre),
            new Claim(ClaimTypes.Surname, usuario.Apellido),
            new Claim(ClaimTypes.Role, usuario.Rol), // "Admin" o "Agente"
            
            // Un claim personalizado para la URL del avatar
            new Claim("AvatarURL", usuario.AvatarURL ?? "/uploads/avatars/default.png") 
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(claimsIdentity);
    }
}