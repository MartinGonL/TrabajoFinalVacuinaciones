using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FotoEscuela
{
    [Key]
    public int FotoID { get; set; }
    public string FotoURL { get; set; }
    public string? Descripcion { get; set; }
    
    public int EscuelaID { get; set; }

    // Propiedad de navegación
    [ForeignKey(nameof(EscuelaID))]
    public Escuela Escuela { get; set; }
    
    [NotMapped]
    public IFormFile FotoFile { get; set; }
}