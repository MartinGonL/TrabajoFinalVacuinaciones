using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

public class Usuario
{
    [Key]
    public int UsuarioID { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string DNI { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    public string PasswordHash { get; set; } = null!;
    
    public string Rol { get; set; } = null!; // "Admin" o "Agente"
    public string? AvatarURL { get; set; }
    public string? Telefono { get; set; }
    public DateTime? FechaBaja { get; set; }
    public bool Borrado { get; set; }

    // Propiedad para manejar la subida del avatar
    [NotMapped] // No se guarda en la BD
    public IFormFile? AvatarFile { get; set; } // nullable
}