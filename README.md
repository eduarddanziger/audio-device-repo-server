# Audio Device Repository Server / ASP.NET Core with REST API

The backend of Audio Device Repository Server as a ASP.Net Core Server with REST API (C# / MongoDB).

For Audio Device REST API see [rest-api-documentation.md](DeviceRepoAspNetCore/rest-api-documentation.md).

## Latest rollout on GitHub Infrastructure

- The ASP.NET Core server is started by a client request, if the client runs on the GitHub Infrastructure, too.
- The GUI client (ReactJS SPA) resides in a repository [audio-device-repo-server](https://github.com/eduarddanziger/audio-device-repo-server/)

## Development environment

- The ASP.NET Core server can be started via Terminal using the following command:

```powershell or bash
cd DeviceRepoAspNetCore
dotnet run --launch-profile http
```
