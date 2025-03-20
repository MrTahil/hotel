using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class AuthResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
    public class Forgotpass
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class Forgotpass1
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
