using hmz_rt.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;

        public TokenService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://api.hmzrt.eu/") };
        }

        public async Task<string> GetToken()
        {
            // Egyszerűen visszaadjuk a TokenStorage-ban tárolt tokent
            return TokenStorage.AuthToken;
        }
    }
}
