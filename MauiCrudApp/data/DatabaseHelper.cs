using MySql.Data.MySqlClient;

namespace MauiCrudApp.Data
{
    public static class DatabaseHelper
    {
        private const string connectionString = "server=127.0.0.1;port=3306;database=crudmaui;user=root;password=";


        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
