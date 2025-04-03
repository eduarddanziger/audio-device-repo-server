using DeviceRepoAspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioDevicesController(IAudioDeviceStorage storage) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<AudioDevice> GetAll() => storage.GetAll();

        [HttpPost]
        public IActionResult Add([FromBody] AudioDevice device)
        {
            storage.Add(device);
            return CreatedAtAction(
                nameof(GetByKey), 
                new { pnpId = device.PnpId, hostName = device.HostName }, 
                device
                );
        }

        [HttpGet("{pnpId}/{hostName}")]
        public IActionResult GetByKey(string pnpId, string hostName)
        {
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
            storage.Remove(pnpId, hostName);
            return NoContent();
        }

        [HttpPut("{pnpId}/{hostName}/volume")]
        public IActionResult UpdateVolume(string pnpId, string hostName, [FromBody] int volume)
        {
            storage.UpdateVolume(pnpId, hostName, volume);
            return NoContent();
        }

        [HttpGet("search")]
        public IEnumerable<AudioDevice> Search(
            [FromQuery] string query,
            [FromQuery] string field = null)
        {
            return string.IsNullOrEmpty(field) ?
                // Full-text search across all fields
                storage.Search(query) :
                // Field-specific search (e.g., hostName)
                storage.SearchByField(field, query);
        }
    }
}