using RaspberryPiGpioToMqtt.App.MQTT;
using RaspberryPoGpioToMqtt.Devices;

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<MqttClientOptions>(
    builder.Configuration.GetSection(MqttClientOptions.SectionName));


builder.Services.AddSingleton<ICommunication, MqttClient>();
builder.Services.AddHostedService<SensorHostedService>();
builder.Services.AddDevices(builder.Configuration);

var app = builder.Build();

app.Run();