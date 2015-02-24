using Newtonsoft.Json;

namespace GoogleOAuth.Models
{
    public class Image
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
