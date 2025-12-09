A small project to simulate the login process of an user

The idea behind this program was to simulate the login process for an user.
Previously i made a similar program but this time i decided to use sql databases since i have no
experience with databases, after some research i decided to go with SQLite since is was the most
accesible option and since im already learning c# and plan to keep learning all about .NET i decided to proceed(for now) with SQLite and test SQL language and the user of the Directives of System.Microsoft.Data.Sqlite and System.IO.
Using AI and forum recomendations, decided to try and add Hash and salt for the passwords stored on the database, since im learning c# and .net decided to use the native namespace for System.Security.Cryptography to encode and decode the data and following the official documentation which according to its recommendation of using the algorithm SHA256 or higher.