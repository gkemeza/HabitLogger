using Microsoft.Data.Sqlite;

// + create a sqlite database on start (if it doesn't exist) and create a table
// + show the user a menu of options
// get date from user
// users can insert~, delete, update and view+ their logged habit
// fix all errors

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
            date DATE,
            count INTEGER
        );";

            Console.WriteLine($"Command text: {tableCommand.CommandText}");

            tableCommand.ExecuteNonQuery();
            connection.Close();
        }

        string input = "";

        while (input != "0")
        {
            input = ShowMainMenu();

            switch (input)
            {
                case "0":
                    Console.WriteLine("Exiting...");
                    break;
                case "1":
                    ViewRecords();
                    break;
                case "2":
                    InsertRecord();
                    break;
                case "3":
                    //DeleteRecord();
                    break;
                case "4":
                    //UpdateRecord();
                    break;
            }
        }
    }

    void ConnectDatabase()
    {
        string connectionSource = @"Data Source=HabitLogger.db";

        using (var connection = new SqliteConnection(connectionSource))
        {
            connection.Open();

            connection.Close();
        }
    }

    static string ShowMainMenu()
    {
        Console.WriteLine("\nMAIN MENU\n");
        Console.WriteLine("What would you like to do?\n");
        Console.WriteLine("Type 0 to close");
        Console.WriteLine("Type 1 to View all records");
        Console.WriteLine("Type 2 to Insert record");
        Console.WriteLine("Type 3 to Delete record");
        Console.WriteLine("Type 4 to Update record");
        Console.WriteLine("---------------------------------------");

        return Console.ReadLine();
    }

    static void ViewRecords()
    {
        string connectionSource = @"Data Source=HabitLogger.db";

        using (var connection = new SqliteConnection(connectionSource))
        {
            connection.Open();
            var tableCommand = connection.CreateCommand();

            tableCommand.CommandText = "SELECT * FROM water_glasses";

            using (var reader = tableCommand.ExecuteReader())
            {
                Console.WriteLine("Water glasses each day:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetInt32(2)} glasses");
                }
            }

            connection.Close();
        }
    }

    static void InsertRecord()
    {
        string connectionSource = @"Data Source=HabitLogger.db";

        using (var connection = new SqliteConnection(connectionSource))
        {
            connection.Open();
            var tableCommand = connection.CreateCommand();

            Console.WriteLine("Enter the number of glasses consumed:");

            while (true)
            {
                string input = Console.ReadLine();

                if (int.TryParse(input, out int number))
                {
                    tableCommand.CommandText = $"INSERT INTO water_glasses (count) VALUES ({number});";
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong input. Enter a whole number:");
                }
            }

            tableCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
}