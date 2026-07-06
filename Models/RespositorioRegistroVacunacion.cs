
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioRegistroVacunacion : RepositorioBase, IRepositorioRegistroVacunacion
{
    public RepositorioRegistroVacunacion(string connectionString) : base(connectionString) { }

    public int Alta(RegistroVacunacion registro)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO RegistrosVacunacion (FechaAplicacion, NumeroDosis, Observaciones, AlumnoID, AgenteID, VacunaID, LugarVacunacion_EscuelaID)
                VALUES (@FechaAplicacion, @NumeroDosis, @Observaciones, @AlumnoID, @AgenteID, @VacunaID, @LugarVacunacion_EscuelaID);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@FechaAplicacion", registro.FechaAplicacion);
                command.Parameters.AddWithValue("@NumeroDosis", registro.NumeroDosis);
                command.Parameters.AddWithValue("@Observaciones", registro.Observaciones);
                command.Parameters.AddWithValue("@AlumnoID", registro.AlumnoID);
                command.Parameters.AddWithValue("@AgenteID", registro.AgenteID);
                command.Parameters.AddWithValue("@VacunaID", registro.VacunaID);
                command.Parameters.AddWithValue("@LugarVacunacion_EscuelaID", registro.LugarVacunacion_EscuelaID);
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
            var sql = "DELETE FROM RegistrosVacunacion WHERE RegistroID = @id";
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

    public int Modificar(RegistroVacunacion registro)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                UPDATE RegistrosVacunacion SET 
                FechaAplicacion = @FechaAplicacion,
                NumeroDosis = @NumeroDosis,
                Observaciones = @Observaciones,
                AlumnoID = @AlumnoID,
                AgenteID = @AgenteID,
                VacunaID = @VacunaID,
                LugarVacunacion_EscuelaID = @LugarVacunacion_EscuelaID
                WHERE RegistroID = @RegistroID";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@FechaAplicacion", registro.FechaAplicacion);
                command.Parameters.AddWithValue("@NumeroDosis", registro.NumeroDosis);
                command.Parameters.AddWithValue("@Observaciones", registro.Observaciones);
                command.Parameters.AddWithValue("@AlumnoID", registro.AlumnoID);
                command.Parameters.AddWithValue("@AgenteID", registro.AgenteID);
                command.Parameters.AddWithValue("@VacunaID", registro.VacunaID);
                command.Parameters.AddWithValue("@LugarVacunacion_EscuelaID", registro.LugarVacunacion_EscuelaID);
                command.Parameters.AddWithValue("@RegistroID", registro.RegistroID);
                res = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return res;
    }

    public RegistroVacunacion? ObtenerPorId(int id)
    {
        RegistroVacunacion? res = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = GetQueryBase() + " WHERE r.RegistroID = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res = MapFromReader(reader);
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<RegistroVacunacion> ObtenerTodos()
    {
        var res = new List<RegistroVacunacion>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = GetQueryBase();
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(MapFromReader(reader));
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<RegistroVacunacion> ObtenerPorAlumnoId(int alumnoId)
    {
        var res = new List<RegistroVacunacion>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = GetQueryBase() + " WHERE r.AlumnoID = @alumnoId";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@alumnoId", alumnoId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(MapFromReader(reader));
                    }
                }
                connection.Close();
            }
        }
        return res;
    }

    public IEnumerable<RegistroVacunacion> ObtenerPaginados(int pagina, int cantidad)
    {
        int offset = (pagina - 1) * cantidad;
        var res = new List<RegistroVacunacion>();
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = GetQueryBase() + " ORDER BY r.FechaAplicacion DESC LIMIT @Offset, @Cantidad";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Cantidad", cantidad);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(MapFromReader(reader));
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
            var sql = "SELECT COUNT(*) FROM RegistrosVacunacion";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                total = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return total;
    }

    private string GetQueryBase()
    {
        return @"
            SELECT 
                r.RegistroID, r.FechaAplicacion, r.NumeroDosis, r.Observaciones,
                a.AlumnoID, a.Nombre, a.Apellido, a.DNI, a.FechaNacimiento, a.TelefonoTutor,
                u.UsuarioID, u.Nombre AS NombreAgente, u.Apellido AS ApellidoAgente, u.DNI AS DNIAgente, u.Matricula, u.Email, u.Rol, u.Telefono,
                v.VacunaID, v.NombreVacuna, v.Descripcion,
                e.EscuelaID, e.Nombre AS NombreEscuela, e.Numero, e.Direccion
            FROM RegistrosVacunacion r
            JOIN Alumnos a ON r.AlumnoID = a.AlumnoID
            JOIN Usuarios u ON r.AgenteID = u.UsuarioID
            JOIN Vacunas v ON r.VacunaID = v.VacunaID
            JOIN Escuelas e ON r.LugarVacunacion_EscuelaID = e.EscuelaID";
    }

    private RegistroVacunacion MapFromReader(MySqlDataReader reader)
    {
        return new RegistroVacunacion
        {
            RegistroID = reader.GetInt32("RegistroID"),
            FechaAplicacion = reader.GetDateTime("FechaAplicacion"),
            NumeroDosis = reader.GetInt32("NumeroDosis"),
            Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString("Observaciones"),
            AlumnoID = reader.GetInt32("AlumnoID"),
            AgenteID = reader.GetInt32("UsuarioID"),
            VacunaID = reader.GetInt32("VacunaID"),
            LugarVacunacion_EscuelaID = reader.GetInt32("EscuelaID"),
            Alumno = new Alumno
            {
                AlumnoID = reader.GetInt32("AlumnoID"),
                Nombre = reader.GetString("Nombre"),
                Apellido = reader.GetString("Apellido"),
                DNI = reader.GetString("DNI"),
                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? null : reader.GetString("TelefonoTutor"),
                EscuelaID = reader.GetInt32("EscuelaID")
            },
            Agente = new Usuario
            {
                UsuarioID = reader.GetInt32("UsuarioID"),
                Nombre = reader.GetString("NombreAgente"),
                Apellido = reader.GetString("ApellidoAgente"),
                DNI = reader.GetString("DNIAgente"),
                Matricula = reader.GetString("Matricula"),
                Email = reader.GetString("Email"),
                Rol = reader.GetString("Rol"),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono")
            },
            Vacuna = new Vacuna
            {
                VacunaID = reader.GetInt32("VacunaID"),
                NombreVacuna = reader.GetString("NombreVacuna"),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString("Descripcion")
            },
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