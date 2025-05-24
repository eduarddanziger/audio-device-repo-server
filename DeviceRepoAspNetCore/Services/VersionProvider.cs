using System.Reflection;

namespace DeviceRepoAspNetCore.Services;

public class VersionProvider( (string? CodeVersion, string? LastCommitDate) version)
{
    public static (string? Version, string? LastCommitDate) ReadVersionFromAssembly()
    {
        var assembly = typeof(VersionProvider).Assembly;
        var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var lastCommitDateAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
        return (versionAttribute?.InformationalVersion, lastCommitDateAttribute?.Description);
    }

    public string CodeVersion { get; } = version.CodeVersion ?? "UnknownVersion";
    public string LastCommitDate { get; } = version.LastCommitDate ?? "UnknownDate";
}