using DeviceRepoAspNetCore.Models;

public interface IAudioDeviceStorage
{
    IEnumerable<DeviceMessage> GetAll();
    void Add(DeviceMessage deviceMessage);
    void Remove(string pnpId, string hostName);
    void UpdateVolume(string pnpId, string hostName, int volume);

    IEnumerable<DeviceMessage> Search(string query);
    IEnumerable<DeviceMessage> SearchByField(string field, string query);
}


