using Microsoft.Data.Sqlite;

// -> only have one table for habits and one for operations? (much more efficient on sacale) -> redo project
// + create two tables at start
// + auto create some habbits
// + create option to choose a habbit (by id)
// + create option for new habbit
// + add id column to operations (so that each op has unique id, so I can delete one)
// + fix methods (insert+, view+, delete+, update+)
// + make methods GetHabitName() and GetMeasurement()
// - Seed Data into the database automatically when the database gets created for the first time,
//      (generating a few habits and inserting a hundred records with randomly generated values)
// - get date from user (in Insert method?)
// - make sure id is selected correctly
// - a report functionality where users can view specific information
// - make a separate class for methods?
// - fix all errors
// - create a read me file

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
                    Console.WriteLine("Choose a name for your habit:");
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
                Console.WriteLine("Wrong input (enter a whole number).\n");
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
                    DeleteRecord(habitId);
                    break;
                case "4":
                    UpdateRecord(habitId);
                    break;
            }
        }

        static string ShowMainMenu(int habitId)
        {
            string habitName = GetHabitName(habitId);

            Console.WriteLine($"\nMAIN MENU ({habitName})\n");
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
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
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

        static void ViewRecords(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string habitName = GetHabitName(id);
            string habbitMeasurement = GetMeasurement(id);

            string connectionSource = "Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {operationsTable} WHERE habbitID='{id}'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    Console.WriteLine($"Records from {habitName}:");

                    while (reader.Read())
                    {
                        Console.Write($"{reader.GetInt32(0)}. ");
                        Console.WriteLine($"{reader.GetInt32(3)} {habbitMeasurement}");
                    }
                }
            }
        }

        static void InsertRecord(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string connectionSource = "Data Source=HabitLogger.db";
            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Console.WriteLine($"Enter the number of {habitMeasurement}:");

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
                        Console.WriteLine("Wrong input (enter a whole number):");
                    }
                }

                int rowsDeleted = tableCommand.ExecuteNonQuery();

                if (rowsDeleted == 1)
                {
                    Console.WriteLine($"New record inserted.");
                }
            }
        }

        static void DeleteRecord(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string connectionSource = "Data Source=HabitLogger.db";

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
                        tableCommand.CommandText = $"DELETE FROM {operationsTable} WHERE id = {number};";
                    }
                    else
                    {
                        Console.WriteLine("Wrong input (enter a whole number):");
                    }

                    int rowsDeleted = tableCommand.ExecuteNonQuery();

                    if (rowsDeleted == 0)
                    {
                        Console.WriteLine($"ID {number} doesn't exist (view records to get ids).");
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

        static void UpdateRecord(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string connectionSource = "Data Source=HabitLogger.db";
            string habitMeasurement = GetMeasurement(id);

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
                        Console.WriteLine($"Choose a new number of {habitMeasurement}: ");
                        string input2 = Console.ReadLine();

                        if (int.TryParse(input2, out int newNumber))
                        {
                            tableCommand.CommandText = $"UPDATE {operationsTable} SET measurement = {newNumber} WHERE id = {number};";
                        }
                        else
                        {
                            Console.WriteLine("Wrong input (enter a whole number).");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong input id (view records to get ids)");
                        break;
                    }

                    int rowsDeleted = tableCommand.ExecuteNonQuery();

                    if (rowsDeleted == 0)
                    {
                        Console.WriteLine($"ID {number} doesn't exist. (view records to get ids)");
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

        static string GetHabitName(int id)
        {
            string connectionSource = "Data Source=HabitLogger.db";
            string habitName;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {habitsTable} WHERE id='{id}'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    reader.Read();
                    habitName = reader.GetString(1);
                }
            }
            return habitName;
        }

        static string GetMeasurement(int id)
        {
            string connectionSource = "Data Source=HabitLogger.db";
            string habitMeasurement;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {habitsTable} WHERE id='{id}'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    reader.Read();
                    habitMeasurement = reader.GetString(2);
                }
            }
            return habitMeasurement;
        }
    }
}