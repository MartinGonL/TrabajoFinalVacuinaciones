using Dapper;
using MySql.Data.MySqlClient;

public class RepositorioFotoEscuela : RepositorioBase, IRepositorioFotoEscuela
{
    public RepositorioFotoEscuela(string connectionString) : base(connectionString) { }

    public int Alta(FotoEscuela foto)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO FotosEscuela (FotoURL, Descripcion, EscuelaID)
                VALUES (@FotoURL, @Descripcion, @EscuelaID);
                SELECT LAST_INSERT_ID();";
            return connection.ExecuteScalar<int>(sql, foto);
        }
    }

    public int Baja(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "DELETE FROM FotosEscuela WHERE FotoID = @Id";
            return connection.Execute(sql, new { Id = id });
        }
    }

    public FotoEscuela? ObtenerPorId(int id)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM FotosEscuela WHERE FotoID = @Id";
            return connection.QueryFirstOrDefault<FotoEscuela>(sql, new { Id = id });
        }
    }

    public IEnumerable<FotoEscuela> ObtenerPorEscuelaId(int escuelaId)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM FotosEscuela WHERE EscuelaID = @EscuelaId";
            return connection.Query<FotoEscuela>(sql, new { EscuelaId = escuelaId });
        }
    }
}