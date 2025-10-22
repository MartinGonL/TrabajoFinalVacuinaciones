using Dapper;
using MySql.Data.MySqlClient;

public class RepositorioRegistroVacunacion : RepositorioBase, IRepositorioRegistroVacunacion
{
    public RepositorioRegistroVacunacion(string connectionString) : base(connectionString) { }

    public int Alta(RegistroVacunacion registro)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO RegistrosVacunacion (FechaAplicacion, NumeroDosis, Observaciones, AlumnoID, AgenteID, VacunaID, LugarVacunacion_EscuelaID)
                VALUES (@FechaAplicacion, @NumeroDosis, @Observaciones, @AlumnoID, @AgenteID, @VacunaID, @LugarVacunacion_EscuelaID);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, registro);
        }
    }

    public int Baja(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "DELETE FROM RegistrosVacunacion WHERE RegistroID = @Id";
            return connection.Execute(sql, new { Id = id });
        }
    }

    public int Modificar(RegistroVacunacion registro)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
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
            return connection.Execute(sql, registro);
        }
    }

    public RegistroVacunacion? ObtenerPorId(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM RegistrosVacunacion WHERE RegistroID = @Id";
            // Aquí un JOIN más complejo sería necesario para traer todos los datos
            // Por simplicidad, solo traigo el registro.
            return connection.QueryFirstOrDefault<RegistroVacunacion>(sql, new { Id = id });
        }
    }

    public IEnumerable<RegistroVacunacion> ObtenerTodos()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            // Este JOIN es el más complejo y completo
            var sql = @"
                SELECT r.*, a.*, u.*, v.*, e.*
                FROM RegistrosVacunacion r
                JOIN Alumnos a ON r.AlumnoID = a.AlumnoID
                JOIN Usuarios u ON r.AgenteID = u.UsuarioID
                JOIN Vacunas v ON r.VacunaID = v.VacunaID
                JOIN Escuelas e ON r.LugarVacunacion_EscuelaID = e.EscuelaID";
            
            return connection.Query<RegistroVacunacion, Alumno, Usuario, Vacuna, Escuela, RegistroVacunacion>(
                sql,
                (registro, alumno, agente, vacuna, escuela) =>
                {
                    registro.Alumno = alumno;
                    registro.Agente = agente;
                    registro.Vacuna = vacuna;
                    registro.Escuela = escuela;
                    return registro;
                },
                splitOn: "AlumnoID,UsuarioID,VacunaID,EscuelaID"
            );
        }
    }

    public IEnumerable<RegistroVacunacion> ObtenerPorAlumnoId(int alumnoId)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            // Similar al anterior, pero filtrado por alumno
            var sql = @"
                SELECT r.*, a.*, u.*, v.*, e.*
                FROM RegistrosVacunacion r
                JOIN Alumnos a ON r.AlumnoID = a.AlumnoID
                JOIN Usuarios u ON r.AgenteID = u.UsuarioID
                JOIN Vacunas v ON r.VacunaID = v.VacunaID
                JOIN Escuelas e ON r.LugarVacunacion_EscuelaID = e.EscuelaID
                WHERE r.AlumnoID = @AlumnoId";
            
            return connection.Query<RegistroVacunacion, Alumno, Usuario, Vacuna, Escuela, RegistroVacunacion>(
                sql,
                (registro, alumno, agente, vacuna, escuela) =>
                {
                    registro.Alumno = alumno;
                    registro.Agente = agente;
                    registro.Vacuna = vacuna;
                    registro.Escuela = escuela;
                    return registro;
                },
                new { AlumnoId = alumnoId },
                splitOn: "AlumnoID,UsuarioID,VacunaID,EscuelaID"
            );
        }
    }
}