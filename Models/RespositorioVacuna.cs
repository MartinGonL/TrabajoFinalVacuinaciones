using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioVacuna : RepositorioBase, IRepositorioVacuna
{
    public RepositorioVacuna(string connectionString) : base(connectionString) { }

    public int Alta(Vacuna vacuna)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO Vacunas (NombreVacuna, Descripcion)
                VALUES (@NombreVacuna, @Descripcion);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@NombreVacuna", vacuna.NombreVacuna ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Descripcion", vacuna.Descripcion ?? (object)DBNull.Value);
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
            var sql = "DELETE FROM Vacunas WHERE VacunaID = @Id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                rows = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return rows;
    }

    public int Modificar(Vacuna vacuna)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                UPDATE Vacunas SET 
                    NombreVacuna = @NombreVacuna, 
                    Descripcion = @Descripcion
                WHERE VacunaID = @VacunaID";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@NombreVacuna", vacuna.NombreVacuna ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Descripcion", vacuna.Descripcion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@VacunaID", vacuna.VacunaID);
                rows = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return rows;
    }

    public Vacuna? ObtenerPorId(int id)
    {
        Vacuna? v = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT VacunaID, NombreVacuna, Descripcion FROM Vacunas WHERE VacunaID = @Id LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        v = MapearVacuna(reader);
                    }
                }
            }
            connection.Close();
        }
        return v;
    }

    public IEnumerable<Vacuna> ObtenerTodos()
    {
        var lista = new List<Vacuna>();
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT VacunaID, NombreVacuna, Descripcion FROM Vacunas ORDER BY NombreVacuna";
            using (var command = new MySqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(MapearVacuna(reader));
                }
            }
            connection.Close();
        }
        return lista;
    }

    private Vacuna MapearVacuna(IDataRecord r)
    {
        return new Vacuna
        {
            VacunaID = r["VacunaID"] == DBNull.Value ? 0 : Convert.ToInt32(r["VacunaID"]),
            NombreVacuna = r["NombreVacuna"] == DBNull.Value ? null : r["NombreVacuna"].ToString(),
            Descripcion = r["Descripcion"] == DBNull.Value ? null : r["Descripcion"].ToString()
        };
    }
}