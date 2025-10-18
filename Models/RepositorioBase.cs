using MySql.Data.MySqlClient;

public class RepositorioBase
{
    protected readonly string connectionString;

    public RepositorioBase(string connectionString)
    {
        this.connectionString = connectionString;
    }
}