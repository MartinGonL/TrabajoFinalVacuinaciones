public interface IRepositorioUsuario
{
    int Alta(Usuario usuario);
    int Baja(int id);
    int Modificar(Usuario usuario);
    IEnumerable<Usuario> ObtenerTodos();
    Usuario? ObtenerPorId(int id);
    Usuario? ObtenerPorEmail(string email);
}