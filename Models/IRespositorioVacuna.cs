public interface IRepositorioVacuna
{
    int Alta(Vacuna vacuna);
    int Baja(int id);
    int Modificar(Vacuna vacuna);
    IEnumerable<Vacuna> ObtenerTodos();
    Vacuna? ObtenerPorId(int id);
}