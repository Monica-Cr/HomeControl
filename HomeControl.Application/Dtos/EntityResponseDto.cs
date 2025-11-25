using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeControl.Application.Dtos
{
    public class EntityResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("result")]
        public List<EntityDto> Entities { get; set; }

    }
    public class Attributes
    {
        [JsonPropertyName("friendly_name")]
        public string FriendlyName { get; set; }

        [JsonPropertyName("supported_features")]
        public int SupportedFeatures { get; set; }

        [JsonPropertyName("event_types")]
        public List<string> EventTypes { get; set; }

        [JsonPropertyName("event_type")]
        public object EventType { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("device_class")]
        public string DeviceClass { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("radius")]
        public int? Radius { get; set; }

        [JsonPropertyName("passive")]
        public bool? Passive { get; set; }

        [JsonPropertyName("persons")]
        public List<object> Persons { get; set; }

        [JsonPropertyName("editable")]
        public bool? Editable { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("device_trackers")]
        public List<string> DeviceTrackers { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("next_dawn")]
        public DateTime? NextDawn { get; set; }

        [JsonPropertyName("next_dusk")]
        public DateTime? NextDusk { get; set; }

        [JsonPropertyName("next_midnight")]
        public DateTime? NextMidnight { get; set; }

        [JsonPropertyName("next_noon")]
        public DateTime? NextNoon { get; set; }

        [JsonPropertyName("next_rising")]
        public DateTime? NextRising { get; set; }

        [JsonPropertyName("next_setting")]
        public DateTime? NextSetting { get; set; }

        [JsonPropertyName("elevation")]
        public double? Elevation { get; set; }

        [JsonPropertyName("azimuth")]
        public double? Azimuth { get; set; }

        [JsonPropertyName("rising")]
        public bool? Rising { get; set; }

        [JsonPropertyName("group_members")]
        public List<string> GroupMembers { get; set; }

        [JsonPropertyName("volume_level")]
        public double? VolumeLevel { get; set; }

        [JsonPropertyName("is_volume_muted")]
        public bool? IsVolumeMuted { get; set; }

        [JsonPropertyName("media_content_id")]
        public string MediaContentId { get; set; }

        [JsonPropertyName("media_content_type")]
        public string MediaContentType { get; set; }

        [JsonPropertyName("media_duration")]
        public int? MediaDuration { get; set; }

        [JsonPropertyName("media_position")]
        public int? MediaPosition { get; set; }

        [JsonPropertyName("media_position_updated_at")]
        public DateTime? MediaPositionUpdatedAt { get; set; }

        [JsonPropertyName("media_title")]
        public string MediaTitle { get; set; }

        [JsonPropertyName("media_artist")]
        public string MediaArtist { get; set; }

        [JsonPropertyName("media_album_name")]
        public string MediaAlbumName { get; set; }

        [JsonPropertyName("media_playlist")]
        public string MediaPlaylist { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("shuffle")]
        public bool? Shuffle { get; set; }

        [JsonPropertyName("repeat")]
        public string Repeat { get; set; }

        [JsonPropertyName("queue_size")]
        public int? QueueSize { get; set; }

        [JsonPropertyName("entity_picture")]
        public string EntityPicture { get; set; }

        [JsonPropertyName("min")]
        public int? Min { get; set; }

        [JsonPropertyName("max")]
        public int? Max { get; set; }

        [JsonPropertyName("step")]
        [JsonIgnore]
        public JsonElement Step { get; set; }

        public int? StepValue
        {
            get
            {
                return Step.ValueKind switch
                {
                    JsonValueKind.Number => Step.GetInt32(),
                    JsonValueKind.String => int.TryParse(Step.GetString(), out var s) ? s : null,
                    _ => null
                };
            }
        }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("queue_position")]
        public int? QueuePosition { get; set; }

        [JsonPropertyName("state_class")]
        public string StateClass { get; set; }

        [JsonPropertyName("unit_of_measurement")]
        public string UnitOfMeasurement { get; set; }
    }

    public class Context
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("parent_id")]
        public object ParentId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

    public class EntityDto
    {
        [JsonPropertyName("entity_id")]
        public string EntityId { get; set; }

        [JsonPropertyName("state")]
        public object State { get; set; }

        [JsonPropertyName("attributes")]
        public Attributes Attributes { get; set; }

        [JsonPropertyName("last_changed")]
        public DateTime LastChanged { get; set; }

        [JsonPropertyName("last_reported")]
        public DateTime LastReported { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("context")]
        public Context Context { get; set; }
    }
}
