using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Data.SqlClient;

namespace WaitlistApplication
{
    public class DatabaseManager
    {
        private static object lockObj = new object();
        private static DatabaseManager? singletonInstance = null;

        // Database names and variables
        const string WAITLIST_DBNAME = "WaitlistApp";
        const string SQLEXPRESS_MASTER_CONNECTIONSTRING = @"SERVER=.\SQLEXPRESS01;Database=master;Integrated Security=SSPI";
        const string SQLEXPRESS_WAITLIST_CONNECTIONSTRING = $@"SERVER=.\SQLEXPRESS01;Database={WAITLIST_DBNAME};Integrated Security=SSPI";
        const string SQLITE_DBNAME = $"{WAITLIST_DBNAME}.db";
        const string SQLITE_CONNECTIONSTRING = $"Data Source={SQLITE_DBNAME}";

        private DatabaseManager()
        {
            this.InitializeDatabase();
        }

        private DatabaseType PickDatabaseType()
        {
            // By default, connect to a Sqlite in-memory DB instance, but allow 
            // fallbacks to SqlExpress or Azure SQL as necessary
            return DatabaseType.Sqlite;
        }

        private void InitializeDatabase()
        {
            switch (this.PickDatabaseType())
            {
                case DatabaseType.Sqlite:
                    InitializeSqlite();
                    break;
                case DatabaseType.SqlExpress:
                    InitializeSqlExpress();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void InitializeSqlExpress()
        {
            // See if the SQLExpress database exists, and if not, create it
            using (DbConnection dbConn = this.CreateConnection())
            {
                dbConn.Open();
                var sqlCmd = dbConn.CreateCommand();
                sqlCmd.CommandText = $"IF DB_ID('{WAITLIST_DBNAME}') IS NULL CREATE DATABASE {WAITLIST_DBNAME}";
                sqlCmd.ExecuteNonQuery();
            }

            // Now connect to the waitlist database and initialize schema
            using (SqlConnection sqlConn = new SqlConnection(SQLEXPRESS_WAITLIST_CONNECTIONSTRING))
            {
                sqlConn.Open();

                InitializeSchema(sqlConn);
            }
        }

        private void InitializeSqlite()
        {
            // Clean up any existing file
            if (File.Exists(SQLITE_DBNAME))
            {
                File.Delete(SQLITE_DBNAME);
            }

            using (var connection = new SqliteConnection($"Data Source={SQLITE_DBNAME}"))
            {
                connection.Open();

                InitializeSchema(connection);
            }
        }

        private bool DoesTableExist(DbConnection dbConn, string tableName)
        {
            // Sqlite and SqlExpress/Azure have different syntax for checking if tables exist
            using (var command = dbConn.CreateCommand())
            {
                switch (this.PickDatabaseType())
                {
                    case DatabaseType.Sqlite:
                        command.CommandText = $"SELECT 1 FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                        break;
                    case DatabaseType.SqlExpress:
                        command.CommandText = $"SELECT 1 FROM sys.tables WHERE name='{tableName}'";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return command.ExecuteScalar() != null;
            }
        }

        private void InitializeSchema(DbConnection dbConn)
        {
            // See if Restaurant table exists, and if not, initialize the schema
            if (!DoesTableExist(dbConn, "Restaurant"))
            {
                using (var command = dbConn.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE Restaurant (
                            ID UNIQUEIDENTIFIER NOT NULL,
                            Name NVARCHAR(255) NOT NULL
                        )";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Singleton instance of the Database Manager class.
        /// </summary>
        public static DatabaseManager Instance
        {
            get
            {
                if (singletonInstance == null)
                {
                    // Use the double-check-lock pattern since initialization of the DBManager is
                    // extremely expensive, and likely to cause race conditions if not synchronized
                    lock (lockObj)
                    {
                        if (singletonInstance == null)
                        {
                            singletonInstance = new DatabaseManager();
                        }
                    }
                }

                return singletonInstance;
            }
        }

        /// <summary>
        /// Ensures that the database connection is successful and that the database is ready for the program to run.
        /// </summary>
        public static void Initialize()
        {
            // The singleton instance automatically initializes during construction, so just calling the singleton will ensure initialization
            var dbInstance = Instance;
        }

        /// <summary>
        /// Creates a connection to the currently attached database.
        /// </summary>
        /// <returns>A database connection which is not yet opened.</returns>
        public DbConnection CreateConnection()
        {
            switch (this.PickDatabaseType())
            {
                case DatabaseType.Sqlite:
                    return new SqliteConnection(SQLITE_CONNECTIONSTRING);
                case DatabaseType.SqlExpress:
                    return new SqlConnection(SQLEXPRESS_MASTER_CONNECTIONSTRING);
            }

            throw new NotImplementedException();
        }
    }
}
