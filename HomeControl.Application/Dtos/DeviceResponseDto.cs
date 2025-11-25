using System.Text.Json.Serialization;

namespace HomeControl.Application.Dtos
{
    public class DevicesResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("result")]
        public List<DeviceDto> Devices { get; set; }
    }
    public class DeviceDto
    {

        [JsonPropertyName("area_id")]
        public string AreaId { get; set; }
        
        [JsonPropertyName("connections")]
        public List<List<string>> Connections { get; set; }

        [JsonPropertyName("created_at")]
        public double CreatedAt { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("identifiers")]
        public List<List<string>> Identifiers { get; set; }

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("modified_at")]
        public double ModifiedAt { get; set; }

        [JsonPropertyName("name_by_user")]
        public string NameByUser { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("primary_config_entry")]
        public string PrimaryConfigEntry { get; set; }

        [JsonPropertyName("sw_version")]
        public string SwVersion { get; set; }

    }
}
