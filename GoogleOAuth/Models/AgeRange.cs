using Newtonsoft.Json;

namespace GoogleOAuth.Models
{
    public class AgeRange
    {
        [JsonProperty("min")]
        public int Min { get; set; }
    }
}