using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RoomListApp
{
    public static class TokenStorage
    {
        public static string AuthToken { get; set; }
        public static string RefreshToken { get; set; }
        public static string Role { get; set; }  // ÚJ: Role tárolása
        public static string Username { get; set; }

        public static void ClearTokens()
        {
            AuthToken = null;
            RefreshToken = null;
            Username = null;
            Role = null;
        }
    }

    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            // SSL beállítások
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7047/Rooms/") };

            // Aszinkron hívás, de ne zárd be az ablakot hibák esetén
            _ = LoadRooms();

            // Display the logged-in username
            if (!string.IsNullOrEmpty(TokenStorage.Username))
            {
                UserNameDisplay.Text = TokenStorage.Username;
            }

            // Set up a timer to update the current time
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Initialize the time display
            UpdateTimeDisplay();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }


        private async Task LoadRooms()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("https://localhost:7047/Rooms/GetRoomWith"); // API végpont
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


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void szobak_kez_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            // Make sure to stop the timer to prevent memory leaks
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            // Clear tokens and authentication information
            TokenStorage.ClearTokens();

            // Create and show the login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            // Close this window
            this.Close();
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

