
public interface IRepositorioEscuela
{
    int Alta(Escuela escuela);
    int Baja(int id);
    int Modificar(Escuela escuela);
    IEnumerable<Escuela> ObtenerTodos();
    Escuela? ObtenerPorId(int id);
    IEnumerable<Escuela> BuscarPorNombre(string nombre);
}