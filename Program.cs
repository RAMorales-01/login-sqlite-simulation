using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace UserLogin
{
    ///<summary>
    ///A small project to simulate a login process for an user. It also a self study and practice
    ///in the use of databases using Sqlite.
    ///</summary>
    class Program
    {
        private static string databaseFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.db");//to define the path of the database
        private static string connectionString = $"Data Source={databaseFilePath}";//stores the return of the method Path.Combine for better manageability

        static void Main(string[] args)
        {
            VerifyDatabaseIsCreated();

            Console.WriteLine("--- User Login ---");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if(ValidateUser(username, password))
            {
                Console.WriteLine($"\nLogin Succesful. welcome {username}!.");
            }
            else
            {
                Console.Write("\nLogin Denied. Invalid username or password.");
            }

            //TODO: Add a masking for the password, code to manage the Exceptions and new user registration.
        }

        ///<summary>
        ///Ensures the database and folder exist
        ///</summary>
        private static void VerifyDatabaseIsCreated()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(databaseFilePath));//Checks the path for the database exists

            using(SqliteConnection connection = new SqliteConnection(connectionString))//Represents the communication channel for the database
            {
                connection.Open();//then open the connection
                SqliteCommand command = connection.CreateCommand();//Represents the SQL instruction and we use CommandText to assign the query 
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL);";
                command.ExecuteNonQuery();//finally we use one of the execute methods ExecuteNonQuery(), ExecuteScalar(), or ExecuteReader()

                AddDefaultUser(connection);
            }
        }

        ///<summary>
        ///Validates the credentials of the user
        ///</summary>
        ///<param name="username">username input from the user</param>
        ///<param name="password">password input from the user</param>
        ///<returns>bool of the user verification</returns>
        private static bool ValidateUser(string username, string password)
        {
            using(SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM Users WHERE Username = @user AND PasswordHash = @hash";
                command.Parameters.AddWithValue("@user", username);
                command.Parameters.AddWithValue("@hash", password);

                long count = (long)command.ExecuteScalar();

                return count > 0;
            }
        }

        ///<summary>
        ///Default credentials in case user doesn't have credentials in the database
        ///</summary>
        private static void AddDefaultUser(SqliteConnection connection)
        {
            SqliteCommand checkCommand = connection.CreateCommand();
            checkCommand.CommandText = @"SELECT COUNT(*) FROM Users";
            long userCount = (long)checkCommand.ExecuteScalar();

            if(userCount == 0)
            {
                SqliteCommand insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"INSERT INTO Users (Username, PasswordHash) VALUES (@user, @hash)";
                insertCommand.Parameters.AddWithValue("@user", "admin");
                insertCommand.Parameters.AddWithValue("@hash", "password123");
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Use default user 'admin' created with password 'password123'.");
            }
        }
    }
}