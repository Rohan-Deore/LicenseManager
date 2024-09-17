using Microsoft.AspNetCore.Mvc;

namespace LicenseManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly ILogger<LicenseController> _logger;

        public LicenseController(ILogger<LicenseController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{UserName},{MachineID}", Name = "GetLicense")]
        public IEnumerable<License> Get(string UserName, string MachineID)
        {
            _logger.LogCritical("Get for license called");
            return Enumerable.Range(1, 5).Select(index => new License
            {
                UserName = $"User name {UserName}",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                MachineID = $"MachineID {MachineID}"
            })
            .ToArray();
        }
    }
}
