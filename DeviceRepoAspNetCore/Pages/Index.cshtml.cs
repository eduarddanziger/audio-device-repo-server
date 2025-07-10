using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace DeviceRepoAspNetCore.Pages;

public class IndexModel(VersionProvider versionProvider)
    : PageModel
{
    public string Version => versionProvider.CodeVersion;
    public string Timestamp => versionProvider.LastCommitDate;
    public string Runtime => versionProvider.Runtime;

    public string CopyrightYear => DateTime.TryParse(Timestamp, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt.Year.ToString() : "2024";

    public void OnGet()
    {
        ViewData["CopyrightYear"] = CopyrightYear;
    }
}