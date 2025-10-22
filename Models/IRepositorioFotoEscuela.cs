public interface IRepositorioFotoEscuela
{
    int Alta(FotoEscuela foto);
    int Baja(int id);
    FotoEscuela? ObtenerPorId(int id);
    IEnumerable<FotoEscuela> ObtenerPorEscuelaId(int escuelaId);
}