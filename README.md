# Audio Device Repository Server / ASP.NET Core with REST API

- The backend of Audio Device Repository Server as a ASP.Net Core Server with REST API (C# / MongoDB).

- The GUI client (React SPA) can be started here: [Audio Device Repository Client](https://eduarddanziger.github.io/list-audio-react-app/)

- For Audio Device REST API desctiption see [rest-api-documentation.md](DeviceRepoAspNetCore/rest-api-documentation.md).

## Latest rollouts on GitHub Codespace and Azure App Service

- The ASP.NET Core Server starts automatically on-demand, if it doesn't run already.
- The GUI client resides in a repository [list-audio-react-app](https://github.com/eduarddanziger/list-audio-react-app/)

## Development environment

- The ASP.NET Core server can be started via Terminal using the following command:

```powershell or bash
cd DeviceRepoAspNetCore
dotnet run --launch-profile http
```
