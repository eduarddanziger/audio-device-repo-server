using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeviceRepoAspNetCore.Pages;

public class IndexModel(ILogger<IndexModel> logger, VersionProvider versionProvider)
    : PageModel
{
    public string Version => versionProvider.CodeVersion;

    public void OnGet()
    {
        // Use Version or log it if desired
        logger.LogInformation("Current version is {Version}", Version);
    }
}