# Audio Device Repository Server (ASP.NET Core with REST API)

- The backend of Audio Device Repository Server as a ASP.Net Core Server with REST API (C# / MongoDB).

- The GUI client (Next.js / React / TypeScript) deployed on Vercel: [https://list-audio-react-app.vercel.app](https://list-audio-react-app.vercel.app)

- For Audio Device REST API description see [rest-api-documentation.md](DeviceRepoAspNetCore/rest-api-documentation.md).

- Client Apps start the Audio Device Repository Server on-demand, if it doesn't run already.

- The main GUI client's repository is [list-audio-react-app](https://github.com/eduarddanziger/list-audio-react-app/)

## Development environment

- The ASP.NET Core server can be started via Terminal using the following command:

```powershell or bash
cd DeviceRepoAspNetCore
dotnet run --launch-profile http
```
