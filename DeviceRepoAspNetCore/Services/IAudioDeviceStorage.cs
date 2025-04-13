using DeviceRepoAspNetCore.Models.RestApi;

namespace DeviceRepoAspNetCore.Services;

public interface IAudioDeviceStorage
{
    IEnumerable<EntireDeviceMessage> GetAll();
    void Add(EntireDeviceMessage entireDeviceMessage);
    void Remove(string pnpId, string hostName);
    void UpdateVolume(string pnpId, string hostName, VolumeChangeMessage volumeChangeMessage);

    IEnumerable<EntireDeviceMessage> Search(string query);
}