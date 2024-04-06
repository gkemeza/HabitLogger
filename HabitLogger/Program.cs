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
// - a report functionality where users can view specific information
// - fix bug: can delete/update records from other habits
// - make sure id is selected correctly
// - fix all errors
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

            if (int.TryParse(input2, out int result))
            {
                if (result == 0)
                {
                    habitId = methods.CreateHabit();
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
            input = methods.ShowMainMenu(habitId);

            if (input != "0" && input != "1" && input != "2" && input != "3" && input != "4")
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
            }
        }
    }
}