using DeviceRepoAspNetCore.Models;
using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioDevicesController(IAudioDeviceStorage storage) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<DeviceMessage> GetAll() => storage.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] DeviceMessage deviceMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            deviceMessage = deviceMessage with { HostName = CryptService.ComputeChecksum(deviceMessage.HostName) };

            storage.Add(deviceMessage);
            return CreatedAtAction(
                nameof(GetByKey), 
                new { pnpId = deviceMessage.PnpId, hostName = deviceMessage.HostName },
                deviceMessage
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
        public IActionResult UpdateVolume(string pnpId, string hostName, [FromBody] VolumeMessage volumeMessage)
        {
            hostName = CryptService.ComputeChecksum(hostName);

            storage.UpdateVolume(pnpId, hostName, volumeMessage);
            return NoContent();
        }


        [HttpGet("search")]
        public IEnumerable<DeviceMessage> Search(
            [FromQuery] string query)
        {
            var hashedHost = CryptService.ComputeChecksum(query);

            return storage.Search(query)
                .Union(storage.Search(hashedHost));
        }
    }
}