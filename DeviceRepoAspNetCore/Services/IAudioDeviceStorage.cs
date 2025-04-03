using DeviceRepoAspNetCore.Models;

public interface IAudioDeviceStorage
{
    IEnumerable<AudioDevice> GetAll();
    void Add(AudioDevice device);
    void Remove(string pnpId, string hostName);
    void UpdateVolume(string pnpId, string hostName, int volume);

    IEnumerable<AudioDevice> Search(string query);
    IEnumerable<AudioDevice> SearchByField(string field, string query);
}


