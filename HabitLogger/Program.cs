using HabitLogger;

internal class Program
{
    public string operationsTable = "Operations";
    public string habitsTable = "Habits";

    private static void Main(string[] args)
    {
        Methods methods = new Methods();

        if (!File.Exists("HabitLogger.db"))
        {
            methods.CreateTwoTables();

            methods.GenerateHabits();

            methods.InsertRandomRecords(1);
            methods.InsertRandomRecords(2);
        }

        int habitId;

        while (true)
        {
            Console.WriteLine("Choose a habit or create new:");
            Console.WriteLine("0. Create a new habit");

            methods.ShowAllHabits();

            string inputIdChoice = Console.ReadLine();

            if (int.TryParse(inputIdChoice, out int inputId))
            {
                if (inputId == 0)
                {
                    Console.Clear();
                    habitId = methods.CreateHabit();
                    break;
                }
                else
                {
                    if (methods.ValidId(inputId))
                    {
                        Console.Clear();
                        habitId = inputId;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid id!\n");
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong input (enter a whole number).\n");
            }
        }

        string inputMenuChoice = "";

        while (inputMenuChoice != "0")
        {
            inputMenuChoice = methods.ShowMainMenu(habitId);

            if (!new[] { "0", "1", "2", "3", "4", "5" }.Contains(inputMenuChoice))
            {
                Console.WriteLine("Wrong input!");
            }

            switch (inputMenuChoice)
            {
                case "0":
                    Console.WriteLine("Exiting...");
                    break;
                case "1":
                    methods.ViewRecords(habitId);
                    break;
                case "2":
                    methods.InsertRecord(habitId);
                    break;
                case "3":
                    methods.DeleteRecord(habitId);
                    break;
                case "4":
                    methods.UpdateRecord(habitId);
                    break;
                case "5":
                    methods.ViewReports(habitId);
                    break;
            }
        }
    }
}