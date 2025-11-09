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
        public HomeAssistantController(IHomeAssistantClient homeAssistantClient)
        {
            _homeAssistantClient = homeAssistantClient;
        }

        [HttpGet("entities")]
        public async Task<ActionResult> GetEntities()
        {
            //List<EntityDto> entities = await _homeAssistantClient.GetEntities();

            string entities = await _homeAssistantClient.GetEntities();

            return Ok(entities);
        }
    }
}
