using Dapper;
using MySql.Data.MySqlClient;

public class RepositorioVacuna : RepositorioBase, IRepositorioVacuna
{
    public RepositorioVacuna(string connectionString) : base(connectionString) { }

    public int Alta(Vacuna vacuna)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO Vacunas (NombreVacuna, Descripcion)
                VALUES (@NombreVacuna, @Descripcion);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, vacuna);
        }
    }

    public int Baja(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "DELETE FROM Vacunas WHERE VacunaID = @Id";
            return connection.Execute(sql, new { Id = id });
        }
    }

    public int Modificar(Vacuna vacuna)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                UPDATE Vacunas SET 
                NombreVacuna = @NombreVacuna, 
                Descripcion = @Descripcion
                WHERE VacunaID = @VacunaID";
            return connection.Execute(sql, vacuna);
        }
    }

    public Vacuna? ObtenerPorId(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Vacunas WHERE VacunaID = @Id";
            return connection.QueryFirstOrDefault<Vacuna>(sql, new { Id = id });
        }
    }

    public IEnumerable<Vacuna> ObtenerTodos()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Vacunas ORDER BY NombreVacuna";
            return connection.Query<Vacuna>(sql);
        }
    }
}