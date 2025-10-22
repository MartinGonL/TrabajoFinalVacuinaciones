public interface IRepositorioRegistroVacunacion
{
    int Alta(RegistroVacunacion registro);
    int Baja(int id);
    int Modificar(RegistroVacunacion registro);
    IEnumerable<RegistroVacunacion> ObtenerTodos();
    RegistroVacunacion? ObtenerPorId(int id);
    IEnumerable<RegistroVacunacion> ObtenerPorAlumnoId(int alumnoId);
}