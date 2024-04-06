using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitLogger
{
    internal class Methods
    {
        Program program = new Program();

        public void CreateTwoTables()
        {
            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = @$"
                        CREATE TABLE IF NOT EXISTS Habits ( 
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            habitName TEXT,
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
                            habitID INTEGER,
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

        public void GenerateHabits()
        {
            string habitName = "Exercise";
            string habitMeasurement = "Squats";

            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"INSERT INTO {program.habitsTable} (habitName, measurementName) VALUES ('{habitName}', '{habitMeasurement}');";
                tableCommand.ExecuteNonQuery();

                habitName = "Cycling";
                habitMeasurement = "Kilometers";

                tableCommand.CommandText = $"INSERT INTO {program.habitsTable} (habitName, measurementName) VALUES ('{habitName}', '{habitMeasurement}');";
                tableCommand.ExecuteNonQuery();
            }
        }

        public void InsertRandomRecords(int id)
        {
            string connectionSource = "Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                string date = DateTime.Now.ToShortDateString();

                Random rand = new Random();
                int counter = 0;

                while (counter < 100)
                {
                    int number = rand.Next(100);
                    tableCommand.CommandText = $"INSERT INTO {program.operationsTable} (habitID, date, measurement) VALUES ({id}, '{date}', {number});";
                    tableCommand.ExecuteNonQuery();
                    counter++;
                }
            }
        }

        public void ShowAllHabits()
        {
            string connectionSource = "Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable}";

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

        public int CreateHabit()
        {
            string habitName;
            string habitMeasurement;
            int habitId;

            Console.WriteLine("Choose a name for your habit:");
            habitName = Console.ReadLine();

            Console.WriteLine("Choose a unit of measurement for your habit (measured by quantity):");
            habitMeasurement = Console.ReadLine();

            string connectionSource = @"Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"INSERT INTO {program.habitsTable} (habitName, measurementName) VALUES ('{habitName}', '{habitMeasurement}');";
                tableCommand.ExecuteNonQuery();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable} ORDER by id DESC";
                var reader = tableCommand.ExecuteReader();

                reader.Read();
                habitId = reader.GetInt32(0);
            }

            return habitId;
        }

        public string ShowMainMenu(int habitId)
        {
            string habitName = GetHabitName(habitId);

            Console.WriteLine($"\nMAIN MENU ({habitName})\n");
            Console.WriteLine("What would you like to do?\n");
            Console.WriteLine("Type 0 to Close");
            Console.WriteLine("Type 1 to View records");
            Console.WriteLine("Type 2 to Insert record");
            Console.WriteLine("Type 3 to Delete record");
            Console.WriteLine("Type 4 to Update record");
            Console.WriteLine("Type 5 to View reports");
            Console.WriteLine("---------------------------------------");

            return Console.ReadLine();
        }

        public void ViewRecords(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string habitName = GetHabitName(id);
            string habitMeasurement = GetMeasurement(id);

            string connectionSource = "Data Source=HabitLogger.db";

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.operationsTable} WHERE habitID = {id}";

                using (var reader = tableCommand.ExecuteReader())
                {
                    Console.WriteLine($"Records from {habitName}:");

                    while (reader.Read())
                    {
                        Console.Write($"{reader.GetInt32(0)}. ");
                        Console.Write($"{reader.GetString(2)} | ");
                        Console.WriteLine($"{reader.GetInt32(3)} {habitMeasurement}");
                    }
                }
            }
        }

        public void InsertRecord(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string connectionSource = "Data Source=HabitLogger.db";
            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                string systemFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                Console.WriteLine($"Enter the date ({systemFormat}):");

                string date = Console.ReadLine();

                while (!DateTime.TryParseExact(date, systemFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
                {
                    Console.WriteLine("Invalid date!");
                    Console.WriteLine($"Enter the date in correct format ({systemFormat}):");

                    date = Console.ReadLine();
                }

                Console.WriteLine($"Enter the number of {habitMeasurement}:");

                while (true)
                {
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int number))
                    {
                        tableCommand.CommandText = $"INSERT INTO {program.operationsTable} (habitID, date, measurement) VALUES ({id}, '{date}', {number});";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid number (enter a whole number):");
                    }
                }

                int rowsDeleted = tableCommand.ExecuteNonQuery();

                if (rowsDeleted == 1)
                {
                    Console.WriteLine($"New record inserted.");
                }
            }
        }

        public void DeleteRecord(int id)
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
                        tableCommand.CommandText = $"DELETE FROM {program.operationsTable} WHERE id = {number};";
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

        public void UpdateRecord(int id)
        {
            Console.Clear();
            Console.WriteLine($"ID: {id}");

            string connectionSource = "Data Source=HabitLogger.db";
            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Console.WriteLine("Choose id to update:");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int number))
                {
                    string systemFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    Console.WriteLine($"Enter the date ({systemFormat}):");

                    string date = Console.ReadLine();

                    while (!DateTime.TryParseExact(date, systemFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
                    {
                        Console.WriteLine("Invalid date!");
                        Console.WriteLine($"Enter the date in correct format ({systemFormat}):");

                        date = Console.ReadLine();
                    }

                    Console.WriteLine($"Choose a new number of {habitMeasurement}: ");
                    string input2 = Console.ReadLine();

                    while (!int.TryParse(input2, out int _))
                    {
                        Console.WriteLine("Wrong input (enter a whole number):");
                        input2 = Console.ReadLine();
                    }

                    tableCommand.CommandText = $"UPDATE {program.operationsTable} SET date = '{date}', measurement = {input2} WHERE id = {number};";
                }
                else
                {
                    Console.WriteLine("Wrong input id (id is a whole number)");
                }

                int rowsUpdated = tableCommand.ExecuteNonQuery();

                if (rowsUpdated == 0)
                {
                    Console.WriteLine($"ID {number} doesn't exist. (view records to get ids)");
                }
                else if (rowsUpdated == 1)
                {
                    Console.WriteLine($"Record ID {number} updated");
                }
            }
        }

        public void ViewReports(int id)
        {
            string habitMeasurement = GetMeasurement(id);

            Console.WriteLine("Choose a report to see specific information: ");
            Console.WriteLine($"Type 0 to Leave");
            Console.WriteLine($"Type 1 to see total {habitMeasurement}");
            Console.WriteLine($"Type 2 to see total {habitMeasurement}");
            Console.WriteLine("---------------------------------------");
        }

        string GetHabitName(int id)
        {
            string connectionSource = "Data Source=HabitLogger.db";
            string habitName;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable} WHERE id = {id}";

                using (var reader = tableCommand.ExecuteReader())
                {
                    reader.Read();
                    habitName = reader.GetString(1);
                }
            }
            return habitName;
        }

        string GetMeasurement(int id)
        {
            string connectionSource = "Data Source=HabitLogger.db";
            string habitMeasurement;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable} WHERE id = {id}";

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