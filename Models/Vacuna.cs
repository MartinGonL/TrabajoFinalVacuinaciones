using System.ComponentModel.DataAnnotations;

public class Vacuna
{
    [Key]
    public int VacunaID { get; set; }
    public string NombreVacuna { get; set; }
    public string? Descripcion { get; set; }
}