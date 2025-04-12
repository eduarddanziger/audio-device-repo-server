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

public record DeviceMessage
{
    [Required]
    public required string PnpId { get; init; }

    [Required]
    public required string HostName { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public DeviceFlowType FlowType { get; init; }

    [Range(0, 1000)]
    public int RenderVolume { get; init; }

    [Range(0, 1000)]
    public int CaptureVolume { get; init; }

    [Required]
    public required DateTime UpdateDate { get; init; }
    
    [Required]
    [AllowedDeviceMessageTypes(DeviceMessageType.Confirmed, DeviceMessageType.Discovered)]
    public DeviceMessageType DeviceMessageType { get; init; }
}

public record VolumeMessage
{
    [Required]
    public required DateTime UpdateDate { get; init; }

    [Required]
    [AllowedDeviceMessageTypes(DeviceMessageType.VolumeRenderChanged, DeviceMessageType.VolumeCaptureChanged)]
    public DeviceMessageType DeviceMessageType { get; init; }

    [Required]
    public int Volume { get; init; }
}
