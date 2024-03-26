using Microsoft.Data.Sqlite;

// +? create a sqlite database on start (if it doesn't exist) + create table
// show the user a menu of options
// users can insert, delete, update and view their logged habit

internal class Program
{
    private static void Main(string[] args)
    {
        string connectionSource = @"Data Source=HabitLogger.db";

        using (var connection = new SqliteConnection(connectionSource))
        {
            connection.Open();
            var tableCommand = connection.CreateCommand();

            tableCommand.CommandText = @"
        CREATE TABLE IF NOT EXISTS water_glasses ( 
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            date DATE NOT NULL
        );";
            Console.WriteLine($"Command text: {tableCommand.CommandText}");

            tableCommand.ExecuteNonQuery();
            connection.Close();
        }
    }

    static void ShowMainMenu()
    {
        Console.WriteLine("MAIN MENU\n");
        Console.WriteLine("What would you like to do?\n");
        Console.WriteLine("Type 0 to close");
        Console.WriteLine("Type 1 to View all records");
        Console.WriteLine("Type 2 to Insert record");
        Console.WriteLine("Type 3 to Delete record");
        Console.WriteLine("Type 4 to Update record");
        Console.WriteLine("---------------------------------------");
    }

}