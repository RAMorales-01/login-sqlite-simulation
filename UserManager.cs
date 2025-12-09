using System;
using System.Text;
using Microsoft.Data.Sqlite;
using System.IO;

namespace UserLogin
{
    public class UserManager
    {
        private static string databaseFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.db");//to define the path of the database
        private static string connectionString = $"Data Source={databaseFilePath}";//stores the return of the method Path.Combine for better manageability

        ///<summary>
        ///Ensures the database and folder exist
        ///</summary>
        public static void VerifyDatabaseIsCreated(bool hasAccount)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(databaseFilePath));//Checks the path for the database exists

            using(SqliteConnection connection = new SqliteConnection(connectionString))//Represents the communication channel for the database
            {
                connection.Open();//then open the connection
                SqliteCommand command = connection.CreateCommand();//Represents the SQL instruction and we use CommandText to assign the query 
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL, PasswordSalt TEXT NOT NULL);";
                command.ExecuteNonQuery();//finally we use one of the execute methods ExecuteNonQuery(), ExecuteScalar(), or ExecuteReader()

                if(hasAccount == true)
                {
                    AddDefaultUser(); 
                } 
                else
                {
                    CreateNewUser();
                }
            }
        }

        ///<summary>
        ///Default credentials in case user doesn't have credentials in the database
        ///</summary>
        private static void AddDefaultUser()
        {
            using(SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"SELECT COUNT(*) FROM Users";
                long userCount = (long)checkCommand.ExecuteScalar();

                if(userCount == 0)
                {
                    var hashedPasswordInfo = HashSaltUtil.HashPassword("password123");

                    SqliteCommand insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = @"INSERT INTO Users (Username, PasswordHash, PasswordSalt) VALUES (@user, @hash, @salt)";
                    insertCommand.Parameters.AddWithValue("@user", "admin");
                    insertCommand.Parameters.AddWithValue("@hash", hashedPasswordInfo.hash);
                    insertCommand.Parameters.AddWithValue("@salt", hashedPasswordInfo.salt);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        ///<summary>
        ///Let user add credentials to the database, username and password
        ///</summary>
        private static void CreateNewUser()
        {
            while(true)
            {
                string username = UserInputsHandler.CreateCredential("Enter username: ");
                string password = UserInputsHandler.CreateCredential("Enter password: ");
                
                try
                {
                    using(SqliteConnection connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();

                        var hashedPasswordInfo = HashSaltUtil.HashPassword(password);

                        SqliteCommand addUserCommand = connection.CreateCommand();
                        addUserCommand.CommandText = @"INSERT INTO Users (Username, PasswordHash, PasswordSalt) VALUES (@user, @hash, @salt)";
                        addUserCommand.Parameters.AddWithValue("@user", username);
                        addUserCommand.Parameters.AddWithValue("@hash", hashedPasswordInfo.hash);
                        addUserCommand.Parameters.AddWithValue("@salt", hashedPasswordInfo.salt);
                        addUserCommand.ExecuteNonQuery();
                    }

                    Console.WriteLine("\nNew user succesfully created!. Press any key to continue login process.");
                    Console.ReadKey();
                    break;
                }
                catch(Microsoft.Data.Sqlite.SqliteException ex)
                {
                    if(ex.SqliteErrorCode == 19)
                    {
                        Console.Clear();
                        Console.WriteLine($"\nERROR: The username '{username}' already exists.");
                        Console.WriteLine("Please choose a different username. Press any key to try again.\n");
                        Console.ReadKey();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
        }

        ///<summary>
        ///Validates the credentials of the user
        ///</summary>
        ///<param name="username">username input from the user</param>
        ///<param name="password">password input from the user</param>
        ///<returns>bool of the user verification</returns>
        public static bool ValidateUser(string username, string password)
        {
            using(SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT PasswordHash, PasswordSalt FROM Users WHERE Username = @user";
                command.Parameters.AddWithValue("@user", username);

                using(SqliteDataReader reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        string storedHash = reader.GetString(0);//reads from CommandText --> PasswordHash
                        string storedSalt = reader.GetString(1);//reads from CommandText --> PasswordSalt

                        return HashSaltUtil.VerifyPassword(password, storedHash, storedSalt);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}