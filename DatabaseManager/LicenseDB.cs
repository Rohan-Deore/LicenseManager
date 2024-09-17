using Microsoft.Data.Sqlite;

namespace DatabaseManager
{
    public class LicenseDB
    {
        private SqliteConnection? connection = null;

        public LicenseDB()
        {
            connection = new SqliteConnection("Data Source=LicenseDB.db");
            connection.Open();
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Function to create table to be used for debugging on location machine
        /// </summary>
        public void CreateTables()
        {
            var command = connection!.CreateCommand();
            command!.CommandText = @"
                    CREATE TABLE LicenseTbl (
                        UserID INT PRIMARY KEY  NOT NULL,
                        UserName TEXT,
                        ApplicationName TEXT,
                        StartDate TEXT,
                        EndDate TEXT
                    );";

            command.ExecuteNonQuery();

            command!.CommandText = @"
                    CREATE TABLE LicenseLogTbl (
                        UserID INT,
                        MachineID INT,
                        ErrorMsg TEXT
                    );";

            command.ExecuteNonQuery();
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

        //public void AddCustomerDB(Customer customer)
        //{
        //    List<Customer> list = new List<Customer>();
        //    var command = connection.CreateCommand();
        //    command.CommandText = $@"INSERT INTO CustomerData
        //            VALUES ('{customer.CustomerName}', '{customer.CustomerLocation}');";

        //    command.ExecuteNonQuery();
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
