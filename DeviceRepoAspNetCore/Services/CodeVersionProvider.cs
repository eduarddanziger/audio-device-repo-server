namespace DeviceRepoAspNetCore.Services;

public class CodeVersionProvider(string? codeVersion)
{
    private const string VersionFile = "vers.env";

    public static string? ReadFromFile()
    {
        var versionLine = File.Exists(VersionFile) ? File.ReadAllLines(VersionFile)
            .FirstOrDefault(line => line.StartsWith("CODE_VERSION=")) : null;
        return versionLine?.Split('=')[1];
    }
    public string CodeVersion { get; } = codeVersion ?? "Developer build";
}