using System;
using System.Text;
using Microsoft.Data.Sqlite;
using System.IO;

namespace UserLogin
{
    public class UserInputsHandler
    {
        ///<summary>
        ///Verfies if user has an account already or wants to create a new account.
        ///</summary>
        ///<param name="prompt1">ask user for existing credentials</param>
        ///<param name="prompt2">ask for the input if yes or no</param>
        ///<returns>bool, true if wants to use existing credentials, false if wants to create new credentials</returns>
        public static bool IsRegisteredAlready(string prompt1, string prompt2)
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
        ///Takes the input for the creation for: username and password
        ///</summary>
        ///<param name="prompt">prompts user for creation of username or password</param>
        ///<returns>string between 6 and 12 characters of the new username or password</returns> 
        public static string CreateCredential(string prompt)
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
        ///</summary>
        ///<param name="prompt">informs user that is currently in the process for login</param>
        ///<returns>tuple of strings, one for the username input and the other for password input</returns>
        public static (string username, string password) CredentialsInput(string prompt)
        {
            Console.Clear();
            Console.WriteLine(prompt);
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = MaskingPassword();

            return (username, password);
        }

        ///<summary>
        ///Helper method to mask char input with '*' during password input
        ///</summary>
        ///<returns>string masked with char '*' to hide the password input</returns>
        private static string MaskingPassword()
        {
            StringBuilder passInput = new StringBuilder();
            ConsoleKeyInfo key; //saves all the info for the pressed key

            do
            {
                key = Console.ReadKey(true);//to detect the input without showing what was pressed

                if(key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Escape)
                {
                    passInput.Append(key.KeyChar);//adds character to pressed key
                    Console.Write("*");//mask the pressed key during password input
                }
                else if(key.Key == ConsoleKey.Backspace && passInput.Length > 0)//to prevent backspace to be register as enter char during pass input
                {
                    passInput.Remove(passInput.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            while(key.Key != ConsoleKey.Enter);

            return passInput.ToString();
        }
    }
}