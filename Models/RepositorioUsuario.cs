using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
{
    public RepositorioUsuario(string connectionString) : base(connectionString) { }

    public int Alta(Usuario usuario)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO usuarios (Nombre, Apellido, DNI, Matricula, Email, PasswordHash, Rol, AvatarURL, Telefono)
                VALUES (@Nombre, @Apellido, @DNI, @Matricula, @Email, @PasswordHash, @Rol, @AvatarURL, @Telefono);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DNI", usuario.DNI ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Matricula", usuario.Matricula ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Rol", usuario.Rol ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AvatarURL", usuario.AvatarURL ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);

                id = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
        }
        return id;
    }

    // Baja física (DELETE). Si quieres soft delete, usa la ALTER TABLE anterior y cambia esto.
    public int Baja(int id)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "DELETE FROM usuarios WHERE UsuarioID = @Id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                rows = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return rows;
    }

    public int Modificar(Usuario usuario)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                UPDATE usuarios SET
                    Nombre = @Nombre,
                    Apellido = @Apellido,
                    DNI = @DNI,
                    Matricula = @Matricula,
                    Email = @Email,
                    Rol = @Rol,
                    Telefono = @Telefono";

            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                sql += ", PasswordHash = @PasswordHash";
            }

            if (!string.IsNullOrEmpty(usuario.AvatarURL))
            {
                sql += ", AvatarURL = @AvatarURL";
            }

            sql += " WHERE UsuarioID = @UsuarioID";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DNI", usuario.DNI ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Matricula", usuario.Matricula ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Rol", usuario.Rol ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);

                if (!string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
                }

                if (!string.IsNullOrEmpty(usuario.AvatarURL))
                {
                    command.Parameters.AddWithValue("@AvatarURL", usuario.AvatarURL);
                }

                command.Parameters.AddWithValue("@UsuarioID", usuario.UsuarioID);

                rows = command.ExecuteNonQuery();
            }

            connection.Close();
        }
        return rows;
    }

    public Usuario? ObtenerPorId(int id)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM usuarios WHERE UsuarioID = @Id LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = MapearUsuario(reader);
                    }
                }
            }
            connection.Close();
        }
        return usuario;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM usuarios WHERE Email = @Email LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = MapearUsuario(reader);
                    }
                }
            }
            connection.Close();
        }
        return usuario;
    }

    public IEnumerable<Usuario> ObtenerTodos()
    {
        var lista = new List<Usuario>();
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM usuarios";
            using (var command = new MySqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(MapearUsuario(reader));
                }
            }
            connection.Close();
        }
        return lista;
    }
    //recomendacion de copilot la dejo Qué hace, en pocas líneas:

    //Lee los campos del reader (r["Campo"]) y los transforma/convierte al tipo correcto.
    //Maneja valores NULL de la BD (DBNull) y los convierte a null o valores por defecto en C#.
    //Devuelve un objeto Usuario listo para usar en el código.
    //Por qué es útil:
    //Centraliza el mapeo (evita duplicar lógica en ObtenerPorId, ObtenerPorEmail, ObtenerTodos).
    //Reduce errores de conversión y NRE por valores NULL.
    //Facilita mantener el código cuando cambian columnas: solo hay que actualizar este método.
    private Usuario MapearUsuario(IDataRecord r)
    {
        return new Usuario
        {
            UsuarioID = r["UsuarioID"] == DBNull.Value ? 0 : Convert.ToInt32(r["UsuarioID"]),
            Nombre = r["Nombre"] == DBNull.Value ? null : r["Nombre"].ToString(),
            Apellido = r["Apellido"] == DBNull.Value ? null : r["Apellido"].ToString(),
            DNI = r["DNI"] == DBNull.Value ? null : r["DNI"].ToString(),
            Matricula = r["Matricula"] == DBNull.Value ? null : r["Matricula"].ToString(),
            Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
            PasswordHash = r["PasswordHash"] == DBNull.Value ? null : r["PasswordHash"].ToString(),
            Rol = r["Rol"] == DBNull.Value ? null : r["Rol"].ToString(),
            AvatarURL = r["AvatarURL"] == DBNull.Value ? null : r["AvatarURL"].ToString(),
            Telefono = r["Telefono"] == DBNull.Value ? null : r["Telefono"].ToString(),
        };
    }
}
