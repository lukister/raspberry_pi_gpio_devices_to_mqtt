using RaspberryPiGpioToMqtt.App.MQTT;

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<MqttClientOptions>(
    builder.Configuration.GetSection(MqttClientOptions.SectionName));

builder.Services.AddSingleton<MqttClient>();
builder.Services.AddHostedService<SensorHostedService>();

var app = builder.Build();

app.Run();