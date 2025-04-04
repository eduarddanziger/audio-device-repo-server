using System.Reflection;

namespace DeviceRepoAspNetCore.Services;

public class CodeVersionProvider(string? codeVersion)
{
    public static string? ReadFromAssembly()
    {
        var assembly = typeof(CodeVersionProvider).Assembly;
        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        return attribute?.InformationalVersion;
    }
    public string CodeVersion { get; } = codeVersion ?? "Unknown";
}