namespace LicenseManager
{
    /// <summary>
    /// User name, start date, end date, machine details (MAC ID)
    /// returns no string on successful check 
    /// or error message for user on failure.
    /// 
    /// Save everything in database
    /// 
    /// Before start date application is in trial mode and after end date application not valid
    /// </summary>
    public class License
    {
        public int UserID { get; set; }

        public string? UserName { get; set; }

        public string? CompanyName { get; set; }
        
        public string? ApplicationName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? MachineID { get; set; }

        public string? AppError { get; set; }
    }

    public class LicenseCheckResult
    { 
        public bool IsLicensed { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
