using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RegistroVacunacion
{
    [Key]
    public int RegistroID { get; set; }
    public DateTime FechaAplicacion { get; set; }
    public int NumeroDosis { get; set; }
    public string? Observaciones { get; set; }

    public int AlumnoID { get; set; }
    public int AgenteID { get; set; }
    public int VacunaID { get; set; }
    public int LugarVacunacion_EscuelaID { get; set; }

    // Propiedades de navegación
    [ForeignKey(nameof(AlumnoID))]
    public Alumno Alumno { get; set; }

    [ForeignKey(nameof(AgenteID))]
    public Usuario Agente { get; set; }

    [ForeignKey(nameof(VacunaID))]
    public Vacuna Vacuna { get; set; }

    [ForeignKey(nameof(LugarVacunacion_EscuelaID))]
    public Escuela Escuela { get; set; }
}