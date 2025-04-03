namespace DeviceRepoAspNetCore.Models;

public class AudioDevice
{
    public required string PnpId { get; set; }
    public required string Name { get; set; }
    public int Volume { get; set; }
    public required DateTime LastSeen { get; set; }
    public required string HostName { get; set; }
}