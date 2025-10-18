using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{
    [Key]
    public int UsuarioID { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string DNI { get; set; }
    public string Matricula { get; set; }
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    public string Rol { get; set; } // "Admin" o "Agente"
    public string? AvatarURL { get; set; }
    public string? Telefono { get; set; }

    // Propiedad para manejar la subida del avatar
    [NotMapped] // No se guarda en la BD
    public IFormFile AvatarFile { get; set; }
}