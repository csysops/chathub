using Microsoft.Data.Sqlite;

public class UserRepository
{
    private const string ConnectionString = "Data Source=chat.db";

    public bool CreateUser(string username, string password)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
            @"
                INSERT INTO Users (Username, Password)
                VALUES ($u, $p)
            ";
            cmd.Parameters.AddWithValue("$u", username);
            cmd.Parameters.AddWithValue("$p", password);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidateUser(string username, string password)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText =
        @"
            SELECT COUNT(*)
            FROM Users
            WHERE Username = $u AND Password = $p
        ";
        cmd.Parameters.AddWithValue("$u", username);
        cmd.Parameters.AddWithValue("$p", password);

        long count = (long)cmd.ExecuteScalar();
        return count == 1;
    }

    public List<string> GetAllUsers()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Username FROM Users";

        List<string> users = new List<string>();

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(reader.GetString(0));
        }

        return users;
    }
}
