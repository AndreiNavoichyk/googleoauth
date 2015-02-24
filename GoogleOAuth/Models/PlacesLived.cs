using Newtonsoft.Json;

namespace GoogleOAuth.Models
{
    public class PlacesLived
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("primary")]
        public bool Primary { get; set; }
    }
}
