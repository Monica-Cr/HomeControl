using HomeControl.Application.Dtos;
using HomeControl.Application.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeAssistantController : ControllerBase
    {
        private readonly IHomeAssistantClient _homeAssistantClient;
        private readonly IHomeAssistantWebsocketClient _wsClient;
        public HomeAssistantController(IHomeAssistantClient homeAssistantClient, IHomeAssistantWebsocketClient wsClient)
        {
            _homeAssistantClient = homeAssistantClient;
            _wsClient = wsClient;
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            var devicesJson = await _wsClient.GetDevicesJsonAsync();

            DevicesResponseDto? devicesResponse = JsonSerializer.Deserialize<DevicesResponseDto>(devicesJson);
            return Ok(devicesResponse.Devices);
        }

        [HttpGet("entities")]
        public async Task<ActionResult> GetEntities()
        {
            var entitiesJson = await _wsClient.GetEntitiesJsonAsync();

            EntityResponseDto? entityResponse = JsonSerializer.Deserialize<EntityResponseDto>(entitiesJson);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return new JsonResult(entityResponse.Entities, options);
        }

        [HttpGet("states")]
        public async Task<ActionResult> GetStates()
        {
            string statesJson = await _homeAssistantClient.GetStatesAsync();
            var json = JsonDocument.Parse(statesJson);
            return Ok(json.RootElement);
        }

        [HttpGet("services")]
        public async Task<ActionResult> GetServices()
        {
            string servicesJson = await _homeAssistantClient.GetServicesAsync();
            var json = JsonDocument.Parse(servicesJson);
            return Ok(json.RootElement);
        }
    }
}
