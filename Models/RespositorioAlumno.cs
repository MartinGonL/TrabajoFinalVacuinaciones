using Dapper;
using MySql.Data.MySqlClient;

public class RepositorioAlumno : RepositorioBase, IRepositorioAlumno
{
    public RepositorioAlumno(string connectionString) : base(connectionString) { }

    public int Alta(Alumno alumno)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO Alumnos (Nombre, Apellido, DNI, FechaNacimiento, TelefonoTutor, EscuelaID)
                VALUES (@Nombre, @Apellido, @DNI, @FechaNacimiento, @TelefonoTutor, @EscuelaID);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, alumno);
        }
    }

    public int Baja(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "DELETE FROM Alumnos WHERE AlumnoID = @Id";
            return connection.Execute(sql, new { Id = id });
        }
    }

    public int Modificar(Alumno alumno)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                UPDATE Alumnos SET 
                Nombre = @Nombre, 
                Apellido = @Apellido, 
                DNI = @DNI, 
                FechaNacimiento = @FechaNacimiento, 
                TelefonoTutor = @TelefonoTutor, 
                EscuelaID = @EscuelaID
                WHERE AlumnoID = @AlumnoID";
            return connection.Execute(sql, alumno);
        }
    }

    public Alumno? ObtenerPorId(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT a.*, e.* FROM Alumnos a
                JOIN Escuelas e ON a.EscuelaID = e.EscuelaID
                WHERE a.AlumnoID = @Id";
            
            return connection.Query<Alumno, Escuela, Alumno>(
                sql,
                (alumno, escuela) =>
                {
                    alumno.Escuela = escuela;
                    return alumno;
                },
                new { Id = id },
                splitOn: "EscuelaID"
            ).FirstOrDefault();
        }
    }

    public IEnumerable<Alumno> ObtenerTodos()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT a.*, e.* FROM Alumnos a
                JOIN Escuelas e ON a.EscuelaID = e.EscuelaID";
            
            return connection.Query<Alumno, Escuela, Alumno>(
                sql,
                (alumno, escuela) =>
                {
                    alumno.Escuela = escuela;
                    return alumno;
                },
                splitOn: "EscuelaID"
            );
        }
    }

    public IEnumerable<Alumno> ObtenerPaginados(int pagina, int cantidad)
    {
        int offset = (pagina - 1) * cantidad;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT a.*, e.* FROM Alumnos a
                JOIN Escuelas e ON a.EscuelaID = e.EscuelaID
                ORDER BY a.Apellido, a.Nombre
                LIMIT @Offset, @Cantidad";
            
            return connection.Query<Alumno, Escuela, Alumno>(
                sql,
                (alumno, escuela) =>
                {
                    alumno.Escuela = escuela;
                    return alumno;
                },
                new { Offset = offset, Cantidad = cantidad },
                splitOn: "EscuelaID"
            );
        }
    }

    public int ObtenerTotal()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Alumnos");
        }
    }
    
    public IEnumerable<Alumno> ObtenerPorEscuelaId(int escuelaId)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Alumnos WHERE EscuelaID = @EscuelaId";
            return connection.Query<Alumno>(sql, new { EscuelaId = escuelaId });
        }
    }
}