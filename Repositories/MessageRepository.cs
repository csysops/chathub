using Microsoft.Data.Sqlite;

public class MessageRepository
{
    private const string ConnectionString = "Data Source=chat.db";

    public void SaveMessage(string sender, string receiver, string message)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText =
        @"
            INSERT INTO Messages (Sender, Receiver, Message, Timestamp)
            VALUES ($s, $r, $m, $t)
        ";

        cmd.Parameters.AddWithValue("$s", sender);
        cmd.Parameters.AddWithValue("$r", receiver);
        cmd.Parameters.AddWithValue("$m", message);
        cmd.Parameters.AddWithValue("$t", DateTime.UtcNow.ToString("o"));

        cmd.ExecuteNonQuery();
    }

    public List<ChatMessage> LoadHistory(string userA, string userB)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText =
        @"
            SELECT Id, Sender, Receiver, Message, Timestamp
            FROM Messages
            WHERE (Sender = $a AND Receiver = $b)
               OR (Sender = $b AND Receiver = $a)
            ORDER BY Id ASC
        ";

        cmd.Parameters.AddWithValue("$a", userA);
        cmd.Parameters.AddWithValue("$b", userB);

        var list = new List<ChatMessage>();
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new ChatMessage
            {
                Id = reader.GetInt32(0),
                Sender = reader.GetString(1),
                Receiver = reader.GetString(2),
                Message = reader.GetString(3),
                Timestamp = reader.GetString(4)
            });
        }

        return list;
    }
}
