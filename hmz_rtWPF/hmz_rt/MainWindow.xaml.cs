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

        // Változók a szerkesztési módhoz
        private bool isEditMode = false;
        private int currentEditStaffId = 0;

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

                var response = await _httpClient.GetAsync("Rooms/GetRoomWith"); 
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

        private async void Border_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {

                dashboardGrid.Visibility = Visibility.Collapsed;
                staffContainer.Visibility = Visibility.Visible;


                staffEditPanel.Visibility = Visibility.Collapsed;
                isEditMode = false;
                currentEditStaffId = 0;


                await LoadStaffToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

 
                dashboardGrid.Visibility = Visibility.Visible;
                staffContainer.Visibility = Visibility.Collapsed;
            }
        }


        private void StaffBackButton_Click(object sender, RoutedEventArgs e)
        {

            staffContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }


        private async Task LoadStaffToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Staff/ListStaff"); // A Staff controller végpontja
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a dolgozók lekérdezésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var staffMembers = JsonSerializer.Deserialize<List<Staff>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (staffMembers == null || staffMembers.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető dolgozók az adatbázisban.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                
                staffListView.ItemsSource = staffMembers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                throw; 
            }
        }

        private void AddStaffButton_Click(object sender, RoutedEventArgs e)
        {

            ClearStaffFormFields();


            isEditMode = false;
            currentEditStaffId = 0;


            staffEditPanel.Visibility = Visibility.Visible;
        }


        private void EditStaffButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = staffListView.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy dolgozót a szerkesztéshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            isEditMode = true;
            currentEditStaffId = selectedStaff.StaffId;


            FirstNameTextBox.Text = selectedStaff.FirstName;
            LastNameTextBox.Text = selectedStaff.LastName;
            EmailTextBox.Text = selectedStaff.Email;
            PhoneTextBox.Text = selectedStaff.PhoneNumber;
            PositionTextBox.Text = selectedStaff.Position;
            SalaryTextBox.Text = selectedStaff.Salary?.ToString();
            DepartmentTextBox.Text = selectedStaff.Department;


            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Content.ToString() == selectedStaff.Status)
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }


            staffEditPanel.Visibility = Visibility.Visible;
        }


        private async void DeleteStaffButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = staffListView.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy dolgozót a törléshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedStaff.LastName} {selectedStaff.FirstName} nevű dolgozót?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteStaff(selectedStaff.StaffId);
            }
        }


        private async Task DeleteStaff(int staffId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);


                var response = await _httpClient.DeleteAsync($"Staff/DeleteStaff/{staffId}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a dolgozó törlésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A dolgozó sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);


                await LoadStaffToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {

            staffEditPanel.Visibility = Visibility.Collapsed;
            isEditMode = false;
            currentEditStaffId = 0;
        }

        private async void SaveStaffButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                    StatusComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!", "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (!decimal.TryParse(SalaryTextBox.Text, out decimal salary))
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes fizetést!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (isEditMode)
                {

                    var updateDto = new UpdateStaffDto
                    {
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Email = EmailTextBox.Text,
                        PhoneNumber = PhoneTextBox.Text,
                        Position = PositionTextBox.Text,
                        Salary = salary,
                        Department = DepartmentTextBox.Text,
                        Status = selectedStatus  // ÚJ
                    };

                    await UpdateStaff(currentEditStaffId, updateDto);
                }
                else
                {

                    var newDto = new NewStaffDto
                    {
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Email = EmailTextBox.Text,
                        PhoneNumber = PhoneTextBox.Text,
                        Position = PositionTextBox.Text,
                        Salary = salary,
                        Status = selectedStatus,
                        Departmen = DepartmentTextBox.Text
                    };

                    await CreateStaff(newDto);
                }

                
                staffEditPanel.Visibility = Visibility.Collapsed;
                isEditMode = false;
                currentEditStaffId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task CreateStaff(NewStaffDto newStaff)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var content = new StringContent(
                    JsonSerializer.Serialize(newStaff, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Staff/CreateStaff", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a dolgozó létrehozásakor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az új dolgozó sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);


                await LoadStaffToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateStaff(int staffId, UpdateStaffDto updateStaff)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var content = new StringContent(
                    JsonSerializer.Serialize(updateStaff, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Staff/UpdateStaff?id={staffId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a dolgozó frissítésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A dolgozó sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);


                await LoadStaffToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ClearStaffFormFields()
        {
            FirstNameTextBox.Text = string.Empty;
            LastNameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            PositionTextBox.Text = string.Empty;
            SalaryTextBox.Text = string.Empty;
            DepartmentTextBox.Text = string.Empty;
            StatusComboBox.SelectedIndex = 0; 
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

    public class Staff
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? DateHired { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName} - {Position}, {Department}";
        }
    }

    public class NewStaffDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public decimal? Salary { get; set; }
        public string Status { get; set; }
        public string Departmen { get; set; }  
    }

    public class UpdateStaffDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public decimal? Salary { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }  
    }

}
