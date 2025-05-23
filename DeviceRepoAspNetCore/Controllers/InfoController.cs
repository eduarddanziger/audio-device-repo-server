using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController(CodeVersionProvider versionProvider) : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                releaseVersion = versionProvider.CodeVersion
            });
        }
    }
}
