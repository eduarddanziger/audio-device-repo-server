using DeviceRepoAspNetCore.Models.RestApi;
using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioDevicesController(IAudioDeviceStorage storage) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<EntireDeviceMessage> GetAll() => storage.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] EntireDeviceMessage entireDeviceMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entireDeviceMessage = entireDeviceMessage with { HostName = CryptService.ComputeChecksum(entireDeviceMessage.HostName) };

            storage.Add(entireDeviceMessage);
            return CreatedAtAction(
                nameof(GetByKey), 
                new { pnpId = entireDeviceMessage.PnpId, hostName = entireDeviceMessage.HostName },
                entireDeviceMessage
                );
        }

        [HttpGet("{pnpId}/{hostName}")]
        public IActionResult GetByKey(string pnpId, string hostName)
        {
            hostName = CryptService.ComputeChecksum(hostName);

            var device = storage.GetAll().FirstOrDefault(
                d => d.PnpId == pnpId && d.HostName == hostName
                );
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpDelete("{pnpId}/{hostName}")]
        public IActionResult Remove(string pnpId, string hostName)
        {
            hostName = CryptService.ComputeChecksum(hostName);

            storage.Remove(pnpId, hostName);
            return NoContent();
        }

        [HttpPut("{pnpId}/{hostName}")]
        public IActionResult UpdateVolume(string pnpId, string hostName, [FromBody] VolumeChangeMessage volumeChangeMessage)
        {
            hostName = CryptService.ComputeChecksum(hostName);

            storage.UpdateVolume(pnpId, hostName, volumeChangeMessage);
            return NoContent();
        }


        [HttpGet("search")]
        public IEnumerable<EntireDeviceMessage> Search(
            [FromQuery] string query)
        {
            var hashedHost = CryptService.ComputeChecksum(query);

            return storage.Search(query)
                .Union(storage.Search(hashedHost));
        }
    }
}