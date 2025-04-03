using DeviceRepoAspNetCore.Middleware;
using DeviceRepoAspNetCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
#pragma warning disable CS0618 // Type or member is obsolete
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
#pragma warning restore CS0618 // Type or member is obsolete
});
builder.Logging.AddDebug();
// builder.Logging.AddEventSourceLogger(); // ETW (Event Tracing for Windows) or EventPipe (cross-platform).

// Add services to the container.
builder.Services.AddSingleton<IAudioDeviceStorage, InMemoryAudioDeviceStorage>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();

// Use my middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactApp"); // Add this line to use the CORS policy

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();