﻿using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitLogger
{
    internal class Methods
    {
        Program program = new Program();

        string connectionSource = "Data Source=HabitLogger.db";

        public void CreateTwoTables()
        {
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

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"INSERT INTO {program.habitsTable} (habitName, measurementName) VALUES ({"@0"}, {"@1"})";

                tableCommand.Parameters.AddWithValue("@0", habitName);
                tableCommand.Parameters.AddWithValue("@1", habitMeasurement);

                tableCommand.ExecuteNonQuery();

                tableCommand.Parameters.Clear();

                habitName = "Cycling";
                habitMeasurement = "Kilometers";

                tableCommand.CommandText = $"INSERT INTO {program.habitsTable} (habitName, measurementName) VALUES ({"@0"}, {"@1"})";

                tableCommand.Parameters.AddWithValue("@0", habitName);
                tableCommand.Parameters.AddWithValue("@1", habitMeasurement);

                tableCommand.ExecuteNonQuery();
            }
        }

        public void InsertRandomRecords(int id)
        {
            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Random rand = new Random();
                int counter = 0;
                int daysRange = 5 * 365;

                while (counter < 100)
                {
                    int number = rand.Next(101);
                    string date = DateTime.Today.AddDays(-rand.Next(daysRange)).ToShortDateString();

                    tableCommand.CommandText = $"INSERT INTO {program.operationsTable} (habitID, date, measurement) VALUES ({"@0"}, {"@1"}, {"@2"})";

                    tableCommand.Parameters.Clear();
                    tableCommand.Parameters.AddWithValue("@0", id);
                    tableCommand.Parameters.AddWithValue("@1", date);
                    tableCommand.Parameters.AddWithValue("@2", number);

                    tableCommand.ExecuteNonQuery();
                    counter++;
                }
            }
        }

        public void ShowAllHabits()
        {
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
                    Console.WriteLine("-----------------------------------");
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

            Console.WriteLine();

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"INSERT INTO {program.habitsTable} (habitName, measurementName) VALUES ({"@0"}, {"@1"})";

                tableCommand.Parameters.AddWithValue("@0", habitName);
                tableCommand.Parameters.AddWithValue("@1", habitMeasurement);

                tableCommand.ExecuteNonQuery();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable} ORDER by id DESC";

                var reader = tableCommand.ExecuteReader();

                reader.Read();
                habitId = reader.GetInt32(0);
            }

            return habitId;
        }

        public bool ValidId(int idNumber)
        {
            List<int> list = new List<int>();

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable}";

                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetInt32(0));
                    }
                }
            }

            return list.Contains(idNumber);
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
            Console.WriteLine("-----------------------------------");

            return Console.ReadLine();
        }

        public string ShowReportsMenu(int habitId)
        {
            string habitName = GetHabitName(habitId);
            string habitMeasurement = GetMeasurement(habitId);

            Console.WriteLine($"\nREPORTS MENU ({habitName})\n");
            Console.WriteLine("Choose a report to see specific information: ");
            Console.WriteLine($"Type 0 to Leave");
            Console.WriteLine($"Type 1 to see total {habitMeasurement}");
            Console.WriteLine($"Type 2 to see average {habitMeasurement}");
            Console.WriteLine($"Type 3 to see most {habitMeasurement} in one time");
            Console.WriteLine("---------------------------------------");

            return Console.ReadLine();
        }

        public void ViewRecords(int id)
        {
            Console.Clear();

            string habitName = GetHabitName(id);
            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.operationsTable} WHERE habitID = @0";

                tableCommand.Parameters.AddWithValue("@0", id);

                using (var reader = tableCommand.ExecuteReader())
                {
                    Console.WriteLine($"Records from {habitName}:");
                    Console.WriteLine($"ID |   Date    |  Measurement\n");

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
                    string inputMeasurement = Console.ReadLine();

                    if (int.TryParse(inputMeasurement, out int inputMeasurementToInsert))
                    {
                        if (inputMeasurementToInsert >= 0)
                        {
                            tableCommand.CommandText = $@"INSERT INTO {program.operationsTable} (habitID, date, measurement) 
                                                            VALUES ({"@0"}, {"@1"}, {"@2"})";

                            tableCommand.Parameters.AddWithValue("@0", id);
                            tableCommand.Parameters.AddWithValue("@1", date);
                            tableCommand.Parameters.AddWithValue("@2", inputMeasurementToInsert);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid number (enter a positive number):");
                        }
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

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Console.WriteLine("Choose id to delete:");

                while (true)
                {
                    string inputId = Console.ReadLine();

                    if (int.TryParse(inputId, out int inputIdToDelete))
                    {
                        tableCommand.CommandText = $"DELETE FROM {program.operationsTable} WHERE id = {"@0"} AND habitId = {"@1"}";

                        tableCommand.Parameters.AddWithValue("@0", inputIdToDelete);
                        tableCommand.Parameters.AddWithValue("@1", id);
                    }
                    else
                    {
                        Console.WriteLine("Wrong input (enter a whole number):");
                    }

                    int rowsDeleted = tableCommand.ExecuteNonQuery();

                    if (rowsDeleted == 0)
                    {
                        Console.WriteLine($"ID {inputIdToDelete} doesn't exist (view records to get ids).");
                        break;
                    }
                    else if (rowsDeleted == 1)
                    {
                        Console.WriteLine($"Record ID {inputIdToDelete} deleted");
                        break;
                    }
                }
            }
        }

        public void UpdateRecord(int id)
        {
            Console.Clear();

            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                Console.WriteLine("Choose id to update:");
                string inputId = Console.ReadLine();

                if (int.TryParse(inputId, out int inputIdToUpdate))
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

                    while (true)
                    {
                        Console.WriteLine($"Choose a new number of {habitMeasurement}: ");
                        string inputMeasurement = Console.ReadLine();

                        if (int.TryParse(inputMeasurement, out int inputMeasurementToUpdate))
                        {
                            if (inputMeasurementToUpdate >= 0)
                            {
                                tableCommand.CommandText = @$"UPDATE {program.operationsTable} SET date = {"@0"}, measurement = {"@1"} 
                                                WHERE id = {"@2"} AND habitId = {"@3"}";

                                tableCommand.Parameters.AddWithValue("@0", date);
                                tableCommand.Parameters.AddWithValue("@1", inputMeasurementToUpdate);
                                tableCommand.Parameters.AddWithValue("@2", inputIdToUpdate);
                                tableCommand.Parameters.AddWithValue("@3", id);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Wrong input (enter a positive number)");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input (enter a whole number)");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Wrong input id (id is a whole number)");
                }

                int rowsUpdated = tableCommand.ExecuteNonQuery();

                if (rowsUpdated == 0)
                {
                    Console.WriteLine($"ID {inputIdToUpdate} doesn't exist. (view records to get ids)");
                }
                else if (rowsUpdated == 1)
                {
                    Console.WriteLine($"Record ID {inputIdToUpdate} updated");
                }
            }
        }

        public bool OperationsAvailable(int id)
        {
            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.operationsTable} WHERE habitID = {"@0"}";

                tableCommand.Parameters.AddWithValue("@0", id);

                using (var reader = tableCommand.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public void ViewReports(int id)
        {
            Console.Clear();

            if (!OperationsAvailable(id))
            {
                Console.WriteLine("There are no data in this habit (insert records first).");
                return;
            }

            string inputReportChoice = "";

            while (inputReportChoice != "0")
            {
                inputReportChoice = ShowReportsMenu(id);

                if (new[] { "0", "1", "2", "3" }.Contains(inputReportChoice))
                {
                    Console.WriteLine("Wrong input!");
                }

                switch (inputReportChoice)
                {
                    case "1":
                        Total(id);
                        break;
                    case "2":
                        Average(id);
                        break;
                    case "3":
                        MostAtOnce(id);
                        break;
                }
            }

            Console.Clear();
        }

        public void Total(int id)
        {
            Console.Clear();

            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT SUM (measurement) FROM Operations WHERE habitID = {"@0"}";

                tableCommand.Parameters.AddWithValue("@0", id);

                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Total {habitMeasurement}: {reader.GetInt32(0)}");
                    }
                }
            }
        }

        public void Average(int id)
        {
            Console.Clear();

            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT AVG (measurement) FROM Operations WHERE habitID = {"@0"}";

                tableCommand.Parameters.AddWithValue("@0", id);

                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Average {habitMeasurement}: {Math.Round(reader.GetDouble(0))}");
                    }
                }
            }
        }

        public void MostAtOnce(int id)
        {
            Console.Clear();

            string habitMeasurement = GetMeasurement(id);

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT MAX (measurement) FROM Operations WHERE habitID = {"@0"}";

                tableCommand.Parameters.AddWithValue("@0", id);

                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Most {habitMeasurement} in one time: {reader.GetInt32(0)}");
                    }
                }
            }
        }

        string GetHabitName(int id)
        {
            string habitName;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable} WHERE id = {"@0"}";

                tableCommand.Parameters.AddWithValue("@0", id);

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
            string habitMeasurement;

            using (var connection = new SqliteConnection(connectionSource))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"SELECT * FROM {program.habitsTable} WHERE id = {"@0"}";

                tableCommand.Parameters.AddWithValue("@0", id);

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