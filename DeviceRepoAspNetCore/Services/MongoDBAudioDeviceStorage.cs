using DeviceRepoAspNetCore.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DeviceRepoAspNetCore.Services;

public class MongoDbAudioDeviceStorage : IAudioDeviceStorage
{
    private readonly ILogger<CryptService> _logger;
    private readonly IMongoCollection<AudioDeviceDocument> _devicesCollection;

    public MongoDbAudioDeviceStorage(IOptions<MongoDbSettings> mongoDbSettings, CryptService cryptService, ILogger<CryptService> logger)
    {
        _logger = logger;
        const string shortestPassword = "my.shortest.password";
        const string urlPrefix = "mongodb+srv://";

        var decryptedUser = cryptService.TryDecryptOrReturnOriginal(mongoDbSettings.Value.DatabaseUser, shortestPassword);
        var decryptedPassword = cryptService.TryDecryptOrReturnOriginal(mongoDbSettings.Value.DatabasePassword, shortestPassword);
        var connectionString = mongoDbSettings.Value.ConnectionStringAnonymous
            .Replace(urlPrefix, $"{urlPrefix}{decryptedUser}:{decryptedPassword}@");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _devicesCollection = database.GetCollection<AudioDeviceDocument>("devices");

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
        if (existingDevice != null)
        {
            _logger.LogWarning("Device already exists, updating instead of adding");
        }

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
            .Set(d => d.DeviceMessageType, deviceMessage.DeviceMessageType)
            .Push(d => d.ChangeJournal, new DeviceChangeEvent
            {
                EventDate = deviceMessage.UpdateDate,
                EventType = deviceMessage.DeviceMessageType,
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

    public void UpdateVolume(string pnpId, string hostName, VolumeMessage volumeMessage)
    {
        var filter = Builders<AudioDeviceDocument>.Filter.Where(d =>
            d.PnpId == pnpId &&
            d.HostName == hostName);

        var update = Builders<AudioDeviceDocument>.Update
            .Set(d => d.UpdateDate, volumeMessage.UpdateDate)
            .Push(d => d.ChangeJournal, new DeviceChangeEvent
            {
                EventDate = volumeMessage.UpdateDate,
                EventType = volumeMessage.DeviceMessageType,
                Details = volumeMessage.DeviceMessageType == DeviceMessageType.VolumeRenderChanged
                    ? $"Render volume changed to {volumeMessage}"
                    : $"Capture volume changed to {volumeMessage}"
            });

        update = volumeMessage.DeviceMessageType == DeviceMessageType.VolumeRenderChanged
            ? update.Set(d => d.RenderVolume, volumeMessage.Volume)
            : update.Set(d => d.CaptureVolume, volumeMessage.Volume);

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
    public required string ConnectionStringAnonymous { get; set; }
    public required string DatabaseName { get; set; }
    public required string DatabaseUser { get; set; }
    public required string DatabasePassword { get; set; }
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
    public DeviceMessageType DeviceMessageType { get; set; }
    public List<DeviceChangeEvent> ChangeJournal { get; set; } = [];

    public AudioDeviceDocument(DeviceMessage message)
    {
        PnpId = message.PnpId;
        Name = message.Name;
        FlowType = message.FlowType;
        RenderVolume = message.RenderVolume;
        CaptureVolume = message.CaptureVolume;
        UpdateDate = message.UpdateDate;
        HostName = message.HostName;
        DeviceMessageType = message.DeviceMessageType;

        ChangeJournal.Add(new DeviceChangeEvent
        {
            EventDate = message.UpdateDate,
            EventType = message.DeviceMessageType,
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
            DeviceMessageType = DeviceMessageType
        };
    }
}

public class DeviceChangeEvent
{
    public DateTime EventDate { get; set; }
    public DeviceMessageType EventType { get; set; }
    public required string Details { get; set; }
}