using HomeControl.Application.Dtos.HomeAssistant;
using HomeControl.Application.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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


        //[HttpGet("entity")]
        //public async Task<ActionResult> GetEntity(string id)
        //{
        //    //List<EntityDto> entities = await _homeAssistantClient.GetEntities();

        //    string entities = await _homeAssistantClient.GetEntity(id);

        //    return Ok(entities);
        //}

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            var devicesJson = await _wsClient.GetDevicesJsonAsync();
            var json = JsonDocument.Parse(devicesJson);
            return Ok(json.RootElement);
        }

        [HttpGet("entities")]
        public async Task<ActionResult> GetEntities()
        {
            var entitiesJson = await _wsClient.GetEntitiesJsonAsync();
            var json = JsonDocument.Parse(entitiesJson);
            return Ok(json.RootElement);
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
