
using Integration.Wiz;
using Newtonsoft.Json;
using static Integration.Wiz.WizUdp;

var ip = "192.168.0.187";

Console.WriteLine(JsonConvert.SerializeObject(await GetPilotAsync(ip)));

while(true)
{
    var t = Console.ReadLine();
    if(t == "1")
        await TurnOnAsync(ip);
    else if(t == "0")
        await TurnOffAsync(ip);
}

