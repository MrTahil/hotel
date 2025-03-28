﻿using hmz_rt.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace hmz_rt.Models.Services
{
    public class EventService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly TokenService _tokenService;

        public EventService(TokenService tokenService)
        {
            _httpClient = new HttpClient();
            _baseUrl = "https://api.hmzrt.eu/Events";
            _tokenService = tokenService;
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _tokenService.GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<Event>> GetAllEvents()
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_baseUrl}/Geteventsadmin");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Event>>(content);
            }
            else
            {
                throw new Exception($"Hiba történt az események lekérésekor: {response.StatusCode}");
            }
        }

        public async Task<Event> GetEventById(int eventId)
        {
            await SetAuthorizationHeader();
            var allEvents = await GetAllEvents();
            return allEvents.Find(e => e.EventId == eventId);
        }
    }

}
