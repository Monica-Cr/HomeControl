using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.Net.Http.Json;
using HomeControl.Application.InterFaces;
using HomeControl.Application.Dtos.HomeAssistant;

namespace HomeControl.Infrastructure.Clients
{
    public class HomeAssistantClient : IHomeAssistantClient
    {
        private readonly HttpClient _httpClient;
        public HomeAssistantClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task</*List<EntityDto>*/string> GetEntities()
        {
            try
            {
                var response = await _httpClient.GetAsync("states");

                response.EnsureSuccessStatusCode();

                //List<EntityDto>? entities = await response.Content.ReadFromJsonAsync<List<EntityDto>>();

                //return entities ?? new List<EntityDto>();

                // Get raw JSON
                var json = await response.Content.ReadAsStringAsync();
                return json;

            }
            catch( HttpRequestException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
