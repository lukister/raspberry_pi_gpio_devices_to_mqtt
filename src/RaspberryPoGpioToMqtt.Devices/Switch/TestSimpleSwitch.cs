//using RaspberryPiGpioToMqtt.Devices.DTOs;

//namespace RaspberryPoGpioToMqtt.Devices.Switch;

////TODO make internal
//public class TestSimpleSwitch : ISwitch
//{
//    public string Id { get; }
//    public string Name { get; }

//    private readonly string _stateTopic;
//    private readonly string _commandTopic;

//    public event MqttEventHandler StateChanged;

//    public TestSimpleSwitch(string id, string topicPrefix, string name)
//    {
//        Id = id;
//        Name = name;
//        _stateTopic = $"{topicPrefix}/state";
//        _commandTopic = $"{topicPrefix}/command";
//    }

//    public ConfigurationDto[] GetConfigurations()
//    {
//        var @switch = new ConfigurationDto()
//        {
//            Name = $"{Name} switch",
//            StateTopic = _stateTopic,
//            CommandTopic = _commandTopic,
//            UniqueId = $"{Id}_switch",
//        };
//        return [@switch];
//    }

//    public void SetState(StateData stateData)
//    {
//        if (stateData.Topic != _commandTopic)
//            return;

//        Console.WriteLine($"State {stateData.State} changed for {Id}");

//        StateChanged?.Invoke(new StateData(_stateTopic, stateData.State));
//    }
//}