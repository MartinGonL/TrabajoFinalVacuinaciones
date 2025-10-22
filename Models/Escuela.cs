using System.ComponentModel.DataAnnotations;

public class Escuela
{
    [Key]
    public int EscuelaID { get; set; }
    public string Nombre { get; set; }
    public int? Numero { get; set; }
    public string? Direccion { get; set; }

    // Propiedades de navegación
    public ICollection<FotoEscuela> Fotos { get; set; }
    public ICollection<Alumno> Alumnos { get; set; }
}