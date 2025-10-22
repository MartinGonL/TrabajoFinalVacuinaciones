using Dapper;
using MySql.Data.MySqlClient;

public class RepositorioEscuela : RepositorioBase, IRepositorioEscuela
{
    public RepositorioEscuela(string connectionString) : base(connectionString) { }

    public int Alta(Escuela escuela)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO Escuelas (Nombre, Numero, Direccion)
                VALUES (@Nombre, @Numero, @Direccion);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, escuela);
        }
    }

    public int Baja(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            // Ojo: Deberías borrar en cascada (o manejar las fotos y alumnos)
            var sql = "DELETE FROM Escuelas WHERE EscuelaID = @Id";
            return connection.Execute(sql, new { Id = id });
        }
    }

    public int Modificar(Escuela escuela)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                UPDATE Escuelas SET 
                Nombre = @Nombre, 
                Numero = @Numero, 
                Direccion = @Direccion
                WHERE EscuelaID = @EscuelaID";
            return connection.Execute(sql, escuela);
        }
    }

    public Escuela? ObtenerPorId(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Escuelas WHERE EscuelaID = @Id";
            return connection.QueryFirstOrDefault<Escuela>(sql, new { Id = id });
        }
    }

    public IEnumerable<Escuela> ObtenerTodos()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Escuelas";
            return connection.Query<Escuela>(sql);
        }
    }

    public IEnumerable<Escuela> BuscarPorNombre(string nombre)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Escuelas WHERE Nombre LIKE @Nombre LIMIT 10";
            return connection.Query<Escuela>(sql, new { Nombre = $"%{nombre}%" });
        }
    }
}