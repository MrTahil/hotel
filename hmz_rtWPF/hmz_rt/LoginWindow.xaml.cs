using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Security.Cryptography;

namespace RoomListApp
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _credentialsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RoomListApp", "user.dat");

        public LoginWindow()
        {
            InitializeComponent();

            // SSL validáció kikapcsolása (ha szükséges)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7047/UserAccounts/") };

            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            try
            {
                if (File.Exists(_credentialsFile))
                {
                    string[] lines = File.ReadAllLines(_credentialsFile);
                    if (lines.Length >= 2)
                    {
                        txtUsername.Text = Decrypt(lines[0]);
                        txtPassword.Password = Decrypt(lines[1]);
                        rememberMeCheckBox.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a tárolt adatok betöltésekor: {ex.Message}");
            }
        }

        private void SaveCredentials(string username, string password)
        {
            try
            {
                string directory = Path.GetDirectoryName(_credentialsFile);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string[] lines = {
                    Encrypt(username),
                    Encrypt(password)
                };

                File.WriteAllLines(_credentialsFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a bejelentkezési adatok mentésekor: {ex.Message}");
            }
        }

        private void ClearSavedCredentials()
        {
            try
            {
                if (File.Exists(_credentialsFile))
                {
                    File.Delete(_credentialsFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a tárolt adatok törlésekor: {ex.Message}");
            }
        }

        private string Encrypt(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        private string Decrypt(string text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("btnLogin_Click lefutott!");

            btnLogin.IsEnabled = false;

            bool success = await AuthenticateUser(txtUsername.Text, txtPassword.Password);

            if (success)
            {
                if (rememberMeCheckBox.IsChecked == true)
                {
                    SaveCredentials(txtUsername.Text, txtPassword.Password);
                }
                else
                {
                    ClearSavedCredentials();
                }

                MessageBox.Show("Sikeres bejelentkezés, DialogResult = true lesz!");
                MainWindow adminWindow = new MainWindow();
                adminWindow.Show();
                this.Close();
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

                // Token és role mentése
                TokenStorage.AuthToken = result.AccessToken;
                TokenStorage.RefreshToken = result.RefreshToken;
                TokenStorage.Role = result.Role; // ÚJ: Role mentése
                TokenStorage.Username = username;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                MessageBox.Show($"Sikeres bejelentkezés!\nSzerepkör: {result.Role}", "Üdv", MessageBoxButton.OK, MessageBoxImage.Information);
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
}


public class AuthResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
