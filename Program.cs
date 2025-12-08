using System;
using System.Text;
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
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("The next program is a test to simulate a login process.");
            Console.WriteLine("the user can choose to create a new username and password or use the default credentials.");
            Console.WriteLine("\nPress any key to begin.");
            Console.ReadKey();

            try
            {
                bool hasAccount = UserInputsHandler.IsRegisteredAlready("Are you a registered user?", "Y/N: ");
                UserManager.VerifyDatabaseIsCreated(hasAccount);
                var credentials = UserInputsHandler.CredentialsInput("--- User Login ---");

                if(UserManager.ValidateUser(credentials.username, credentials.password))
                {
                    Console.Clear();
                    Console.WriteLine($"Access granted!, welcome {credentials.username}.\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Access denied, please contact an Administrator for more information.\n");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
            }
        }
    }
}