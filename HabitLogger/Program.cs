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
// -> only have one table for habits and one for operations? (much more efficient on sacale) -> redo whole project
// + create two tables at start
// + auto create some habbits
// + create option to choose a habbit (by id)
// + create option for new habbit
// - fix methods (insert+, view+, delete-, update-)
// - make sure id is selected correctly
// - make separate classes for methods

internal class Program
{
    private static void Main(string[] args)
    {
        const string habitsTable = "Habits";
        const string operationsTable = "Operations";

        string habitName;
        string habitMeasurement;

        if (!File.Exists("HabitLogger.db"))
        {
            CreateTwoTables();

            GenerateHabits();
        }
        else
        {
            Console.WriteLine("Database already exist!\n");
            //SelectNameAndMeasurement(out habbitName, out habbitMeasurement);
        }

        int habitId = -1;

        while (true)
        {
            Console.WriteLine("Choose a habit or create new:");
            Console.WriteLine("0. Create a new habit");

            ShowAllHabits();

            string input2 = Console.ReadLine();

            if (int.TryParse(input2, out int result))
            {
                if (result == 0)
                {
                    Console.WriteLine("Choose a name for your habit (no spaces):");
                    habitName = Console.ReadLine();

                    Console.WriteLine("Choose a unit of measurement for your habit (measured by quantity):");
                    habitMeasurement = Console.ReadLine();

                    string connectionSource = @"Data Source=HabitLogger.db";

                    using (var connection = new SqliteConnection(connectionSource))
                    {
                        connection.Open();
                        var tableCommand = connection.CreateCommand();

                        tableCommand.CommandText = $"INSERT INTO {habitsTable} (habbitName, measurementName) VALUES ('{habitName}', '{habitMeasurement}');";
                        tableCommand.ExecuteNonQuery();

                        tableCommand.CommandText = $"SELECT * FROM {habitsTable} ORDER by id DESC";

                        using (var reader = tableCommand.ExecuteReader())
                        {
                            reader.Read();
                            habitId = reader.GetInt32(0);
                            Console.WriteLine(reader.GetInt32(0));
                        }
                    }
                }
                else
                {
                    habitId = result;
                }
                break;
            }
            else
            {
                Console.WriteLine("Wrong input (enter a whole number)\n");
            }
        }

        string input = "";

        while (input != "0")
        {
            input = ShowMainMenu(habitId);

            if (input != "0" && input != "1" && input != "2" && input != "3" && input != "4")
                Console.WriteLine("Wrong input!");

            switch (input)
            {
                case "0":
                    Console.WriteLine("Exiting...");
                    break;
                case "1":
                    ViewRecords(habitId);
                    break;
                case "2":
                    InsertRecord(habitId);
                    break;
                case "3":
                    //DeleteRecord(habbitName, habbitMeasurement);
                    break;
                case "4":
                    //UpdateRecord(habbitName, habbitMeasurement);
                    break;
            }
        }

        static string ShowMainMenu(int habitId)
        {
            Console.WriteLine($"\nMAIN MENU ({habitId})\n");
            Console.WriteLine("What would you like to do?\n");
            Console.WriteLine("Type 0 to Close");
            Console.WriteLine("Type 1 to View all records");
            Console.WriteLine("Type 2 to Insert record");
            Console.WriteLine("Type 3 to Delete record");
            Console.WriteLine("Type 4 to Update record");
            Console.WriteLine("---------------------------------------");

            return Console.ReadLine();
        }

        static void CreateTwoTables()
        {
            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = @$"
                        CREATE TABLE IF NOT EXISTS Habits ( 
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            habbitName TEXT,
                            measurementName TEXT
                        );";

                try
                {
                    tableCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = @$"
                        CREATE TABLE IF NOT EXISTS Operations ( 
                            habbitID INTEGER,
                            date DATE,
                            measurement INTEGER
                        );";

                try
                {
                    tableCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static void GenerateHabits()
        {
            string habbitName = "Exercise";
            string habbitMeasurement = "Squats";

            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"INSERT INTO {habitsTable} (habbitName, measurementName) VALUES ('{habbitName}', '{habbitMeasurement}');";
                tableCommand.ExecuteNonQuery();

                habbitName = "Cycling";
                habbitMeasurement = "Kilometers";

                tableCommand.CommandText = $"INSERT INTO {habitsTable} (habbitName, measurementName) VALUES ('{habbitName}', '{habbitMeasurement}');";
                tableCommand.ExecuteNonQuery();
            }
        }

        static void ShowAllHabits()
        {
            string connectionSource = "Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {habitsTable}";

                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.Write(reader.GetString(0) + ". ");
                        Console.Write(reader.GetString(1));
                        Console.WriteLine();
                    }
                }
            }
        }

        static void SelectNameAndMeasurement(out string habbitName, out string habbitMeasurement)
        {
            habbitName = "";
            habbitMeasurement = "";

            string connectionSource = "Data Source=HabitLogger.db";

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
            }
        }

        static void ViewRecords(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string habbitName = "";
            string habbitMeasurement = "";

            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                // make this using into method?
                tableCommand.CommandText = $"SELECT * FROM {habitsTable} WHERE id='{id}'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    reader.Read();
                    habbitName = (string)reader.GetValue(1);
                    habbitMeasurement = (string)reader.GetValue(2);
                }

                tableCommand.CommandText = $"SELECT * FROM {operationsTable} WHERE habbitID='{id}'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    Console.WriteLine($"Records from {habbitName}:");
                    int counter = 1;
                    while (reader.Read())
                    {
                        Console.Write($"{counter++}. ");
                        Console.WriteLine($"{reader.GetInt32(2)} {habbitMeasurement}");
                    }
                }
            }
        }

        static void InsertRecord(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string connectionSource = @"Data Source=HabitLogger.db";
            string habbitMeasurement;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {habitsTable} WHERE id='{id}'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    reader.Read();
                    habbitMeasurement = reader.GetString(2);
                }

                Console.WriteLine($"Enter the number of {habbitMeasurement}:");

                while (true)
                {
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number))
                    {
                        tableCommand.CommandText = $"INSERT INTO {operationsTable} (habbitID, measurement) VALUES ({id}, {number});";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Wrong input. Enter a whole number:");
                    }
                }

                tableCommand.ExecuteNonQuery();
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
            }
        }
    }
}