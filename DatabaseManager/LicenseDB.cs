using Microsoft.Data.Sqlite;
using NLog;

namespace DatabaseManager
{
    public class LicenseDB
    {
        private SqliteConnection? connection = null;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public LicenseDB()
        {
            connection = new SqliteConnection("Data Source=LicenseDB.db");
            connection.Open();
            logger.Info("SQLite connection created.");
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }

            LogManager.Shutdown();
        }

        /// <summary>
        /// Function to create table to be used for debugging on location machine
        /// </summary>
        public void CreateTables()
        {
            if (connection == null)
            {
                logger.Error("Connection not created.");
                return;
            }

            var command = connection?.CreateCommand();
            command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS LicenseTbl (
                        UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserName TEXT,
                        CompanyName TEXT,
                        ApplicationName TEXT,
                        StartDate TEXT,
                        EndDate TEXT,
                        IsLicensed BOOLEAN
                    );";

            command.ExecuteNonQuery();

            command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS LicenseLogTbl (
                        LogDateTime TEXT,
                        UserID INT,
                        MachineID TEXT,
                        ErrorMsg TEXT
                    );";

            command.ExecuteNonQuery();
        }

        public bool AddUser(string userName, string companyName, string applicationName, bool isLicensed, DateOnly startDate, DateOnly endDate)
        {
            var command = connection!.CreateCommand();

            // Check if the record exists
            command.CommandText = @"
                SELECT COUNT(1) 
                FROM LicenseTbl 
                WHERE UserName = @UserName AND CompanyName = @CompanyName AND ApplicationName = @ApplicationName";

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@CompanyName", companyName);
            command.Parameters.AddWithValue("@ApplicationName", applicationName);

            int count = Convert.ToInt32(command.ExecuteScalar());

            if (count == 0)
            {
                // Insert the record if it does not exist
                command.CommandText = @"
                    INSERT INTO LicenseTbl(UserName, CompanyName, ApplicationName, StartDate, EndDate, IsLicensed)
                    VALUES (@UserName, @CompanyName, @ApplicationName, @StartDate, @EndDate, @IsLicensed);
                    SELECT last_insert_rowid();";

                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@IsLicensed", isLicensed);

                var userId = (long)command.ExecuteScalar();

                logger.Info($"Inserted user ID: {userId}");

                var logCommand = connection!.CreateCommand();

                logCommand.CommandText = @"
                    INSERT INTO LicenseLogTbl(LogDateTime, UserID, MachineID, ErrorMsg)
                    VALUES (@LogDateTime, @UserID, @MachineID, @ErrorMsg)";
                logCommand.Parameters.AddWithValue("@LogDateTime", DateTime.Now);
                logCommand.Parameters.AddWithValue("@UserID", userId);
                logCommand.Parameters.AddWithValue("@MachineID", "");
                logCommand.Parameters.AddWithValue("@ErrorMsg", "User added");
                logCommand.ExecuteNonQuery();

                return true;
            }
            else 
            {
                logger.Error($"Entry with {userName}, {companyName}, {applicationName} already exists.");
                return false;
            }
        }

        //public List<Customer> GetCustomerAll()
        //{
        //    List<Customer> list = new List<Customer>();
        //    var command = connection.CreateCommand();
        //    command.CommandText = @"SELECT *
        //                        FROM CustomerData";
        //    using (var reader = command.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            var customerName = reader.GetString(0);
        //            var customerLoc = reader.GetString(1);

        //            list.Add(new Customer() { CustomerName = customerName, CustomerLocation = customerLoc });
        //            Console.WriteLine($"Hello, {customerName}!");
        //        }
        //    }

        //    return list;
        //}

        //public void DeleteCustomerDB(Customer customer)
        //{
        //    var command = connection.CreateCommand();
        //    command.CommandText = $@"DELETE FROM CustomerData 
        //        WHERE CustomerName='{customer.CustomerName}' AND 
        //        CustomerLocation='{customer.CustomerLocation}';";

        //    command.ExecuteNonQuery();
        //}

        //internal List<Orders> GetOrderAll()
        //{
        //    List<Orders> list = new List<Orders>();
        //    var command = connection.CreateCommand();
        //    command.CommandText = @"SELECT *
        //                        FROM OrderData";
        //    using (var reader = command.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            var partID = reader.GetString(0);
        //            var partName = reader.GetString(1);

        //            list.Add(new Orders() { PartID = partID, PartName = partName });
        //        }
        //    }

        //    return list;
        //}

        //public void AddOrdersDB(Orders orders)
        //{
        //    List<Orders> list = new List<Orders>();
        //    var command = connection.CreateCommand();
        //    command.CommandText = $@"INSERT INTO OrderData
        //            VALUES ('{orders.PartID}', '{orders.PartName}');";

        //    command.ExecuteNonQuery();
        //}

        //public void DeleteOrdersDB(Orders order)
        //{
        //    var command = connection.CreateCommand();
        //    command.CommandText = $@"DELETE FROM OrderData 
        //        WHERE PartID='{order.PartID}' AND 
        //        PartName='{order.PartName}';";

        //    command.ExecuteNonQuery();
        //}
    }
}
