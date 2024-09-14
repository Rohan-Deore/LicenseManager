namespace LicenseManager
{
    /// <summary>
    /// User name, start date, end date, machine details (MAC ID)
    /// returns no string on successful check 
    /// or error message for user on failure.
    /// 
    /// Save everything in database
    /// </summary>
    public class License
    {
        public string? UserName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? MachineID { get; set; }
    }
}
