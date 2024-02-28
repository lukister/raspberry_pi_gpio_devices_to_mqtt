using RaspberryPiGpioToMqtt.App;
using RaspberryPiGpioToMqtt.App.MQTT;

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<MqttClientOptions>(
    builder.Configuration.GetSection(MqttClientOptions.SectionName));
builder.Services.Configure<DeviceRepository.Options>(
    builder.Configuration.GetSection(DeviceRepository.Options.SectionName));

builder.Services.AddSingleton<MqttClient>();
builder.Services.AddHostedService<SensorHostedService>();
builder.Services.AddSingleton<DeviceRepository>();

var app = builder.Build();

app.Run();