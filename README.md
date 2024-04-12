**Habit Logger**

This is my first C# database console application.
The app tracks habits using basic CRUD operations.
Developed using ADO.NET, SQLite and Visual Studio.

**Requirements:**

1) The habits can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day).
3) When the application starts, it should create a sqlite database, if one isn’t present.
4) It should also create a table in the database, where the habit will be logged.
5) Show the user a menu of options.
6) The users should be able to insert, delete, update and view their logged habit.
7) Handle all possible errors so that the application never crashes.
8) The application should only be terminated when the user inserts 0.
9) Only interact with the database using raw SQL. Don't use mappers such as Entity Framework.
10) Let the users create their own habits to track. That will require that you let them choose the unit of measurement of each habit.
11) Seed Data into the database automatically when the database gets created for the first time, generating a few habits and inserting a hundred records with randomly generated values.
12) Create a report functionality where the users can view specific information.
