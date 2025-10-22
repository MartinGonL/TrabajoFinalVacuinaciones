using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Alumno
{
    [Key]
    public int AlumnoID { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string DNI { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? TelefonoTutor { get; set; }
    
    public int EscuelaID { get; set; }

    // Propiedad de navegación
    [ForeignKey(nameof(EscuelaID))]
    public Escuela Escuela { get; set; }
}