**Habit Logger**

- This is my first C# database console application.
- The app tracks habits using basic CRUD operations.
- Developed using ADO.NET, SQLite and Visual Studio.

**Requirements:**

1) The habits can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day).
2) When the application starts, it should create a sqlite database, if one isn’t present.
3) It should also create a table in the database, where the habit will be logged.
4) Show the user a menu of options.
5) The users should be able to insert, delete, update and view their logged habit.
6) Handle all possible errors so that the application never crashes.
7) The application should only be terminated when the user inserts 0.
8) Only interact with the database using raw SQL. Don't use mappers such as Entity Framework.
9) Let the users create their own habits to track. That will require that you let them choose the unit of measurement of each habit.
10) Seed Data into the database automatically when the database gets created for the first time, generating a few habits and inserting a hundred records with randomly generated values.
11) Create a report functionality where the users can view specific information.

**Features:**

- SQLite database connection
  - The program uses a SQLite db connection to store and read information.
  - If no database exists, it will be created on start.
    
- A console based UI where users can navigate by key presses
  - (2 images)
    
- CRUD DB functions
  - First menu asks to choose a habit or create new.
  - In the second menu users can Create, Read, Update or Delete entries for specific date, entered in their local format.
  - Wrong inputs will output a helpful message for the user.
 
- Basic Reports of the chosen habit data
  - (1 image)
