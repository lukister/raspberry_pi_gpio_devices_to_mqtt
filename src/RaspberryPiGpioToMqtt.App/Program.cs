using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RaspberryPiGpioToMqtt.App.MQTT;
using RaspberryPoGpioToMqtt.Devices;

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

var serviceName = nameof(RaspberryPiGpioToMqtt);

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddOtlpExporter();
});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddOtlpExporter())
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddOtlpExporter());

builder.Services.AddOptions<SensorsOptions>()
    .Bind(builder.Configuration.GetSection(SensorsOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<MqttClientOptions>()
    .Bind(builder.Configuration.GetSection(MqttClientOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<ICommunication, MqttClient>();
builder.Services.TryDecorate<ICommunication, CommunicationLogger>();
builder.Services.AddHostedService<SensorHostedService>();
builder.Services.AddDevices(builder.Configuration);

var app = builder.Build();

app.MapGet("", () => "Welcome to Raspeberry Pi Gpio to Mqtt app");

app.Run();