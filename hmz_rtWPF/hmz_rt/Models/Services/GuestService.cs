using hmz_rt.Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hmz_rt.Models.Services
{
    public class GuestService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly TokenService _tokenService;

        public GuestService(TokenService tokenService)
        {
            _httpClient = new HttpClient();
            _baseUrl = "https://api.hmzrt.eu/Guests";
            _tokenService = tokenService;
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _tokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<Guest>> GetAllGuests()
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_baseUrl}/GetAllGuestever");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Guest>>(content);
            }
            else
            {
                throw new Exception($"Hiba történt a vendégek lekérésekor: {response.StatusCode}");
            }
        }

        public async Task<Guest> GetGuestById(int guestId)
        {
            await SetAuthorizationHeader();
            var allGuests = await GetAllGuests();
            return allGuests.Find(g => g.GuestId == guestId);
        }
    }
}
