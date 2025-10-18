using Dapper;
using MySql.Data.MySqlClient;

public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
{
    public RepositorioUsuario(string connectionString) : base(connectionString) { }

    public int Alta(Usuario usuario)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO Usuarios (Nombre, Apellido, DNI, Matricula, Email, PasswordHash, Rol, AvatarURL, Telefono)
                VALUES (@Nombre, @Apellido, @DNI, @Matricula, @Email, @PasswordHash, @Rol, @AvatarURL, @Telefono);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, usuario);
        }
    }

    public int Baja(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "DELETE FROM Usuarios WHERE UsuarioID = @Id";
            return connection.Execute(sql, new { Id = id });
        }
    }

    public int Modificar(Usuario usuario)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                UPDATE Usuarios SET 
                Nombre = @Nombre, 
                Apellido = @Apellido, 
                DNI = @DNI, 
                Matricula = @Matricula, 
                Email = @Email, 
                Rol = @Rol, 
                Telefono = @Telefono";

            // Solo actualizar password si se provee uno nuevo
            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                sql += ", PasswordHash = @PasswordHash";
            }
            
            // Solo actualizar avatar si se provee uno nuevo
            if (!string.IsNullOrEmpty(usuario.AvatarURL))
            {
                sql += ", AvatarURL = @AvatarURL";
            }

            sql += " WHERE UsuarioID = @UsuarioID";
            return connection.Execute(sql, usuario);
        }
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Usuarios WHERE Email = @Email";
            return connection.QueryFirstOrDefault<Usuario>(sql, new { Email = email });
        }
    }

    public Usuario? ObtenerPorId(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Usuarios WHERE UsuarioID = @Id";
            return connection.QueryFirstOrDefault<Usuario>(sql, new { Id = id });
        }
    }

    public IEnumerable<Usuario> ObtenerTodos()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Usuarios";
            return connection.Query<Usuario>(sql);
        }
    }
}