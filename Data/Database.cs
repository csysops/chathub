using Microsoft.Data.Sqlite;

public static class Database
{
    public static void Initialize()
    {
        using var connection = new SqliteConnection("Data Source=chat.db");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Messages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Sender TEXT NOT NULL,
                Receiver TEXT NOT NULL,
                Message TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
    }
}
