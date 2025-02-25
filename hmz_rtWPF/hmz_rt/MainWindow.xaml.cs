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
        public static string Role { get; set; }
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

            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7047/") };


            if (!string.IsNullOrEmpty(TokenStorage.Username))
            {
                UserNameDisplay.Text = $"Üdvözöljük, {TokenStorage.Username}!";
            }


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

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

        private async void szobak_kez_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {

                dashboardGrid.Visibility = Visibility.Collapsed;
                roomsContainer.Visibility = Visibility.Visible;


                await LoadRoomsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);


                dashboardGrid.Visibility = Visibility.Visible;
                roomsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            
            roomsContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private async Task LoadRoomsToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Rooms/GetRoomWith"); // API végpont
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


                roomsListView.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                throw; 
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            TokenStorage.ClearTokens();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            this.Close();
        }

    }

    // Az adatbázis struktúrájának megfelelő Room osztály
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