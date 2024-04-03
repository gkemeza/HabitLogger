using Microsoft.Data.Sqlite;

// + create a sqlite database on start (if it doesn't exist) and create a table
// + show the user a menu of options
// + users can insert~, delete+, update+ and view+ their logged habit
// + users can create their own habits+, choose the unit of measurement of each habit+
// + fix bug: can't get table name if no records! (tereikejo vietoj sqlite_sequence querinti duomenis is sqlite_master)
// - Seed Data into the database automatically when the database gets created for the first time,
//      (generating a few habits and inserting a hundred records with randomly generated values)
// - get date from user (in Insert method?)
// - a report functionality where the users can view specific information
// - fix all errors
// - create a read me file
// ? only have one table for habits and one for operations? (much more efficient on sacale) -> redo whole project?

internal class Program
{
    private static void Main(string[] args)
    {
        string habbitName = "";
        string habbitMeasurement = "";

        if (!File.Exists("HabitLogger.db"))
        {
            while (true)
            {
                Console.WriteLine("Choose a name for your habit (no spaces):");
                habbitName = Console.ReadLine();

                Console.WriteLine("Choose a unit of measurement for your habit (measured by quantity):");
                habbitMeasurement = Console.ReadLine();

                string connectionSource = @"Data Source=HabitLogger.db";

                using (var connection = new SqliteConnection(connectionSource))
                {
                    connection.Open();
                    var tableCommand = connection.CreateCommand();

                    tableCommand.CommandText = @$"
                        CREATE TABLE IF NOT EXISTS {habbitName} ( 
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            date DATE,
                            {habbitMeasurement} INTEGER
                        );";

                    //Console.WriteLine($"Command text: {tableCommand.CommandText}");

                    try
                    {
                        tableCommand.ExecuteNonQuery();
                        connection.Close();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Invalid name/measurement!\n");
                    }
                }
            }
        }
        else
        {
            SelectNameAndMeasurement(out habbitName, out habbitMeasurement);
        }

        string input = "";

        while (input != "0")
        {
            input = ShowMainMenu();

            if (input != "0" && input != "1" && input != "2" && input != "3" && input != "4")
                Console.WriteLine("Wrong input!");

            switch (input)
            {
                case "0":
                    Console.WriteLine("Exiting...");
                    break;
                case "1":
                    ViewRecords(habbitName, habbitMeasurement);
                    break;
                case "2":
                    InsertRecord(habbitName, habbitMeasurement);
                    break;
                case "3":
                    DeleteRecord(habbitName, habbitMeasurement);
                    break;
                case "4":
                    UpdateRecord(habbitName, habbitMeasurement);
                    break;
            }
        }

        static string ShowMainMenu()
        {
            Console.WriteLine("\nMAIN MENU\n");
            Console.WriteLine("What would you like to do?\n");
            Console.WriteLine("Type 0 to Close");
            Console.WriteLine("Type 1 to View all records");
            Console.WriteLine("Type 2 to Insert record");
            Console.WriteLine("Type 3 to Delete record");
            Console.WriteLine("Type 4 to Update record");
            Console.WriteLine("---------------------------------------");

            return Console.ReadLine();
        }

        static void SelectNameAndMeasurement(out string habbitName, out string habbitMeasurement)
        {
            habbitName = "";
            habbitMeasurement = "";

            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT name FROM sqlite_master";

                Console.WriteLine($"Command text: {tableCommand.CommandText}");

                using (var reader = tableCommand.ExecuteReader())
                {
                    reader.Read();
                    habbitName = reader.GetString(0);
                    Console.WriteLine(habbitName);
                }

                tableCommand.CommandText = $"SELECT * FROM {habbitName}";

                Console.WriteLine($"Command text: {tableCommand.CommandText}");

                using (var reader = tableCommand.ExecuteReader())
                {
                    habbitMeasurement = reader.GetName(2);
                    Console.WriteLine(habbitMeasurement);
                }

                connection.Close();
            }
        }

        static void ViewRecords(string habbitName, string habbitMeasurement)
        {
            Console.Clear();
            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {habbitName}";

                using (var reader = tableCommand.ExecuteReader())
                {
                    Console.WriteLine($"Records from {habbitName}:");
                    while (reader.Read())
                    {
                        Console.Write($"#{reader.GetInt32(0)} => ");
                        Console.WriteLine($"{reader.GetInt32(2)} {habbitMeasurement}");
                    }
                }

                connection.Close();
            }
        }

        static void InsertRecord(string habbitName, string habbitMeasurement)
        {
            Console.Clear();
            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Console.WriteLine($"Enter the number of {habbitMeasurement}:");

                while (true)
                {
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number))
                    {
                        tableCommand.CommandText = $"INSERT INTO {habbitName} ({habbitMeasurement}) VALUES ({number});";
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

        static void DeleteRecord(string habbitName, string habbitMeasurement)
        {
            Console.Clear();
            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Console.WriteLine("Choose id to delete:");

                while (true)
                {
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number))
                    {
                        tableCommand.CommandText = $"DELETE FROM {habbitName} WHERE id = {number};";
                    }
                    else
                    {
                        Console.WriteLine("Wrong input. Enter a whole number:");
                    }

                    int rowsDeleted = tableCommand.ExecuteNonQuery();

                    if (rowsDeleted == 0)
                    {
                        Console.WriteLine($"ID {number} doesn't exist. View records to get ids");
                        break;
                    }
                    else if (rowsDeleted == -1)
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Record ID {number} deleted");
                        break;
                    }
                }

                connection.Close();
            }
        }

        static void UpdateRecord(string habbitName, string habbitMeasurement)
        {
            Console.Clear();
            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                while (true)
                {
                    Console.WriteLine("Choose id to update:");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number))
                    {
                        Console.WriteLine($"Choose a new number of {habbitMeasurement}: ");
                        string input2 = Console.ReadLine();

                        if (int.TryParse(input2, out int newNumber))
                        {
                            tableCommand.CommandText = $"UPDATE {habbitName} SET {habbitMeasurement} = {newNumber} WHERE id = {number};";
                        }
                        else
                        {
                            Console.WriteLine("Wrong input number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong input id");
                    }

                    int rowsDeleted = tableCommand.ExecuteNonQuery();

                    if (rowsDeleted == 0)
                    {
                        Console.WriteLine($"ID {number} doesn't exist. View records to get ids");
                        break;
                    }
                    else if (rowsDeleted == -1)
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Record ID {number} updated");
                        break;
                    }
                }

                connection.Close();
            }
        }
    }
}