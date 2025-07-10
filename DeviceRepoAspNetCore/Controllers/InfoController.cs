using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController(VersionProvider versionProvider) : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                releaseVersion = versionProvider.CodeVersion,
                lastCommitDate = versionProvider.LastCommitDate,
                runtime = versionProvider.Runtime
            });
        }
    }
}
