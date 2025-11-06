using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioFotoEscuela : RepositorioBase, IRepositorioFotoEscuela
{
    public RepositorioFotoEscuela(string connectionString) : base(connectionString) { }

    public int Alta(FotoEscuela foto)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO fotosescuela (FotoURL, Descripcion, EscuelaID)
                VALUES (@FotoURL, @Descripcion, @EscuelaID);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@FotoURL", foto.FotoURL ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Descripcion", foto.Descripcion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EscuelaID", foto.EscuelaID);
                id = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
        }
        return id;
    }

    public int Baja(int id)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "DELETE FROM fotosescuela WHERE FotoID = @Id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                rows = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return rows;
    }

    public FotoEscuela? ObtenerPorId(int id)
    {
        FotoEscuela? f = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT FotoID, FotoURL, Descripcion, EscuelaID FROM fotosescuela WHERE FotoID = @Id LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        f = MapearFoto(reader);
                    }
                }
            }
            connection.Close();
        }
        return f;
    }

    public IEnumerable<FotoEscuela> ObtenerPorEscuelaId(int escuelaId)
    {
        var lista = new List<FotoEscuela>();
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT FotoID, FotoURL, Descripcion, EscuelaID FROM fotosescuela WHERE EscuelaID = @EscuelaID";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@EscuelaID", escuelaId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearFoto(reader));
                    }
                }
            }
            connection.Close();
        }
        return lista;
    }

    private FotoEscuela MapearFoto(IDataRecord r)
    {
        return new FotoEscuela
        {
            FotoID = r["FotoID"] == DBNull.Value ? 0 : Convert.ToInt32(r["FotoID"]),
            FotoURL = r["FotoURL"] == DBNull.Value ? null : r["FotoURL"].ToString(),
            Descripcion = r["Descripcion"] == DBNull.Value ? null : r["Descripcion"].ToString(),
            EscuelaID = r["EscuelaID"] == DBNull.Value ? 0 : Convert.ToInt32(r["EscuelaID"])
        };
    }
}