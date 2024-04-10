using HabitLogger;

// -> only have one table for habits and one for operations? (much more efficient on sacale) -> redo project
// + create two tables at start
// + auto create some habits
// + create option to choose a habit (by id)
// + create option for new habit
// + add id column to operations (so that each op has unique id, so I can delete one)
// + fix methods (insert+, view+, delete+, update+)
// + make methods GetHabitName() and GetMeasurement()
// + Auto generate a few habits+ and insert a hundred records with randomly generated values+
// + make a separate class for methods
// + get date from user
// + add date validation
// + add random dates for generated habits
// + a report functionality where users can view specific information
// + make sure id is selected correctly
// + fix bug: can delete/update records from other habits+, can enter negative measurement numbers+
// - viewRecords() sometimes prints to console multiple times, but doesn't happen when debugging. WHY?
// - create a read me file

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
        else
        {
            Console.WriteLine("Database already exist!\n");
        }

        int habitId;

        while (true)
        {
            Console.WriteLine("Choose a habit or create new:");
            Console.WriteLine("0. Create a new habit");

            methods.ShowAllHabits();

            string input2 = Console.ReadLine();

            if (int.TryParse(input2, out int inputId))
            {
                if (inputId == 0)
                {
                    habitId = methods.CreateHabit();
                    break;
                }
                else
                {
                    if (methods.ValidId(inputId))
                    {
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

        string input = "";

        while (input != "0")
        {
            input = methods.ShowMainMenu(habitId);

            if (input != "0" && input != "1" && input != "2" && input != "3" && input != "4" && input != "5")
                Console.WriteLine("Wrong input!");

            switch (input)
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