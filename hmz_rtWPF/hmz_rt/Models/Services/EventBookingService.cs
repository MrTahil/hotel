using hmz_rt.Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Services
{
    public class EventBookingsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly TokenService _tokenService;

        public EventBookingsService(TokenService tokenService)
        {
            _httpClient = new HttpClient();
            _baseUrl = "https://api.hmzrt.eu/Feedback";
            _tokenService = tokenService;
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _tokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<EventBookings>> GetAllEventBookings()
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_baseUrl}/GetEventBookings");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Közvetlen deszerializálás listára
                return JsonConvert.DeserializeObject<List<EventBookings>>(content);
            }
            else
            {
                throw new Exception($"Hiba történt az eseményfoglalások lekérésekor: {response.StatusCode}");
            }
        }

        public async Task<bool> DeleteEventBooking(int id)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/DeleteBookingById/{id}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception($"Hiba történt a foglalás törlésekor: {content}");
            }
        }

        public async Task<bool> CreateEventBooking(CreateEventBooking booking, int eventId)
        {
            await SetAuthorizationHeader();
            var json = JsonConvert.SerializeObject(booking);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/NewEvenBooking/{eventId}", content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Hiba történt a foglalás létrehozásakor: {responseContent}");
            }
        }

        // Event lekérése ID alapján
        public async Task<Event> GetEventById(int eventId)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_baseUrl}/GetEventById/{eventId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<Event>>(content);
                return result.Data;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Event>> GetAllEvents()
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_baseUrl}/GetAllEvents");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<Event>>>(content);
                return result.Data;
            }
            else
            {
                throw new Exception($"Hiba történt az események lekérésekor: {response.StatusCode}");
            }
        }
    }

    public class ApiResponse<T>
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
