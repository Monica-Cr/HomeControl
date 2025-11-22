using HomeControl.Application.Dtos.HomeAssistant;
using HomeControl.Application.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("entities")]
        public async Task<ActionResult> GetEntities()
        {
            //List<EntityDto> entities = await _homeAssistantClient.GetEntities();

            string entities = await _homeAssistantClient.GetEntities();

            return Ok(entities);
        }

        [HttpGet("entity")]
        public async Task<ActionResult> GetEntity(string id)
        {
            //List<EntityDto> entities = await _homeAssistantClient.GetEntities();

            string entities = await _homeAssistantClient.GetEntity(id);

            return Ok(entities);
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            var json = await _wsClient.GetDevicesJsonAsync();
            return Content(json, "application/json");
        }

        
    }
}
