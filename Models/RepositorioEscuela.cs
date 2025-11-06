
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioEscuela : RepositorioBase, IRepositorioEscuela
{
    public RepositorioEscuela(string connectionString) : base(connectionString) { }

    public int Alta(Escuela escuela)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO Escuelas (Nombre, Numero, Direccion)
                VALUES (@Nombre, @Numero, @Direccion);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", escuela.Nombre);
                command.Parameters.AddWithValue("@Numero", escuela.Numero);
                command.Parameters.AddWithValue("@Direccion", escuela.Direccion);
                id = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
        }
        return id;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var sqlFotos = "DELETE FROM FotosEscuela WHERE EscuelaID = @Id";
                    using (var cmdFotos = new MySqlCommand(sqlFotos, connection, transaction))
                    {
                        cmdFotos.Parameters.AddWithValue("@Id", id);
                        cmdFotos.ExecuteNonQuery();
                    }

                    var sqlAlumnos = "DELETE FROM Alumnos WHERE EscuelaID = @Id";
                    using (var cmdAlumnos = new MySqlCommand(sqlAlumnos, connection, transaction))
                    {
                        cmdAlumnos.Parameters.AddWithValue("@Id", id);
                        cmdAlumnos.ExecuteNonQuery();
                    }

                    var sqlEscuela = "DELETE FROM Escuelas WHERE EscuelaID = @Id";
                    using (var cmdEscuela = new MySqlCommand(sqlEscuela, connection, transaction))
                    {
                        cmdEscuela.Parameters.AddWithValue("@Id", id);
                        res = cmdEscuela.ExecuteNonQuery();
                    }
                    
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        return res;
    }

    public int Modificar(Escuela escuela)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                UPDATE Escuelas SET 
                Nombre = @Nombre, 
                Numero = @Numero, 
                Direccion = @Direccion
                WHERE EscuelaID = @EscuelaID";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", escuela.Nombre);
                command.Parameters.AddWithValue("@Numero", escuela.Numero);
                command.Parameters.AddWithValue("@Direccion", escuela.Direccion);
                command.Parameters.AddWithValue("@EscuelaID", escuela.EscuelaID);
                res = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return res;
    }

    public Escuela? ObtenerPorId(int id)
    {
        Escuela? res = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT EscuelaID, Nombre, Numero, Direccion FROM Escuelas WHERE EscuelaID = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res = new Escuela
                        {
                            EscuelaID = reader.GetInt32("EscuelaID"),
                            Nombre = reader.GetString("Nombre"),
                            Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetInt32("Numero"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion")
                        };
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<Escuela> ObtenerTodos()
    {
        var res = new List<Escuela>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT EscuelaID, Nombre, Numero, Direccion FROM Escuelas";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Escuela
                        {
                            EscuelaID = reader.GetInt32("EscuelaID"),
                            Nombre = reader.GetString("Nombre"),
                            Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetInt32("Numero"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion")
                        });
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<Escuela> BuscarPorNombre(string nombre)
    {
        var res = new List<Escuela>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT EscuelaID, Nombre, Numero, Direccion FROM Escuelas WHERE Nombre LIKE @Nombre LIMIT 10";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Escuela
                        {
                            EscuelaID = reader.GetInt32("EscuelaID"),
                            Nombre = reader.GetString("Nombre"),
                            Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetInt32("Numero"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion")
                        });
                    }
                }
                connection.Close();
            }
        }
        return res;
    }
}