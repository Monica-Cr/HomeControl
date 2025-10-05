
using Integration.Wiz;
using Newtonsoft.Json;
using static Integration.Wiz.WizUdp;
using Integration.Zigbee;


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



// ZIGBEE2MQTT example

// using Home.Zigbee;
var ctl = new ZigbeeMqttController(
    brokerHost: "localhost",
    brokerPort: 1883,
    username: "",
    password: "",
    useTls: false,
    z2mBase: "zigbee2mqtt");

await ctl.ConnectAsync();

// Permit joins for 120s
await ctl.PermitJoinAsync(true, seconds: 120);

// Get devices
var devices = await ctl.GetDevicesAsync();
Console.WriteLine(JsonConvert.SerializeObject(devices));

// Clean up
await ctl.DisconnectAsync();
await ctl.DisposeAsync();


