using DeviceRepoAspNetCore.Models.RestApi;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceRepoAspNetCore.Models.MongoDb;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(DeviceChangeEvent), typeof(VolumeChangeEvent))]
public abstract class ChangeEvent
{
    public DateTime EventDate { get; set; }
    public DeviceMessageType EventType { get; set; }

}

public class DeviceChangeEvent : ChangeEvent
{
    public DeviceFlowType FlowType { get; set; }
    public required string Details { get; set; }
}

public class VolumeChangeEvent : ChangeEvent
{
    public int Volume { get; set; }
}