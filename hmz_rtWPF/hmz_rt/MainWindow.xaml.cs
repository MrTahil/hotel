using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RoomListApp
{
    public static class TokenStorage
    {
        public static string AuthToken { get; set; }
    }
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7047/Rooms/") };

        public MainWindow()
        {
            InitializeComponent();
            _ = LoadRooms();
        }


        private async Task LoadRooms()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("GetRoomWith"); // API végpont
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a szobák lekérdezésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var rooms = JsonSerializer.Deserialize<List<Room>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (rooms == null || rooms.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető szobák az adatbázisban.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                lstRooms.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}\n{ex.StackTrace}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRooms.SelectedItem is Room selectedRoom)
            {
                DetailsPanel.DataContext = selectedRoom;
            }
        }
    }

    public class Room
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public int? Capacity { get; set; }
        public decimal? PricePerNight { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? FloorNumber { get; set; }
        public string Amenities { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Images { get; set; }

        public override string ToString()
        {
            return $"{RoomNumber} - {RoomType} ({PricePerNight} Ft/éj), Kapacitás: {Capacity} fő, Állapot: {Status}";
        }
    }
}
