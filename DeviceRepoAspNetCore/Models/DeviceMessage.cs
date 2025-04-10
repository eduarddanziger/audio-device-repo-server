using System.ComponentModel.DataAnnotations;

namespace DeviceRepoAspNetCore.Models;

public enum DeviceFlowType
{
    Unknown = 0,
    Render,
    Capture,
    RenderAndCapture
};

public enum MessageType
{
    Neutral = 0,
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
    public required string HostName { get; set; }

    [Required]
    public MessageType MessageType { get; set; }
}