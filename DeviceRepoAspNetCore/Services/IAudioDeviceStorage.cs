using DeviceRepoAspNetCore.Models;

namespace DeviceRepoAspNetCore.Services;

public interface IAudioDeviceStorage
{
    IEnumerable<DeviceMessage> GetAll();
    void Add(DeviceMessage deviceMessage);
    void Remove(string pnpId, string hostName);
    void UpdateVolume(string pnpId, string hostName, int volume, bool renderOrCapture);

    IEnumerable<DeviceMessage> Search(string query);
}