using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.Net.Http.Json;
using HomeControl.Application.InterFaces;

namespace HomeControl.Infrastructure.Clients
{
    public class HomeAssistantClient : IHomeAssistantClient
    {
        private readonly HttpClient _httpClient;
        public HomeAssistantClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStatesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("states");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return json;
            }
            catch( HttpRequestException e)
            {
                throw; // TODO: Add logging
            }
            catch (Exception e)
            {
                throw; // TODO: Add logging
            }
        }

        public async Task<string> GetServicesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("services");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
            catch (HttpRequestException e)
            {
                throw; // TODO: Add logging
            }
            catch (Exception e)
            {
                throw; // TODO: Add logging
            }
        }

        public async Task<string> GetEntity(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"states/{id}");

                response.EnsureSuccessStatusCode();

                //List<EntityDto>? entities = await response.Content.ReadFromJsonAsync<List<EntityDto>>();

                //return entities ?? new List<EntityDto>();

                // Get raw JSON
                var json = await response.Content.ReadAsStringAsync();
                return json;

            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
