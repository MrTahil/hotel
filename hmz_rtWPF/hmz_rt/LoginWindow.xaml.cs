using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RoomListApp
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;

        public LoginWindow()
        {
            InitializeComponent();

            // SSL validáció kikapcsolása (ha szükséges)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7047/UserAccounts/") };
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("btnLogin_Click lefutott!");

            btnLogin.IsEnabled = false;

            bool success = await AuthenticateUser(txtUsername.Text, txtPassword.Password);

            if (success)
            {
                MessageBox.Show("Sikeres bejelentkezés, DialogResult = true lesz!");
                this.DialogResult = true;
            }
            else
            {
                btnLogin.IsEnabled = true;
                MessageBox.Show("Bejelentkezés sikertelen.");
            }
        }









        private async Task<bool> AuthenticateUser(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                string json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("Login", content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Bejelentkezési hiba ({response.StatusCode}): {responseString}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var result = JsonSerializer.Deserialize<AuthResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                TokenStorage.AuthToken = result.Token;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

                MessageBox.Show("Sikeres bejelentkezés!", "Üdv", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Hálózati hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ismeretlen hiba történt: {ex.Message}\n{ex.StackTrace}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
    }
}
