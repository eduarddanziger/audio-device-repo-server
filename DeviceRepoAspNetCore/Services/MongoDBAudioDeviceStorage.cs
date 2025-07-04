﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using DeviceRepoAspNetCore.Models.RestApi;
using DeviceRepoAspNetCore.Settings;
using DeviceRepoAspNetCore.Models.MongoDb;

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

    public IEnumerable<EntireDeviceMessage> GetAll()
    {
        return _devicesCollection.Find(_ => true).ToList()
            .Select(d => d.ToDeviceMessage());
    }

    public void Add(EntireDeviceMessage entireDeviceMessage)
    {
        var existingDevice = _devicesCollection.Find(d =>
            d.PnpId == entireDeviceMessage.PnpId &&
            d.HostName == entireDeviceMessage.HostName).FirstOrDefault();
        if (existingDevice != null)
        {
            _logger.LogWarning("Device already exists, updating instead of adding");
        }

        var eventDetails = 
        existingDevice == null 
            ? "Device creation"
            : "Device entire update";

        var filter = Builders<AudioDeviceDocument>.Filter.And(
            Builders<AudioDeviceDocument>.Filter.Eq(d => d.PnpId, entireDeviceMessage.PnpId),
            Builders<AudioDeviceDocument>.Filter.Eq(d => d.HostName, entireDeviceMessage.HostName));

        var update = Builders<AudioDeviceDocument>.Update
            .Set(d => d.Name, entireDeviceMessage.Name)
            .Set(d => d.OperationSystemName, entireDeviceMessage.OperationSystemName)
            .Set(d => d.FlowType, entireDeviceMessage.FlowType)
            .Set(d => d.RenderVolume, entireDeviceMessage.RenderVolume)
            .Set(d => d.CaptureVolume, entireDeviceMessage.CaptureVolume)
            .Set(d => d.UpdateDate, entireDeviceMessage.UpdateDate)
            .Set(d => d.DeviceMessageType, entireDeviceMessage.DeviceMessageType)
            .Push(d => d.ChangeJournal, new DeviceChangeEvent
            {
                EventDate = entireDeviceMessage.UpdateDate,
                EventType = entireDeviceMessage.DeviceMessageType,
                FlowType = entireDeviceMessage.FlowType,
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

    public void UpdateVolume(string pnpId, string hostName, VolumeChangeMessage volumeChangeMessage)
    {
        var filter = Builders<AudioDeviceDocument>.Filter.Where(d =>
            d.PnpId == pnpId &&
            d.HostName == hostName);

        var update = Builders<AudioDeviceDocument>.Update
            .Set(d => d.UpdateDate, volumeChangeMessage.UpdateDate)
            .Set(d => d.DeviceMessageType, volumeChangeMessage.DeviceMessageType)
            .Push(d => d.ChangeJournal, new VolumeChangeEvent
            {
                EventDate = volumeChangeMessage.UpdateDate,
                EventType = volumeChangeMessage.DeviceMessageType,
                Volume = volumeChangeMessage.Volume
            });

        update = volumeChangeMessage.DeviceMessageType == DeviceMessageType.VolumeRenderChanged
            ? update.Set(d => d.RenderVolume, volumeChangeMessage.Volume)
            : update.Set(d => d.CaptureVolume, volumeChangeMessage.Volume);

        var result = _devicesCollection.UpdateOne(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new KeyNotFoundException("Device not found");
        }
    }

    public IEnumerable<EntireDeviceMessage> Search(string query)
    {
        var loweredQuery = query.ToLowerInvariant();
        return _devicesCollection.AsQueryable()
// MongoDB’s LINQ provider does not support StringComparison.OrdinalIgnoreCase in .Contains() or similar string methods.
#pragma warning disable CA1862
            .Where(d => d.PnpId.ToLower().Contains(loweredQuery)
                        || d.Name.ToLower().Contains(loweredQuery)
                        || d.HostName.ToLower().Contains(loweredQuery)
                        || d.OperationSystemName.ToLower().Contains(loweredQuery)
            )
#pragma warning restore CA1862
            .ToList()
            .Select(d => d.ToDeviceMessage());
    }
}
