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

        // Változók a promóció szerkesztési módhoz
        private bool isEditPromotion = false;
        private int currentEditPromotionId = 0;

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

        private async void Staff_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

                var response = await _httpClient.GetAsync("Staff/ListStaff");
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
                        Status = selectedStatus
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

        private async void PromotionsCard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                
                dashboardGrid.Visibility = Visibility.Collapsed;
                promotionsContainer.Visibility = Visibility.Visible;

                
                promotionEditPanel.Visibility = Visibility.Collapsed;
                isEditPromotion = false;
                currentEditPromotionId = 0;

                await LoadPromotionsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

                dashboardGrid.Visibility = Visibility.Visible;
                promotionsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void PromotionsBackButton_Click(object sender, RoutedEventArgs e)
        {
            promotionsContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private async Task LoadPromotionsToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Promotions/Getpromotions");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a promóciók lekérdezésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var promotionsFromApi = JsonSerializer.Deserialize<List<Promotion>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                promotionsListView.ItemsSource = promotionsFromApi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPromotionFormFields();

            isEditPromotion = false;
            currentEditPromotionId = 0;

            promotionEditPanel.Visibility = Visibility.Visible;
        }

        private void EditPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPromotion = promotionsListView.SelectedItem as Promotion;
            if (selectedPromotion == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy promóciót a szerkesztéshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Szerkesztési mód beállítása
            isEditPromotion = true;
            currentEditPromotionId = selectedPromotion.PromotionId;

            // Mezők feltöltése
            PromotionNameTextBox.Text = selectedPromotion.PromotionName;
            DescriptionTextBox.Text = selectedPromotion.Description;
            StartDatePicker.SelectedDate = selectedPromotion.StartDate;
            EndDatePicker.SelectedDate = selectedPromotion.EndDate;
            DiscountPercentageTextBox.Text = selectedPromotion.DiscountPercentage?.ToString();
            TermsConditionsTextBox.Text = selectedPromotion.TermsConditions;
            RoomIdTextBox.Text = selectedPromotion.RoomId?.ToString();

            // Státusz beállítása
            foreach (ComboBoxItem item in PromotionStatusComboBox.Items)
            {
                if (item.Content.ToString() == selectedPromotion.Status)
                {
                    PromotionStatusComboBox.SelectedItem = item;
                    break;
                }
            }

            // Megjelenítjük a szerkesztő panelt
            promotionEditPanel.Visibility = Visibility.Visible;
        }

        private async void DeletePromotionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPromotion = promotionsListView.SelectedItem as Promotion;
            if (selectedPromotion == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy promóciót a törléshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedPromotion.PromotionName} nevű promóciót?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeletePromotion(selectedPromotion.PromotionId);
            }
        }


        private async Task DeletePromotion(int promotionId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.DeleteAsync($"Promotions/Deletepromotion/{promotionId}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a promóció törlésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A promóció sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadPromotionsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelPromotionEditButton_Click(object sender, RoutedEventArgs e)
        {
            promotionEditPanel.Visibility = Visibility.Collapsed;
            isEditPromotion = false;
            currentEditPromotionId = 0;
        }

        private async void SavePromotionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PromotionNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DiscountPercentageTextBox.Text) ||
                    StartDatePicker.SelectedDate == null ||
                    EndDatePicker.SelectedDate == null ||
                    PromotionStatusComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!", "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (!int.TryParse(DiscountPercentageTextBox.Text, out int discountPercentage) || discountPercentage < 0 || discountPercentage > 100)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes kedvezmény százalékot (0-100)!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
                {
                    MessageBox.Show("A kezdő dátum nem lehet későbbi, mint a befejező dátum!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int roomId = 0;
                if (!string.IsNullOrWhiteSpace(RoomIdTextBox.Text))
                {
                    if (string.IsNullOrWhiteSpace(RoomIdTextBox.Text) || !int.TryParse(RoomIdTextBox.Text, out roomId))
                    {
                        MessageBox.Show("Kérjük, adjon meg egy érvényes szobaazonosítót!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var selectedStatus = (PromotionStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (isEditPromotion)
                {
                    var updateDto = new PromotionUpdateDto
                    {
                        Name = PromotionNameTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        StartDate = StartDatePicker.SelectedDate,
                        EndDate = EndDatePicker.SelectedDate,
                        DiscountPercentage = discountPercentage,
                        TermsConditions = TermsConditionsTextBox.Text,
                        Status = selectedStatus
                    };

                    await UpdatePromotion(currentEditPromotionId, updateDto);
                }
                else
                {
                    var newDto = new PromotionCreateDto
                    {
                        Name = PromotionNameTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        StartDate = StartDatePicker.SelectedDate,
                        EndDate = EndDatePicker.SelectedDate,
                        DiscountPercentage = discountPercentage,
                        TermsConditions = TermsConditionsTextBox.Text,
                        Status = selectedStatus,
                        RoomId = roomId 
                    };


                    await CreatePromotion(newDto);
                }

                promotionEditPanel.Visibility = Visibility.Collapsed;
                isEditPromotion = false;
                currentEditPromotionId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CreatePromotion(PromotionCreateDto newPromotion)
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
                    JsonSerializer.Serialize(newPromotion, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Promotions/CreatePromotion", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a promóció létrehozásakor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az új promóció sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadPromotionsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdatePromotion(int promotionId, PromotionUpdateDto updatePromotion)
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
                    JsonSerializer.Serialize(updatePromotion, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Promotions/UpdatePromotion/{promotionId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a promóció frissítésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A promóció sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadPromotionsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearPromotionFormFields()
        {
            PromotionNameTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(30);
            DiscountPercentageTextBox.Text = string.Empty;
            TermsConditionsTextBox.Text = string.Empty;
            RoomIdTextBox.Text = string.Empty;
            PromotionStatusComboBox.SelectedIndex = 0;
        }
    }

    //Room osztályok
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

    //Staff Osztályok
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

    // Promotion osztályok
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string TermsConditions { get; set; }
        public string Status { get; set; }
        public int? RoomId { get; set; }
        public DateTime? DateAdded { get; set; }

        public override string ToString()
        {
            return $"{PromotionName} - {DiscountPercentage}% kedvezmény, {StartDate?.ToString("yyyy.MM.dd")} - {EndDate?.ToString("yyyy.MM.dd")}";
        }
    }

    public class PromotionCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string TermsConditions { get; set; }
        public string Status { get; set; }
        public int? RoomId { get; set; }
    }

    public class PromotionUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string TermsConditions { get; set; }
        public string Status { get; set; }

    }
}
