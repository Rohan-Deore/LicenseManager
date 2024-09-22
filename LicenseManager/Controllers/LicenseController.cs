using DatabaseManager;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly ILogger<LicenseController> _logger;
        LicenseDB licenseDB = new LicenseDB();

        public LicenseController(ILogger<LicenseController> logger)
        {
            _logger = logger;
        }

        //[HttpGet("", Name = "GetTables")]
        //public LicenseCheckResult Get()
        //{
        //    _logger.LogInformation("Get for creating table");
        //    licenseDB.CreateTables();

        //    return new LicenseCheckResult(); 
        //}

        [HttpGet("{UserName}, {CompanyName}, {ApplicationName}, {MachineID}", Name = "GetLicense")]
        public LicenseCheckResult Get(string UserName, string CompanyName, string ApplicationName, string MachineID)
        {
            _logger.LogInformation("Get for license called");
            
            string errMsg = string.Empty;
            var isValid = licenseDB.CheckLicense(UserName, CompanyName, ApplicationName, MachineID, ref errMsg);
            
            return new LicenseCheckResult() { IsLicensed = isValid, ErrorMessage = errMsg };
        }
    }
}
