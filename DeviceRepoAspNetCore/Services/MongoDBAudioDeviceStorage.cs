using DeviceRepoAspNetCore.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DeviceRepoAspNetCore.Services;

public class MongoDbAudioDeviceStorage : IAudioDeviceStorage
{
    private readonly IMongoCollection<AudioDeviceDocument> _devicesCollection;

    public MongoDbAudioDeviceStorage(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _devicesCollection = database.GetCollection<AudioDeviceDocument>("audio_devices");

        // Create compound index for PnpId and HostName
        var indexKeysDefinition = Builders<AudioDeviceDocument>.IndexKeys
            .Ascending(d => d.PnpId)
            .Ascending(d => d.HostName);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<AudioDeviceDocument>(indexKeysDefinition, indexOptions);
        _devicesCollection.Indexes.CreateOne(indexModel);
    }

    public IEnumerable<DeviceMessage> GetAll()
    {
        return _devicesCollection.Find(_ => true).ToList()
            .Select(d => d.ToDeviceMessage());
    }

    public void Add(DeviceMessage deviceMessage)
    {
        var existingDevice = _devicesCollection.Find(d =>
            d.PnpId == deviceMessage.PnpId &&
            d.HostName == deviceMessage.HostName).FirstOrDefault();
        var eventDetails = 
            existingDevice == null 
                ? "Device creation"
                : "Device entire update";


        var filter = Builders<AudioDeviceDocument>.Filter.And(
            Builders<AudioDeviceDocument>.Filter.Eq(d => d.PnpId, deviceMessage.PnpId),
            Builders<AudioDeviceDocument>.Filter.Eq(d => d.HostName, deviceMessage.HostName));

        var update = Builders<AudioDeviceDocument>.Update
            .Set(d => d.Name, deviceMessage.Name)
            .Set(d => d.FlowType, deviceMessage.FlowType)
            .Set(d => d.RenderVolume, deviceMessage.RenderVolume)
            .Set(d => d.CaptureVolume, deviceMessage.CaptureVolume)
            .Set(d => d.UpdateDate, deviceMessage.UpdateDate)
            .Set(d => d.MessageType, deviceMessage.MessageType)
            .Push(d => d.ChangeJournal, new DeviceChangeEvent
            {
                EventDate = deviceMessage.UpdateDate,
                EventType = deviceMessage.MessageType,
                Details = eventDetails
            });

        var options = new FindOneAndUpdateOptions<AudioDeviceDocument>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        var updatedDoc = _devicesCollection.FindOneAndUpdate(filter, update, options);
        if (updatedDoc == null)
        {
            throw new InvalidOperationException("Failed to add device");
        }


        // var existingDevice = _devicesCollection.Find(d =>
        //     d.PnpId == deviceMessage.PnpId &&
        //     d.HostName == deviceMessage.HostName).FirstOrDefault();
        //
        // if (existingDevice != null)
        // {
        //     throw new InvalidOperationException("Device already exists");
        // }
        //
        // var deviceDoc = new AudioDeviceDocument(deviceMessage);
        // _devicesCollection.InsertOne(deviceDoc);
    }

    public void Remove(string pnpId, string hostName)
    {
        var result = _devicesCollection.DeleteOne(d =>
            d.PnpId == pnpId &&
            d.HostName == hostName);

        if (result.DeletedCount == 0)
        {
            throw new KeyNotFoundException("Device not found");
        }
    }

    public void UpdateVolume(string pnpId, string hostName, int volume, bool renderOrCapture)
    {
        var filter = Builders<AudioDeviceDocument>.Filter.Where(d =>
            d.PnpId == pnpId &&
            d.HostName == hostName);

        var update = Builders<AudioDeviceDocument>.Update
            .Set(d => d.UpdateDate, DateTime.UtcNow)
            .Push(d => d.ChangeJournal, new DeviceChangeEvent
            {
                EventDate = DateTime.UtcNow,
                EventType = renderOrCapture ? MessageType.VolumeRenderChanged : MessageType.VolumeCaptureChanged,
                Details = $"Volume changed to {volume}"
            });

        update = renderOrCapture
            ? update.Set(d => d.RenderVolume, volume)
            : update.Set(d => d.CaptureVolume, volume);

        var result = _devicesCollection.UpdateOne(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new KeyNotFoundException("Device not found");
        }
    }

    public IEnumerable<DeviceMessage> Search(string query)
    {
        var filter = Builders<AudioDeviceDocument>.Filter.Or(
            Builders<AudioDeviceDocument>.Filter.Regex(d => d.PnpId, new BsonRegularExpression(query, "i")),
            Builders<AudioDeviceDocument>.Filter.Regex(d => d.Name, new BsonRegularExpression(query, "i")),
            Builders<AudioDeviceDocument>.Filter.Regex(d => d.HostName, new BsonRegularExpression(query, "i"))
        );

        return _devicesCollection.Find(filter).ToList()
            .Select(d => d.ToDeviceMessage());
    }

}

public class MongoDbSettings
{   // read out of configuration (appsettings.json)
    public required string ConnectionString { get; set; }// = "mongodb+srv://edanziger:qwer1234@cluster0.mdlxo.mongodb.net/";
    public required string DatabaseName { get; set; }// = "DeviceRepo";
}

public class AudioDeviceDocument
{
    public ObjectId Id { get; set; }
    public string PnpId { get; set; }
    public string Name { get; set; }
    public DeviceFlowType FlowType { get; set; }
    public int RenderVolume { get; set; }
    public int CaptureVolume { get; set; }
    public DateTime UpdateDate { get; set; }
    public string HostName { get; set; }
    public MessageType MessageType { get; set; }
    public List<DeviceChangeEvent> ChangeJournal { get; set; } = new();

    public AudioDeviceDocument(DeviceMessage message)
    {
        PnpId = message.PnpId;
        Name = message.Name;
        FlowType = message.FlowType;
        RenderVolume = message.RenderVolume;
        CaptureVolume = message.CaptureVolume;
        UpdateDate = message.UpdateDate;
        HostName = message.HostName;
        MessageType = message.MessageType;

        ChangeJournal.Add(new DeviceChangeEvent
        {
            EventDate = message.UpdateDate,
            EventType = message.MessageType,
            Details = "Initial device creation"
        });
    }

    public DeviceMessage ToDeviceMessage()
    {
        return new DeviceMessage
        {
            PnpId = PnpId,
            Name = Name,
            FlowType = FlowType,
            RenderVolume = RenderVolume,
            CaptureVolume = CaptureVolume,
            UpdateDate = UpdateDate,
            HostName = HostName,
            MessageType = MessageType
        };
    }
}

public class DeviceChangeEvent
{
    public DateTime EventDate { get; set; }
    public MessageType EventType { get; set; }
    public required string Details { get; set; }
}