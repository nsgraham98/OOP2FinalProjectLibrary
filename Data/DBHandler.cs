using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Xml.Linq;

namespace OOP2FinalProjectLibrary.Data
{
    public class DBHandler
    {
        static string dbPath = Path.Combine(FileSystem.AppDataDirectory, "library_rental.db");
        static SQLiteConnection database;
        static string connectionString = $"Data Source={dbPath}";

        public static SQLiteConnection GetInitialDatabaseConnection()
        {
            if (database == null)
            {
                database = new SQLiteConnection($"Data Source={dbPath}");
            }
            return database;
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("library_rental.db").Result;
                using var newFile = File.Create(dbPath);
                stream.CopyTo(newFile);
            }
            GetInitialDatabaseConnection();
            database.Close();
        }

        public static int GetNextId(string tableName, string idColumnName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string selectQuery = $"SELECT MAX({idColumnName}) AS MaxId FROM {tableName}";

                using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, connection))
                {
                    var result = selectCmd.ExecuteScalar();
                    return result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1;
                }
            }  
        }
    }
}
