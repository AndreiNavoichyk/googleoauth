using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace GoogleOAuth.Models
{
    public class GoogleSession
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [XmlIgnore]
        private int _expiresIn;
        [JsonProperty("expires_in")]
        public int ExpiresIn
        {
            get { return _expiresIn; }
            set
            {
                _expiresIn = value;
                TokenExpireDateTime = DateTime.Now.AddSeconds(ExpiresIn);
            }
        }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [XmlIgnore]
        public DateTime TokenExpireDateTime { get; set; }
    }
}
