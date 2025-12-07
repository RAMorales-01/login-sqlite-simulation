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
            Console.Clear();
            Console.WriteLine("The next program is a test to simulate a login process.");
            Console.WriteLine("the user can choose to create a new username and password or use the default credentials.");
            Console.WriteLine("\nPress any key to begin.");
            Console.ReadKey();

            try
            {
                bool hasAccount = IsRegisteredAlready("Are you a registered user?", "Y/N: ");
                VerifyDatabaseIsCreated(hasAccount);
                var credentials = CredentialsInput("--- User Login ---");

                if(ValidateUser(credentials.username, credentials.password))
                {
                    Console.Clear();
                    Console.WriteLine($"Access granted!, welcome {credentials.username}.\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Access denied, please contact an Administrator for more information.\n");
                }

                //TODO: Add a masking for the password, code to manage the Exceptions and new user registration.
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
            }
        }

        ///<summary>
        ///Verfies if user has an account already or wants to create a new account.
        ///</summary>
        ///<param name="prompt1">ask user for existing credentials</param>
        ///<param name="prompt2">ask for the input if yes or no</param>
        ///<returns>bool, true if wants to use existing credentials, false if wants to create new credentials</returns>
        private static bool IsRegisteredAlready(string prompt1, string prompt2)
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("Welcome!");
                Console.WriteLine(prompt1);
                Console.Write(prompt2);
                string input = Console.ReadLine().ToLower();

                if(string.Equals(input, "y", StringComparison.OrdinalIgnoreCase) || string.Equals(input, "yes", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if(string.Equals(input, "n", StringComparison.OrdinalIgnoreCase) || string.Equals(input, "no", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("\nERROR: Invalid input, expected 'y' for yes or 'n' for no. Press any key to try again.\n");
                    Console.ReadKey();
                }
            }
        }

        ///<summary>
        ///Ensures the database and folder exist
        ///</summary>
        private static void VerifyDatabaseIsCreated(bool hasAccount)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(databaseFilePath));//Checks the path for the database exists

            using(SqliteConnection connection = new SqliteConnection(connectionString))//Represents the communication channel for the database
            {
                connection.Open();//then open the connection
                SqliteCommand command = connection.CreateCommand();//Represents the SQL instruction and we use CommandText to assign the query 
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL);";
                command.ExecuteNonQuery();//finally we use one of the execute methods ExecuteNonQuery(), ExecuteScalar(), or ExecuteReader()

                if(hasAccount == true)
                {
                    AddDefaultUser(connection); 
                } 
                else
                {
                    CreateNewUser(connection);
                }
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
            }
        }

        ///<summary>
        ///Let user add credentials to the database, username and password
        ///</summary>
        private static void CreateNewUser(SqliteConnection connection)
        {
            while(true)
            {
                string username = CreateCredential("Enter username: ");
                string password = CreateCredential("Enter password: ");
                
                try
                {
                    SqliteCommand addUserCommand = connection.CreateCommand();
                    addUserCommand.CommandText = @"INSERT INTO Users (Username, PasswordHash) VALUES (@user, @hash)";
                    addUserCommand.Parameters.AddWithValue("@user", username);
                    addUserCommand.Parameters.AddWithValue("@hash", password);
                    addUserCommand.ExecuteNonQuery();

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
        ///Takes the input for the creation for: username and password
        ///</summary>
        ///<param name="prompt">prompts user for creation of username or password</param>
        ///<returns>string between 6 and 12 characters of the new username or password</returns> 
        private static string CreateCredential(string prompt)
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("NOTE: username and password must have a minimum of 6 and a maximum of 12 characters.");
                Console.Write(prompt);
                string userInput = Console.ReadLine();

                if(userInput.Length < 6 || userInput.Length > 12)
                {
                    Console.WriteLine("\nERROR: invalid input, expected string between 6 and 12 characters. Press any key to try again.");
                    Console.ReadKey();
                }
                else
                {
                    return userInput;
                }
            }
        }

        ///<summary>
        ///Ask user for input of the credentials
        ///<summary>
        ///<param name="prompt">informs user that is currently in the process for login</param>
        ///<returns>tuple of strings, one for the username input and the other for password input</returns>
        private static (string username, string password) CredentialsInput(string prompt)
        {
            Console.Clear();
            Console.WriteLine(prompt);
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            return (username, password);
        }
    }
}