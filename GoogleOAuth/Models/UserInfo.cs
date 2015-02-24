using System.Collections.Generic;
using Newtonsoft.Json;

namespace GoogleOAuth.Models
{
    public class UserInfo
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("placesLived")]
        public List<PlacesLived> PlacesLived { get; set; }

        [JsonProperty("isPlusUser")]
        public bool IsPlusUser { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("ageRange")]
        public AgeRange AgeRange { get; set; }

        [JsonProperty("circledByCount")]
        public int CircledByCount { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }
}
