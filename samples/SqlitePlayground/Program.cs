using Microsoft.Data.Sqlite;

Console.WriteLine("Checking if the local DB file is already sitting on disk next to this executable");

if (File.Exists("MyLocalDatabase.db"))
{
    Console.WriteLine("It is, deleting...");
    File.Delete("MyLocalDatabase.db");
}

Console.WriteLine("Opening a connection to a local database instance, which will create the file if it doesn't yet exist");
using (var connection = new SqliteConnection("Data Source=MyLocalDatabase.db"))
{
    connection.Open();

    Console.WriteLine("Creating a table in the database");
    var command = connection.CreateCommand();
    command.CommandText = "CREATE TABLE TestTbl1 (ID INT IDENTITY(1,1), Name NVARCHAR(255))";
    command.ExecuteNonQuery();

    Console.WriteLine("Inserting some data");
    command = connection.CreateCommand();
    command.CommandText = "INSERT INTO TestTbl1 (Name) VALUES ('jeffand')";
    command.ExecuteNonQuery();

    command = connection.CreateCommand();
    command.CommandText = "INSERT INTO TestTbl1 (Name) VALUES ('otherperson')";
    command.ExecuteNonQuery();

    command = connection.CreateCommand();
    command.CommandText = "SELECT Name from TestTbl1";
    command.ExecuteNonQuery();

    Console.WriteLine("Reading the data");
    using (var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            var name = reader.GetString(0);

            Console.WriteLine($"Hello, {name}!");
        }
    }
}