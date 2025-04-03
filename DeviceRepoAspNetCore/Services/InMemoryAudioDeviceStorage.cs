using DeviceRepoAspNetCore.Models;
using System.Reflection;

namespace DeviceRepoAspNetCore.Services;

internal class InMemoryAudioDeviceStorage : IAudioDeviceStorage
{
    private readonly Dictionary<string, AudioDevice> _audioDevices = new()
    {
        {
            "{9FEEEF35-0C6E-4F82-9607-F8F8FE76BD11}_Host1",
            new AudioDevice
            {
                PnpId = "{9FEEEF35-0C6E-4F82-9607-F8F8FE76BD11}",
                Name = "Speakers (High Definition Audio)",
                Volume = 775,
                LastSeen = DateTime.Parse("2021-07-01T08:00:00"),
                HostName = "Host1"
            }
        },
        {
            "{5DDB4DA4-52FF-4175-A061-8071ECBDB55D}_Host2",
            new AudioDevice
            {
                PnpId = "{74FE0EBE-7670-55DB-8C9E-14DC1ABC2231}",
                Name = "Microphone (USB Audio)",
                Volume = 50,
                LastSeen = DateTime.Parse("2023-07-01T11:20:00"),
                HostName = "Host2"
            }
        },
        {
            "{57F86104-2BA4-4C97-ABCA-4A64B9E496ED}_Host3",
            new AudioDevice
            {
                PnpId = "{57F86104-2BA4-4C97-ABCA-4A64B9E496ED}",
                Name = "Realtec",
                Volume = 500,
                LastSeen = DateTime.Parse("2022-01-21T12:20:00"),
                HostName = "Host3"
            }
        }
    };

    public IEnumerable<AudioDevice> GetAll() => _audioDevices.Values;

    public void Add(AudioDevice device)
    {
        var key = $"{device.PnpId}_{device.HostName}";
        _audioDevices[key] = device;
    }

    public void Remove(string pnpId, string hostName)
    {
        var key = $"{pnpId}_{hostName}";
        _audioDevices.Remove(key);
    }

    public void UpdateVolume(string pnpId, string hostName, int volume)
    {
        var key = $"{pnpId}_{hostName}";
        if (_audioDevices.TryGetValue(key, out var device))
        {
            device.Volume = volume;
        }
    }

    public IEnumerable<AudioDevice> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var normalizedQuery = query.ToLowerInvariant();
        return _audioDevices.Values.Where(device =>
#pragma warning disable CA1862
               device.Name.ToLowerInvariant().Contains(normalizedQuery) 
            || device.HostName.ToLowerInvariant().Contains(normalizedQuery)
#pragma warning restore CA1862
        );
    }
    public IEnumerable<AudioDevice> SearchByField(string field, string query)
    {
        if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(query))
            return [];

        var property = typeof(AudioDevice).GetProperty(field,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            throw new ArgumentException($"Unknown field: {field}");

        return _audioDevices.Values.Where(d =>
        {
            var value = property.GetValue(d);
            return value != null && value.ToString()!.Equals(query, StringComparison.OrdinalIgnoreCase);
        });
    }
}
