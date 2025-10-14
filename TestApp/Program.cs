using MQTTnet;
using Newtonsoft.Json;
using System.Text;
using static Integration.Wiz.WizUdp;


// WIZ example
//var ip = "192.168.0.187";

//Console.WriteLine(JsonConvert.SerializeObject(await GetPilotAsync(ip)));

//while(true)
//{
//    var t = Console.ReadLine();
//    if(t == "1")
//        await TurnOnAsync(ip);
//    else if(t == "0")
//        await TurnOffAsync(ip);
//}



//// ZIGBEE2MQTT example

//// using Home.Zigbee;
//var ctl = new ZigbeeMqttController(
//    brokerHost: "localhost",
//    brokerPort: 1883,
//    username: "",
//    password: "",
//    useTls: false,
//    z2mBase: "zigbee2mqtt");

//await ctl.ConnectAsync();

//// Permit joins for 120s
//await ctl.PermitJoinAsync(true, seconds: 120);

//// Get devices
//var devices = await ctl.GetDevicesAsync();
//Console.WriteLine(JsonConvert.SerializeObject(devices));

//// Clean up
//await ctl.DisconnectAsync();
//await ctl.DisposeAsync();

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Starting Zigbee Test App...");

        var mqttFactory = new MqttClientFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId("zigbee_test_app")
            .Build();
        
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"\n[MQTT] Topic: {topic}");
            Console.WriteLine($"[MQTT] Payload: {payload}");
            if (topic.StartsWith("zigbee2mqtt/") && payload.Contains("joined"))
            {
                Console.WriteLine($"\n✅ Device joined! Topic: {topic}");
                Console.WriteLine($"Payload: {payload}");
            }
            else
            {
                Console.WriteLine($"\n[MQTT] Topic: {topic}");
                Console.WriteLine($"[MQTT] Payload: {payload}");
            }

            return Task.CompletedTask;
        };

        mqttClient.ConnectedAsync += e =>
        {
            Console.WriteLine("Connected to MQTT broker.");
            return Task.CompletedTask;
        };

        mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("Disconnected from MQTT broker.");
            return Task.CompletedTask;
        };

        try
        {
            Console.WriteLine("Connecting to MQTT broker...");
            await mqttClient.ConnectAsync(options);
            Console.WriteLine("Connected!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex.Message}");
            Console.ReadKey();
            return;
        }

        await mqttClient.SubscribeAsync("zigbee2mqtt/bridge/#");
        await mqttClient.SubscribeAsync("zigbee2mqtt/logging");
        await mqttClient.SubscribeAsync("zigbee2mqtt/bridge/logging");
        Console.WriteLine("Subscribed to Zigbee2MQTT bridge topics.");

        Console.WriteLine("Enabling Zigbee pairing for 90 seconds...");
        var payload = "{\"id\":1,\"type\":\"request\",\"command\":\"permit_join\",\"payload\":{\"value\":true,\"time\":90}}";

        var enableJoinMsg = new MqttApplicationMessageBuilder()
            .WithTopic("zigbee2mqtt/bridge/request")
            .WithPayload(payload)
            .Build();

        await mqttClient.PublishAsync(enableJoinMsg);


        Console.WriteLine("Put your device in pairing mode.");
        Console.WriteLine("Press enter");
        Console.ReadKey();


        Console.WriteLine("Disabling Zigbee pairing...");
        var payloadDisable = "{\"id\":2,\"type\":\"request\",\"command\":\"permit_join\",\"payload\":{\"value\":false}}";

        var disableJoinMsg = new MqttApplicationMessageBuilder()
            .WithTopic("zigbee2mqtt/bridge/request")
            .WithPayload(payloadDisable)
            .Build();

        await mqttClient.PublishAsync(disableJoinMsg);

        Console.WriteLine("Pairing mode ended. Press enter exit...");
        Console.ReadKey();

        await mqttClient.DisconnectAsync();
    }
}


