using System.Security.Claims;

public interface IAuthService
{
    // Hashea una contraseña en texto plano
    string HashPassword(string password);

    // Verifica si una contraseña en texto plano coincide con un hash
    bool VerifyPassword(string password, string hash);

    // Crea el ClaimsPrincipal para la cookie de autenticación
    ClaimsPrincipal CreateClaimsPrincipal(Usuario usuario);
}