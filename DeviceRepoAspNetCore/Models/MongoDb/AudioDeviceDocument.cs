using DeviceRepoAspNetCore.Models.RestApi;
using MongoDB.Bson;

namespace DeviceRepoAspNetCore.Models.MongoDb;

public class AudioDeviceDocument
{
    public ObjectId Id { get; set; }
    public required string PnpId { get; set; }
    public required string Name { get; set; }
    public DeviceFlowType FlowType { get; set; }
    public int RenderVolume { get; set; }
    public int CaptureVolume { get; set; }
    public DateTime UpdateDate { get; set; }
    public required string HostName { get; set; }
    public DeviceMessageType DeviceMessageType { get; set; }
    public List<ChangeEvent> ChangeJournal { get; set; } = [];

    public EntireDeviceMessage ToDeviceMessage()
    {
        return new EntireDeviceMessage
        {
            PnpId = PnpId,
            Name = Name,
            FlowType = FlowType,
            RenderVolume = RenderVolume,
            CaptureVolume = CaptureVolume,
            UpdateDate = UpdateDate,
            HostName = HostName,
            DeviceMessageType = DeviceMessageType
        };
    }
}