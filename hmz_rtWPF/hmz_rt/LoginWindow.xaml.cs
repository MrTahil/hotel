using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        
        private string _forgotPasswordEmail;

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

                MessageBox.Show("Sikeres bejelentkezés!", "Üdv", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow adminWindow = new MainWindow();
                adminWindow.Show();
                this.Close();
            }
            else
            {
                btnLogin.IsEnabled = true;
                MessageBox.Show("Bejelentkezés sikertelen.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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

                TokenStorage.AuthToken = result.AccessToken;
                TokenStorage.RefreshToken = result.RefreshToken;
                TokenStorage.Role = result.Role;
                TokenStorage.Username = username;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

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

        #region Panelkezelés a jelszó-visszaállításhoz

        private void ShowPanel(string panelName)
        {
            loginPanel.Visibility = Visibility.Collapsed;
            forgotPasswordEmailPanel.Visibility = Visibility.Collapsed;
            forgotPasswordCodePanel.Visibility = Visibility.Collapsed;
            forgotPasswordNewPasswordPanel.Visibility = Visibility.Collapsed;

            switch (panelName)
            {
                case "login":
                    loginPanel.Visibility = Visibility.Visible;
                    break;
                case "forgotEmail":
                    forgotPasswordEmailPanel.Visibility = Visibility.Visible;
                    break;
                case "forgotCode":
                    forgotPasswordCodePanel.Visibility = Visibility.Visible;
                    break;
                case "forgotNewPassword":
                    forgotPasswordNewPasswordPanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        #endregion

        #region Elfelejtett jelszó eseménykezelők

        private void ForgotPasswordText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowPanel("forgotEmail");
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("login");
        }

        private void BackToForgotEmail_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("forgotEmail");
        }

        private void BackToVerifyCode_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("forgotCode");
        }

        private async void SendCode_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(txtForgotEmail.Text)) {
                MessageBox.Show("Kérjük, adja meg az email címét!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnSendCode.IsEnabled = false;
            _forgotPasswordEmail = txtForgotEmail.Text;

            try {
                var response = await _httpClient.PostAsync($"ForgotPasswordsendemail/{_forgotPasswordEmail}", null);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("Az ellenőrző kódot elküldtük a megadott email címre.", "Kód elküldve", MessageBoxButton.OK, MessageBoxImage.Information);
                    ShowPanel("forgotCode");
                }
                else {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba történt a kód küldésekor: {responseContent}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally {
                btnSendCode.IsEnabled = true;
            }
        }


        private async void VerifyCode_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(txtVerificationCode.Text)) {
                MessageBox.Show("Kérjük, adja meg az ellenőrző kódot!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnVerifyCode.IsEnabled = false;

            try {
                var verifyData = new Forgotpass {
                    Email = _forgotPasswordEmail,
                    Code = txtVerificationCode.Text
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(verifyData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync("VerifyTheforgotpass", content);

                if (response.StatusCode == HttpStatusCode.Created) 
                {
                    MessageBox.Show("A kód ellenőrzése sikeres! Most megadhatja az új jelszavát.", "Sikeres ellenőrzés", MessageBoxButton.OK, MessageBoxImage.Information);
                    ShowPanel("forgotNewPassword");
                }
                else {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("A megadott kód hibás vagy lejárt.", "Érvénytelen kód", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally {
                btnVerifyCode.IsEnabled = true;
            }
        }


        private async void SetNewPassword_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(txtNewPassword.Password)) {
                MessageBox.Show("Kérjük, adja meg az új jelszavát!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtNewPassword.Password != txtConfirmPassword.Password) {
                MessageBox.Show("Az új jelszó és a megerősítés nem egyezik!", "Eltérő jelszavak", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnSetNewPassword.IsEnabled = false;

            try {
                var resetData = new Forgotpass1 {
                    Email = _forgotPasswordEmail,
                    Password = txtNewPassword.Password
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(resetData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync("SetNewPassword", content);

                if (response.StatusCode == HttpStatusCode.Created) 
                {
                    MessageBox.Show("Az új jelszó sikeresen beállítva! Most már bejelentkezhet az új jelszavával.", "Sikeres jelszóváltoztatás", MessageBoxButton.OK, MessageBoxImage.Information);


                    ShowPanel("login");

                    txtUsername.Text = _forgotPasswordEmail;
                    txtPassword.Password = string.Empty;
                    txtPassword.Focus();
                }
                else {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba történt az új jelszó beállításakor: {responseContent}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally {
                btnSetNewPassword.IsEnabled = true;
            }
        }


        #endregion
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
    public class Forgotpass {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class Forgotpass1 {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
