
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioAlumno : RepositorioBase, IRepositorioAlumno
{
    public RepositorioAlumno(string connectionString) : base(connectionString) { }

    public int Alta(Alumno alumno)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO Alumnos (Nombre, Apellido, DNI, FechaNacimiento, TelefonoTutor, EscuelaID)
                VALUES (@Nombre, @Apellido, @DNI, @FechaNacimiento, @TelefonoTutor, @EscuelaID);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", alumno.Nombre);
                command.Parameters.AddWithValue("@Apellido", alumno.Apellido);
                command.Parameters.AddWithValue("@DNI", alumno.DNI);
                command.Parameters.AddWithValue("@FechaNacimiento", alumno.FechaNacimiento);
                command.Parameters.AddWithValue("@TelefonoTutor", alumno.TelefonoTutor);
                command.Parameters.AddWithValue("@EscuelaID", alumno.EscuelaID);
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
            var sql = "DELETE FROM Alumnos WHERE AlumnoID = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Alumno alumno)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                UPDATE Alumnos SET 
                Nombre = @Nombre, 
                Apellido = @Apellido, 
                DNI = @DNI, 
                FechaNacimiento = @FechaNacimiento, 
                TelefonoTutor = @TelefonoTutor, 
                EscuelaID = @EscuelaID
                WHERE AlumnoID = @AlumnoID";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", alumno.Nombre);
                command.Parameters.AddWithValue("@Apellido", alumno.Apellido);
                command.Parameters.AddWithValue("@DNI", alumno.DNI);
                command.Parameters.AddWithValue("@FechaNacimiento", alumno.FechaNacimiento);
                command.Parameters.AddWithValue("@TelefonoTutor", alumno.TelefonoTutor);
                command.Parameters.AddWithValue("@EscuelaID", alumno.EscuelaID);
                command.Parameters.AddWithValue("@AlumnoID", alumno.AlumnoID);
                res = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return res;
    }

    public Alumno? ObtenerPorId(int id)
    {
        Alumno? res = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT a.AlumnoID, a.Nombre, a.Apellido, a.DNI, a.FechaNacimiento, a.TelefonoTutor, a.EscuelaID,
                       e.Nombre AS NombreEscuela, e.Numero, e.Direccion 
                FROM Alumnos a JOIN Escuelas e ON a.EscuelaID = e.EscuelaID
                WHERE a.AlumnoID = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res = new Alumno
                        {
                            AlumnoID = reader.GetInt32("AlumnoID"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            DNI = reader.GetString("DNI"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                            TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? null : reader.GetString("TelefonoTutor"),
                            EscuelaID = reader.GetInt32("EscuelaID"),
                            Escuela = new Escuela
                            {
                                EscuelaID = reader.GetInt32("EscuelaID"),
                                Nombre = reader.GetString("NombreEscuela"),
                                Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetInt32("Numero"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion")
                            }
                        };
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<Alumno> ObtenerTodos()
    {
        var res = new List<Alumno>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT a.AlumnoID, a.Nombre, a.Apellido, a.DNI, a.FechaNacimiento, a.TelefonoTutor, a.EscuelaID,
                       e.Nombre AS NombreEscuela, e.Numero, e.Direccion 
                FROM Alumnos a JOIN Escuelas e ON a.EscuelaID = e.EscuelaID";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Alumno
                        {
                            AlumnoID = reader.GetInt32("AlumnoID"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            DNI = reader.GetString("DNI"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                            TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? null : reader.GetString("TelefonoTutor"),
                            EscuelaID = reader.GetInt32("EscuelaID"),
                            Escuela = new Escuela
                            {
                                EscuelaID = reader.GetInt32("EscuelaID"),
                                Nombre = reader.GetString("NombreEscuela"),
                                Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetInt32("Numero"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion")
                            }
                        });
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<Alumno> ObtenerPaginados(int pagina, int cantidad)
    {
        int offset = (pagina - 1) * cantidad;
        var res = new List<Alumno>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT a.AlumnoID, a.Nombre, a.Apellido, a.DNI, a.FechaNacimiento, a.TelefonoTutor, a.EscuelaID,
                       e.Nombre AS NombreEscuela, e.Numero, e.Direccion 
                FROM Alumnos a JOIN Escuelas e ON a.EscuelaID = e.EscuelaID
                ORDER BY a.Apellido, a.Nombre
                LIMIT @Offset, @Cantidad";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Cantidad", cantidad);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Alumno
                        {
                            AlumnoID = reader.GetInt32("AlumnoID"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            DNI = reader.GetString("DNI"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                            TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? null : reader.GetString("TelefonoTutor"),
                            EscuelaID = reader.GetInt32("EscuelaID"),
                            Escuela = new Escuela
                            {
                                EscuelaID = reader.GetInt32("EscuelaID"),
                                Nombre = reader.GetString("NombreEscuela"),
                                Numero = reader.IsDBNull(reader.GetOrdinal("Numero")) ? null : reader.GetInt32("Numero"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion")
                            }
                        });
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public int ObtenerTotal()
    {
        int total = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT COUNT(*) FROM Alumnos";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                total = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return total;
    }
    
    public IEnumerable<Alumno> ObtenerPorEscuelaId(int escuelaId)
    {
        var res = new List<Alumno>();
        using (var connection = new MySqlConnection(connectionString))
        {
            // Nota: Esta consulta no carga la escuela completa, solo el ID.
            var sql = "SELECT AlumnoID, Nombre, Apellido, DNI, FechaNacimiento, TelefonoTutor, EscuelaID FROM Alumnos WHERE EscuelaID = @escuelaId";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@escuelaId", escuelaId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Alumno
                        {
                            AlumnoID = reader.GetInt32("AlumnoID"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            DNI = reader.GetString("DNI"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                            TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? null : reader.GetString("TelefonoTutor"),
                            EscuelaID = reader.GetInt32("EscuelaID")
                        });
                    }
                }
                connection.Close();
            }
        }
        return res;
    }
}