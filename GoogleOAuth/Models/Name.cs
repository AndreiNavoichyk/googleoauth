using Newtonsoft.Json;

namespace GoogleOAuth.Models
{
    public class Name
    {
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }
    }
}
