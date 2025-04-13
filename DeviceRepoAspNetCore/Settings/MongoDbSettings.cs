namespace DeviceRepoAspNetCore.Settings;

public class MongoDbSettings
{   // read out of configuration (appsettings.json)
    public required string ConnectionStringAnonymous { get; set; }
    public required string DatabaseName { get; set; }
    public required string DatabaseUser { get; set; }
    public required string DatabasePassword { get; set; }
}