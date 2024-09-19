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

        public bool CheckLicense(string userName, string companyName, string applicationName, string machineID, ref string errorMessage)
        { 
            bool status = false;

            var command = connection!.CreateCommand();

            // Check if the record exists
            command.CommandText = @"
                SELECT * 
                FROM LicenseTbl 
                WHERE UserName = @UserName AND CompanyName = @CompanyName AND ApplicationName = @ApplicationName";

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@CompanyName", companyName);
            command.Parameters.AddWithValue("@ApplicationName", applicationName);

            var licenseReader = command.ExecuteReader();
            if(!licenseReader.Read())
            {
                errorMessage = "Invalid details";
                return false;
            }

            var userID = licenseReader.GetInt32(0);

            var startDate = DateOnly.Parse(licenseReader.GetString(4)); // start date
            var endDate = DateOnly.Parse(licenseReader.GetString(5)); // end date
            var isLicensed = licenseReader.GetInt32(6) == 1; // bool is licensed

            var dateToday = DateOnly.FromDateTime(DateTime.Today);
            errorMessage = string.Empty;
            if (dateToday <= startDate)
            {
                // trial period
                errorMessage = "Trial period";
                status = true;
            }
            else if (dateToday > startDate && dateToday <= endDate)
            {
                // licensed preriod
                if (isLicensed)
                {
                    errorMessage = "Valid license";
                    status = true;
                }
                else
                {
                    errorMessage = "Invalid license";
                    status = false;
                }
            }
            else
            {
                // not licensed
                errorMessage = "Not licensed";
                status = false;
            }

            var logCommand = connection!.CreateCommand();

            logCommand.CommandText = @"
                    INSERT INTO LicenseLogTbl(LogDateTime, UserID, MachineID, ErrorMsg)
                    VALUES (@LogDateTime, @UserID, @MachineID, @ErrorMsg)";
            logCommand.Parameters.AddWithValue("@LogDateTime", DateTime.Now);
            logCommand.Parameters.AddWithValue("@UserID", userID);
            logCommand.Parameters.AddWithValue("@MachineID", machineID);
            logCommand.Parameters.AddWithValue("@ErrorMsg", errorMessage);
            logCommand.ExecuteNonQuery();

            return status;
        }
    }
}
