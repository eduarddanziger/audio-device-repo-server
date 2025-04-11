using System.ComponentModel.DataAnnotations;

namespace DeviceRepoAspNetCore.Models;

public enum DeviceFlowType
{
    Unknown = 0,
    Render,
    Capture,
    RenderAndCapture
};

public enum DeviceMessageType
{
    Confirmed = 0,
    Discovered,
    Detached,
    VolumeRenderChanged,
    VolumeCaptureChanged
};

public class DeviceMessage
{
    [Required]
    public required string PnpId { get; set; }

    [Required]
    public required string HostName { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public DeviceFlowType FlowType { get; set; }

    [Range(0, 1000)]
    public int RenderVolume { get; set; }

    [Range(0, 1000)]
    public int CaptureVolume { get; set; }

    [Required]
    public required DateTime UpdateDate { get; set; }
    
    [Required]
    [AllowedDeviceMessageTypes(DeviceMessageType.Confirmed, DeviceMessageType.Discovered)]
    public DeviceMessageType DeviceMessageType { get; set; }

    public DeviceMessage Clone()
    {
        return new DeviceMessage
        {
            PnpId = this.PnpId,
            Name = this.Name,
            FlowType = this.FlowType,
            RenderVolume = this.RenderVolume,
            CaptureVolume = this.CaptureVolume,
            UpdateDate = this.UpdateDate,
            DeviceMessageType = this.DeviceMessageType,
            HostName = this.HostName
        };
    }
}

public class VolumeMessage
{
    [Required]
    public required DateTime UpdateDate { get; set; }

    [Required]
    [AllowedDeviceMessageTypes(DeviceMessageType.VolumeRenderChanged, DeviceMessageType.VolumeCaptureChanged)]
    public DeviceMessageType DeviceMessageType { get; set; }

    [Required]
    public int Volume { get; set; }
}
