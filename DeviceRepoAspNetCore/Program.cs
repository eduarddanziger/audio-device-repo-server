using DeviceRepoAspNetCore.Middleware;
using DeviceRepoAspNetCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = false;
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
});
builder.Logging.AddDebug();
// builder.Logging.AddEventSourceLogger(); // ETW (Event Tracing for Windows) or EventPipe (cross-platform).

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<CryptService>();
//builder.Services.AddSingleton<IAudioDeviceStorage, InMemoryAudioDeviceStorage>();
builder.Services.AddSingleton<IAudioDeviceStorage, MongoDbAudioDeviceStorage>();
builder.Services.AddSingleton(new CodeVersionProvider(CodeVersionProvider.ReadFromAssembly()));

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