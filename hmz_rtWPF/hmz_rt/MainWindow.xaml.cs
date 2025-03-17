using hmz_rt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RoomListApp
{
    public static class TokenStorage
    {
        public static string AuthToken { get; set; }
        public static string RefreshToken { get; set; }
        public static string Role { get; set; }
        public static string Username { get; set; }

        public static int UserId { get; set; }


        public static void ClearTokens()
        {
            AuthToken = null;
            RefreshToken = null;
            Username = null;
            Role = null;
            UserId = 0;
        }
    }

    public class StatusToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString();
            switch (status)
            {
                case "Jóváhagyva":
                case "Függőben":
                    return "Check In";
                case "Checked In":
                    return "Check Out";
                case "Finished":
                    return "Lezárva";
                case "Lemondva":
                    return "Lemondva";
                default:
                    return status;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }

    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            int rating = (int)value;
            StackPanel starPanel = new StackPanel { Orientation = Orientation.Horizontal };

            for (int i = 1; i <= 10; i++)
            {
                TextBlock star = new TextBlock
                {
                    Text = "★",
                    FontSize = 16,
                    Foreground = i <= rating ? Brushes.Gold : Brushes.LightGray,
                    Margin = new Thickness(1, 0, 1, 0)
                };

                starPanel.Children.Add(star);
            }

            return starPanel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Brushes.LightGray;

            int rating = (int)value;
            int position = (int)parameter;

            return (position <= rating) ? Brushes.Gold : Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status?.ToLower().Trim())
                {
                    case "fizetve":
                        return Brushes.Green;
                    case "fizetésre vár":
                    case "fizetesre var":
                        return Brushes.OrangeRed;
                    default:
                        return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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

        //Változók a Guest szerkesztési módhoz
        private bool isEditGuest = false;
        private int currentEditGuestId = 0;
        //Változók a Booking szerkesztési módhoz
        private bool isEditBooking = false;
        private int currentEditBookingId = 0;


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
            StaffEmailTextBox.Text = selectedStaff.Email;
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
                    string.IsNullOrWhiteSpace(StaffEmailTextBox.Text) ||
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
                        Email = StaffEmailTextBox.Text,
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
                        Email = StaffEmailTextBox.Text,
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
            StaffEmailTextBox.Text = string.Empty;
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

            isEditPromotion = true;
            currentEditPromotionId = selectedPromotion.PromotionId;


            PromotionNameTextBox.Text = selectedPromotion.PromotionName;
            DescriptionTextBox.Text = selectedPromotion.Description;
            StartDatePicker.SelectedDate = selectedPromotion.StartDate;
            EndDatePicker.SelectedDate = selectedPromotion.EndDate;
            DiscountPercentageTextBox.Text = selectedPromotion.DiscountPercentage?.ToString();
            TermsConditionsTextBox.Text = selectedPromotion.TermsConditions;
            RoomIdTextBox.Text = selectedPromotion.RoomId?.ToString();


            foreach (ComboBoxItem item in PromotionStatusComboBox.Items)
            {
                if (item.Content.ToString() == selectedPromotion.Status)
                {
                    PromotionStatusComboBox.SelectedItem = item;
                    break;
                }
            }


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


                if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
                {
                    MessageBox.Show("A kezdő dátum nem lehet későbbi, mint a befejező dátum!",
                        "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(DiscountPercentageTextBox.Text, out int discountPercentage) ||
                    discountPercentage < 0 || discountPercentage > 100)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes kedvezmény százalékot (0-100)!",
                        "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        Status = selectedStatus,
                        RoomId = string.IsNullOrWhiteSpace(RoomIdTextBox.Text)
                        ? (int?)null
                        : int.Parse(RoomIdTextBox.Text)
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

        private async void Guest_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                dashboardGrid.Visibility = Visibility.Collapsed;
                guestsContainer.Visibility = Visibility.Visible;

                guestEditPanel.Visibility = Visibility.Collapsed;
                isEditGuest = false;
                currentEditGuestId = 0;

                await LoadGuestsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

                dashboardGrid.Visibility = Visibility.Visible;
                guestsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void GuestsBackButton_Click(object sender, RoutedEventArgs e)
        {
            guestsContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private async Task LoadGuestsToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Guests/GetAllGuestever");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a vendégek lekérdezésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var guests = JsonSerializer.Deserialize<List<Guest>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (guests == null || guests.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető vendégek az adatbázisban.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                guestsListView.ItemsSource = guests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void AddGuestButton_Click(object sender, RoutedEventArgs e)
        {
            ClearGuestFormFields();

            isEditGuest = false;
            currentEditGuestId = 0;

            /* Új vendég létrehozásánál is csak olvasható a UserId mező,
            mert azt automatikusan töltjük ki az email alapján*/
            GuestUserIdTextBox.IsReadOnly = true;
            GuestUserIdTextBox.Background = Brushes.LightGray;

            guestEditPanel.Visibility = Visibility.Visible;
        }

        private void EditGuestButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGuest = guestsListView.SelectedItem as Guest;
            if (selectedGuest == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy vendéget a szerkesztéshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isEditGuest = true;
            currentEditGuestId = selectedGuest.GuestId;

            GuestFirstNameTextBox.Text = selectedGuest.FirstName;
            GuestLastNameTextBox.Text = selectedGuest.LastName;
            GuestEmailTextBox.Text = selectedGuest.Email;
            GuestPhoneTextBox.Text = selectedGuest.PhoneNumber;
            GuestAddressTextBox.Text = selectedGuest.Address;
            GuestCityTextBox.Text = selectedGuest.City;
            GuestCountryTextBox.Text = selectedGuest.Country;
            GuestDateOfBirthPicker.SelectedDate = selectedGuest.DateOfBirth;
            GuestUserIdTextBox.Text = selectedGuest.UserId?.ToString();

            GuestUserIdTextBox.IsReadOnly = true;
            GuestUserIdTextBox.Background = Brushes.LightGray;

            foreach (ComboBoxItem item in GuestGenderComboBox.Items)
            {
                if (item.Content.ToString() == selectedGuest.Gender)
                {
                    GuestGenderComboBox.SelectedItem = item;
                    break;
                }
            }

            guestEditPanel.Visibility = Visibility.Visible;
        }

        private async void DeleteGuestButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGuest = guestsListView.SelectedItem as Guest;
            if (selectedGuest == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy vendéget a törléshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedGuest.LastName} {selectedGuest.FirstName} nevű vendéget?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteGuest(selectedGuest.GuestId);
            }
        }

        private void CancelGuestEditButton_Click(object sender, RoutedEventArgs e)
        {
            guestEditPanel.Visibility = Visibility.Collapsed;
            isEditGuest = false;
            currentEditGuestId = 0;
        }

        private async void SaveGuestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GuestFirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(GuestLastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(GuestEmailTextBox.Text) ||
                    GuestGenderComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!", "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedGender = (GuestGenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (isEditGuest)
                {
                    var updateDto = new UpdateGuest
                    {
                        FirstName = GuestFirstNameTextBox.Text,
                        LastName = GuestLastNameTextBox.Text,
                        Email = GuestEmailTextBox.Text,
                        PhoneNumber = GuestPhoneTextBox.Text,
                        Address = GuestAddressTextBox.Text,
                        City = GuestCityTextBox.Text,
                        Country = GuestCountryTextBox.Text,
                        DateOfBirth = GuestDateOfBirthPicker.SelectedDate,
                        Gender = selectedGender
                    };

                    await UpdateGuest(currentEditGuestId, updateDto);
                }
                else
                {

                    int? userId = await GetUserIdByEmail(GuestEmailTextBox.Text);
                    if (userId == null)
                    {
                        MessageBox.Show("Ezzel az email címmel nincs létrehozva felhasználói fiók!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var createDto = new CreateGuest
                    {
                        FirstName = GuestFirstNameTextBox.Text,
                        LastName = GuestLastNameTextBox.Text,
                        Email = GuestEmailTextBox.Text,
                        PhoneNumber = GuestPhoneTextBox.Text,
                        Address = GuestAddressTextBox.Text,
                        City = GuestCityTextBox.Text,
                        Country = GuestCountryTextBox.Text,
                        DateOfBirth = GuestDateOfBirthPicker.SelectedDate,
                        Gender = selectedGender,
                        UserId = userId
                    };

                    await CreateGuest(createDto);
                }

                guestEditPanel.Visibility = Visibility.Collapsed;
                isEditGuest = false;
                currentEditGuestId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<int?> GetUserIdByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync($"UserAccounts/GetId/{email}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }

                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a felhasználó azonosító lekérésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var userId = JsonSerializer.Deserialize<int>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return userId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a felhasználó azonosító lekérésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private async void GuestEmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isEditGuest && !string.IsNullOrWhiteSpace(GuestEmailTextBox.Text))
            {
                int? userId = await GetUserIdByEmail(GuestEmailTextBox.Text);
                if (userId != null)
                {
                    GuestUserIdTextBox.Text = userId.ToString();
                }
                else
                {
                    GuestUserIdTextBox.Text = "";
                    MessageBox.Show("Figyelem: Ezzel az email címmel nincs létrehozva felhasználói fiók!",
                        "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async Task CreateGuest(CreateGuest newGuest)
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
                    JsonSerializer.Serialize(newGuest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Guests/Addnewguest", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a vendég létrehozásakor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az új vendég sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadGuestsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateGuest(int guestId, UpdateGuest updateGuest)
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
                    JsonSerializer.Serialize(updateGuest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Guests/UpdateGuest/{guestId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a vendég frissítésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A vendég adatai sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadGuestsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task DeleteGuest(int guestId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.DeleteAsync($"Guests/DeleteGuest/{guestId}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a vendég törlésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A vendég sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadGuestsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearGuestFormFields()
        {
            GuestFirstNameTextBox.Text = string.Empty;
            GuestLastNameTextBox.Text = string.Empty;
            GuestEmailTextBox.Text = string.Empty;
            GuestPhoneTextBox.Text = string.Empty;
            GuestAddressTextBox.Text = string.Empty;
            GuestCityTextBox.Text = string.Empty;
            GuestCountryTextBox.Text = string.Empty;
            GuestDateOfBirthPicker.SelectedDate = null;
            GuestUserIdTextBox.Text = string.Empty;
            GuestGenderComboBox.SelectedIndex = -1;
        }


        private async void BookingsCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                dashboardGrid.Visibility = Visibility.Collapsed;
                bookingsContainer.Visibility = Visibility.Visible;

                bookingEditPanel.Visibility = Visibility.Collapsed;
                isEditBooking = false;
                currentEditBookingId = 0;

                await LoadBookingsToListView();
                await LoadRoomsComboBox();
                await LoadGuestsComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

                dashboardGrid.Visibility = Visibility.Visible;
                bookingsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void BookingsBackButton_Click(object sender, RoutedEventArgs e)
        {
            bookingsContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private async Task LoadBookingsToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Bookings/Getalldat");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a foglalások lekérdezésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var bookings = JsonSerializer.Deserialize<List<Booking>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (bookings == null || bookings.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető foglalások az adatbázisban.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    bookingsListView.ItemsSource = null;
                    return;
                }

                var feedbacksResponse = await _httpClient.GetAsync("Feedback/GetallFeedback");
                List<Feedback> allFeedbacks = new List<Feedback>();

                if (feedbacksResponse.IsSuccessStatusCode)
                {
                    var feedbacksString = await feedbacksResponse.Content.ReadAsStringAsync();
                    allFeedbacks = JsonSerializer.Deserialize<List<Feedback>>(feedbacksString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var bookingsWithDetails = new List<BookingViewModel>();

                foreach (var booking in bookings)
                {
                    var guest = await GetGuestById(booking.GuestId ?? 0);
                    var room = await GetRoomById(booking.RoomId ?? 0);

                    bool hasFeedback = allFeedbacks.Any(f => f.GuestId == booking.GuestId);

                    bookingsWithDetails.Add(new BookingViewModel
                    {
                        BookingId = booking.BookingId,
                        GuestId = booking.GuestId,
                        GuestName = guest != null ? $"{guest.LastName} {guest.FirstName}" : "Ismeretlen",
                        RoomId = booking.RoomId,
                        RoomNumber = room != null ? room.RoomNumber : "Ismeretlen",
                        CheckInDate = booking.CheckInDate,
                        CheckOutDate = booking.CheckOutDate,
                        //BookingDate = booking.BookingDate,
                        NumberOfGuests = booking.NumberOfGuests,
                        TotalPrice = booking.TotalPrice,
                        PaymentStatus = booking.PaymentStatus,
                        Status = booking.Status,
                        HasFeedback = hasFeedback
                    });
                }

                bookingsListView.ItemsSource = bookingsWithDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task<Guest> GetGuestById(int guestId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var guests = await _httpClient.GetAsync("Guests/GetAllGuestever");
                if (!guests.IsSuccessStatusCode)
                    return null;

                var guestsString = await guests.Content.ReadAsStringAsync();
                var allGuests = JsonSerializer.Deserialize<List<Guest>>(guestsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return allGuests?.FirstOrDefault(g => g.GuestId == guestId);
            }
            catch
            {
                return null;
            }
        }

        private async Task<Room> GetRoomById(int roomId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var rooms = await _httpClient.GetAsync("Rooms/GetRoomWith");
                if (!rooms.IsSuccessStatusCode)
                    return null;

                var roomsString = await rooms.Content.ReadAsStringAsync();
                var allRooms = JsonSerializer.Deserialize<List<Room>>(roomsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return allRooms?.FirstOrDefault(r => r.RoomId == roomId);
            }
            catch
            {
                return null;
            }
        }

        private async Task LoadRoomsComboBox()
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

                var availableRooms = rooms.Where(r => r.Status == "Szabad").ToList();
                RoomComboBox.ItemsSource = availableRooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadGuestsComboBox()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Guests/GetAllGuestever");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a vendégek lekérdezésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var guests = JsonSerializer.Deserialize<List<Guest>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (guests == null || guests.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető vendégek az adatbázisban.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var guestViewModels = guests.Select(g => new
                {
                    g.GuestId,
                    g.FirstName,
                    g.LastName,
                    FullName = $"{g.LastName} {g.FirstName}"
                }).ToList();

                GuestComboBox.ItemsSource = guestViewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooking = bookingsListView.SelectedItem as BookingViewModel;
            if (selectedBooking == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy foglalást a szerkesztéshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isEditBooking = true;
            currentEditBookingId = selectedBooking.BookingId;

            LoadRoomForEditAsync(selectedBooking.RoomId.Value);

            foreach (var item in GuestComboBox.Items)
            {
                var prop = item.GetType().GetProperty("GuestId");
                if (prop != null && (int)prop.GetValue(item) == selectedBooking.GuestId)
                {
                    GuestComboBox.SelectedItem = item;
                    break;
                }
            }

            CheckInDatePicker.SelectedDate = selectedBooking.CheckInDate;
            CheckOutDatePicker.SelectedDate = selectedBooking.CheckOutDate;
            NumberOfGuestsTextBox.Text = selectedBooking.NumberOfGuests?.ToString();
            TotalPriceTextBox.Text = selectedBooking.TotalPrice?.ToString("N0") + " Ft";

            
            if (selectedBooking.PaymentStatus != null)
            {
                foreach (ComboBoxItem item in PaymentMethodComboBox.Items)
                {
                    if (item.Content.ToString() == selectedBooking.PaymentStatus)
                    {
                        PaymentMethodComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (selectedBooking.PaymentMethod != null)
            {
                foreach (ComboBoxItem item in PaymentMethodComboBox.Items)
                {
                    if (item.Content.ToString() == selectedBooking.PaymentMethod)
                    {
                        PaymentMethodComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            bookingEditPanel.Visibility = Visibility.Visible;
        }

        private async void LoadRoomForEditAsync(int roomId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                    return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Rooms/GetRoomWith");
                if (!response.IsSuccessStatusCode)
                    return;

                var responseString = await response.Content.ReadAsStringAsync();
                var allRooms = JsonSerializer.Deserialize<List<Room>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var room = allRooms?.FirstOrDefault(r => r.RoomId == roomId);
                if (room != null)
                {

                    var availableRooms = allRooms.Where(r => r.Status == "Szabad" || r.RoomId == roomId).ToList();
                    RoomComboBox.ItemsSource = availableRooms;


                    foreach (var item in RoomComboBox.Items)
                    {
                        if ((item as Room)?.RoomId == roomId)
                        {
                            RoomComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch
            {
                // Hiba esetén nem teszünk semmit
            }
        }

        private async void DeleteBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooking = bookingsListView.SelectedItem as BookingViewModel;
            if (selectedBooking == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy foglalást a törléshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedBooking.BookingId}. számú foglalást?\nVendég: {selectedBooking.GuestName}\nSzoba: {selectedBooking.RoomNumber}",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteBooking(selectedBooking.BookingId);
            }
        }

        private async Task DeleteBooking(int bookingId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.DeleteAsync($"Bookings/DeleteBooking/{bookingId}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a foglalás törlésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A foglalás sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadBookingsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBookingEditButton_Click(object sender, RoutedEventArgs e)
        {
            bookingEditPanel.Visibility = Visibility.Collapsed;
            isEditBooking = false;
            currentEditBookingId = 0;
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void RoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void NumberOfGuestsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            if (RoomComboBox.SelectedItem != null &&
                CheckInDatePicker.SelectedDate.HasValue &&
                CheckOutDatePicker.SelectedDate.HasValue &&
                !string.IsNullOrWhiteSpace(NumberOfGuestsTextBox.Text))
            {
                var room = RoomComboBox.SelectedItem as Room;
                var checkInDate = CheckInDatePicker.SelectedDate.Value;
                var checkOutDate = CheckOutDatePicker.SelectedDate.Value;

                if (int.TryParse(NumberOfGuestsTextBox.Text, out int numberOfGuests))
                {

                    var days = (checkOutDate - checkInDate).Days;


                    if (days > 0 && room?.PricePerNight.HasValue == true)
                    {
                        var totalPrice = days * room.PricePerNight.Value * numberOfGuests;
                        TotalPriceTextBox.Text = totalPrice.ToString("N0") + " Ft";
                        return;
                    }
                }
            }

            TotalPriceTextBox.Text = string.Empty;
        }

        private async void SaveBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RoomComboBox.SelectedItem == null ||
                    GuestComboBox.SelectedItem == null ||
                    CheckInDatePicker.SelectedDate == null ||
                    CheckOutDatePicker.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(NumberOfGuestsTextBox.Text) ||
                    PaymentMethodComboBox.SelectedItem == null
                    )

                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!", "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CheckInDatePicker.SelectedDate >= CheckOutDatePicker.SelectedDate)
                {
                    MessageBox.Show("A kijelentkezés dátuma későbbi kell legyen, mint a bejelentkezés dátuma!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(NumberOfGuestsTextBox.Text, out int numberOfGuests) || numberOfGuests <= 0)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes számú vendéget!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedRoom = RoomComboBox.SelectedItem as Room;
                if (selectedRoom?.Capacity < numberOfGuests)
                {
                    MessageBox.Show($"A választott szoba kapacitása ({selectedRoom.Capacity} fő) kisebb, mint a megadott vendégek száma ({numberOfGuests} fő).", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedPaymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (isEditBooking)
                {
                    var updateDto = new UpdateBooking
                    {
                        CheckInDate = CheckInDatePicker.SelectedDate,
                        CheckOutDate = CheckOutDatePicker.SelectedDate,
                        NumberOfGuests = numberOfGuests
                    };

                    await UpdateBooking(currentEditBookingId, updateDto);
                }
                else { }

                bookingEditPanel.Visibility = Visibility.Collapsed;
                isEditBooking = false;
                currentEditBookingId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task UpdateBooking(int bookingId, UpdateBooking updateBooking)
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
                    JsonSerializer.Serialize(updateBooking, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Bookings/UpdateBooking/{bookingId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a foglalás frissítésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A foglalás sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadBookingsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearBookingFormFields()
        {
            RoomComboBox.SelectedIndex = -1;
            GuestComboBox.SelectedIndex = -1;
            CheckInDatePicker.SelectedDate = null;
            CheckOutDatePicker.SelectedDate = null;
            NumberOfGuestsTextBox.Text = string.Empty;
            PaymentMethodComboBox.SelectedIndex = -1;
            TotalPriceTextBox.Text = string.Empty;
        }


        private async void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int bookingId = (int)button.Tag;
                var booking = (bookingsListView.Items.Cast<BookingViewModel>()
                    .FirstOrDefault(b => b.BookingId == bookingId));

                if (booking == null) return;

                switch (booking.Status)
                {
                    case "Függőben":
                    case "Jóváhagyva":
                        await UpdateBookingStatus(bookingId, "Checked In");
                        break;

                    case "Checked In":
                        bool isEverythingOk = await ShowCheckoutConfirmation();
                        if (isEverythingOk)
                        {
                            await UpdateBookingStatus(bookingId, "Finished");
                        }
                        else
                        {
                            var complaintResult = await ShowComplaintDialog(bookingId, booking.GuestId ?? 0);
                            if (complaintResult != null)
                            {
                                await UpdateBookingStatus(bookingId, "Finished");
                            }
                        }
                        break;
                }
            }
        }

        private async Task UpdateBookingStatus(int bookingId, string newStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var updateDto = new UpdateBookingStatus
                {
                    Status = newStatus
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(updateDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Bookings/UpdateBookingStatus/{bookingId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var booking = (bookingsListView.Items.Cast<BookingViewModel>()
                        .FirstOrDefault(b => b.BookingId == bookingId));

                    if (booking != null)
                    {
                        booking.Status = newStatus;
                        bookingsListView.Items.Refresh();
                    }

                    MessageBox.Show($"A foglalás állapota sikeresen módosítva: {newStatus}",
                                  "Sikeres művelet", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("A foglalás nem zárható le, mert még nincs kifizetve.",
                                      "Fizetés szükséges", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Hiba a foglalás státuszának frissítésekor: {response.StatusCode}\n{errorResponse}",
                                      "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<bool> ShowCheckoutConfirmation()
        {
            var result = MessageBox.Show(
                "Minden rendben volt a vendéggel kapcsolatban?",
                "Kijelentkezés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        private class ComplaintResult
        {
            public string Text { get; set; }
            public int Rating { get; set; }
        }

        private async Task<ComplaintResult> ShowComplaintDialog(int bookingId, int guestId)
        {
            var complaintWindow = new Window
            {
                Title = "Panasz és értékelés",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            var headerText = new TextBlock
            {
                Text = "Panasz és értékelés",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10)
            };
            Grid.SetRow(headerText, 0);
            grid.Children.Add(headerText);

            var ratingPanel = new StackPanel
            {
                Margin = new Thickness(10, 0, 10, 10)
            };
            Grid.SetRow(ratingPanel, 1);

            ratingPanel.Children.Add(new TextBlock
            {
                Text = "Értékelés (kattintson a csillagokra):",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            int ratingValue = 1;

            var starsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var ratingValueText = new TextBlock
            {
                Text = $"Kiválasztott értékelés: {ratingValue}/10",
                Margin = new Thickness(0, 10, 0, 0)
            };

            for (int i = 1; i <= 10; i++)
            {
                var star = new TextBlock
                {
                    Text = "★",
                    FontSize = 32,
                    Foreground = (i <= ratingValue) ? Brushes.Gold : Brushes.LightGray,
                    Margin = new Thickness(2, 0, 2, 0),
                    Cursor = Cursors.Hand,
                    Tag = i
                };

                star.MouseEnter += (s, e) =>
                {
                    if (s is TextBlock tb && tb.Tag != null)
                    {
                        int hoverRating = (int)tb.Tag;

                        int index = 0;
                        foreach (var starElement in starsPanel.Children.OfType<TextBlock>())
                        {
                            index++;
                            starElement.Foreground = (index <= hoverRating) ? Brushes.Gold : Brushes.LightGray;
                        }
                    }
                };

                star.MouseLeave += (s, e) =>
                {
                    int index = 0;
                    foreach (var starElement in starsPanel.Children.OfType<TextBlock>())
                    {
                        index++;
                        starElement.Foreground = (index <= ratingValue) ? Brushes.Gold : Brushes.LightGray;
                    }
                };
                star.MouseLeftButtonDown += (s, e) =>
                {
                    if (s is TextBlock tb && tb.Tag != null)
                    {
                        ratingValue = (int)tb.Tag;

                        int index = 0;
                        foreach (var starElement in starsPanel.Children.OfType<TextBlock>())
                        {
                            index++;
                            starElement.Foreground = (index <= ratingValue) ? Brushes.Gold : Brushes.LightGray;
                        }

                        ratingValueText.Text = $"Kiválasztott értékelés: {ratingValue}/10";
                    }
                };

                starsPanel.Children.Add(star);
            }

            ratingPanel.Children.Add(starsPanel);
            ratingPanel.Children.Add(ratingValueText);

            grid.Children.Add(ratingPanel);


            var commentPanel = new StackPanel
            {
                Margin = new Thickness(10, 0, 10, 10)
            };
            Grid.SetRow(commentPanel, 2);

            commentPanel.Children.Add(new TextBlock
            {
                Text = "Kérjük, írja le a panaszt részletesen:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var complaintTextBox = new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 150
            };

            commentPanel.Children.Add(complaintTextBox);
            grid.Children.Add(commentPanel);

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 10)
            };
            Grid.SetRow(buttonsPanel, 3);

            var cancelButton = new Button
            {
                Content = "Mégse",
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Foreground = Brushes.White
            };

            cancelButton.Click += (s, e) => complaintWindow.DialogResult = false;

            var saveButton = new Button
            {
                Content = "Mentés",
                Padding = new Thickness(15, 8, 15, 8),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A")),
                Foreground = Brushes.White,
                IsDefault = true
            };

            saveButton.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(complaintTextBox.Text))
                {
                    MessageBox.Show("Kérjük, írja le a panasz részleteit!", "Hiányzó információ",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    saveButton.IsEnabled = false;

                    var feedback = new CreateFeedback
                    {
                        Category = "Panasz",
                        Rating = ratingValue,
                        Status = "Új",
                        Response = complaintTextBox.Text,
                        GuestId = guestId
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(feedback, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                        System.Text.Encoding.UTF8,
                        "application/json");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);
                    var response = await _httpClient.PostAsync("Feedback/UploadFeedback", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("A panasz sikeresen rögzítve!", "Sikeres művelet",
                                       MessageBoxButton.OK, MessageBoxImage.Information);

                        var booking = bookingsListView.Items.Cast<BookingViewModel>()
                            .FirstOrDefault(b => b.BookingId == bookingId);

                        if (booking != null)
                        {
                            booking.HasFeedback = true;
                            bookingsListView.Items.Refresh();
                        }

                        complaintWindow.DialogResult = true;
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Hiba a visszajelzés mentésekor: {errorResponse}",
                                       "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveButton.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    saveButton.IsEnabled = true;
                }
            };

            buttonsPanel.Children.Add(cancelButton);
            buttonsPanel.Children.Add(saveButton);
            grid.Children.Add(buttonsPanel);

            complaintWindow.Content = grid;

            if (complaintWindow.ShowDialog() == true)
            {
                return new ComplaintResult
                {
                    Text = complaintTextBox.Text,
                    Rating = ratingValue
                };
            }

            return null;
        }

        private async Task SaveComplaint(int bookingId, string complaintText)
        {
            try
            {
                var booking = (bookingsListView.Items.Cast<BookingViewModel>()
                    .FirstOrDefault(b => b.BookingId == bookingId));

                if (booking == null || booking.GuestId == null)
                {
                    MessageBox.Show("Nem található vendég a foglaláshoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var feedback = new CreateFeedback
                {
                    Category = "Panasz",
                    Rating = 1,
                    Status = "Új",
                    Response = complaintText,
                    GuestId = booking.GuestId.Value
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(feedback, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);
                var response = await _httpClient.PostAsync("Feedback/UploadFeedback", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("A panasz sikeresen rögzítve!", "Sikeres művelet",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    booking.HasFeedback = true;
                    bookingsListView.Items.Refresh();
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a panasz mentésekor: {errorResponse}", "Hiba",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewFeedback_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int bookingId = (int)button.Tag;
                var booking = bookingsListView.Items.Cast<BookingViewModel>()
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null || !booking.GuestId.HasValue) return;

                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                    var response = await _httpClient.GetAsync("Feedback/GetallFeedback");
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Nem sikerült lekérni a visszajelzéseket.", "Hiba",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    var allFeedbacks = JsonSerializer.Deserialize<List<Feedback>>(responseString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var guestFeedbacks = allFeedbacks
                        .Where(f => f.GuestId == booking.GuestId)
                        .OrderByDescending(f => f.FeedbackDate)
                        .ToList();

                    if (guestFeedbacks.Count == 0)
                    {
                        MessageBox.Show("Nincsenek visszajelzések ehhez a vendéghez.", "Információ",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    ShowFeedbacksListDialog(guestFeedbacks, booking);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowFeedbacksListDialog(List<Feedback> feedbacks, BookingViewModel booking)
        {
            var feedbackWindow = new Window
            {
                Title = $"Visszajelzések - {booking.GuestName}",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            var headerText = new TextBlock
            {
                Text = $"Visszajelzések a vendégtől: {booking.GuestName}",
                Margin = new Thickness(10),
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };
            Grid.SetRow(headerText, 0);
            mainGrid.Children.Add(headerText);

            var listView = new ListView
            {
                Margin = new Thickness(10),
                SelectionMode = SelectionMode.Single
            };
            Grid.SetRow(listView, 1);

            var gridView = new GridView();
            listView.View = gridView;

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Dátum",
                DisplayMemberBinding = new Binding("FeedbackDate")
                {
                    StringFormat = "{0:yyyy.MM.dd}"
                },
                Width = 80
            });

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Kategória",
                DisplayMemberBinding = new Binding("Category"),
                Width = 80
            });

            var ratingColumn = new GridViewColumn
            {
                Header = "Értékelés",
                Width = 180
            };

            ratingColumn.CellTemplate = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(StackPanel));
            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            for (int i = 1; i <= 10; i++)
            {
                var starFactory = new FrameworkElementFactory(typeof(TextBlock));
                starFactory.SetValue(TextBlock.TextProperty, "★");
                starFactory.SetValue(TextBlock.FontSizeProperty, 16.0);
                starFactory.SetValue(TextBlock.MarginProperty, new Thickness(1));

                Binding colorBinding = new Binding("Rating");
                colorBinding.Converter = new StarColorConverter();
                colorBinding.ConverterParameter = i;
                starFactory.SetBinding(TextBlock.ForegroundProperty, colorBinding);

                factory.AppendChild(starFactory);
            }

            ratingColumn.CellTemplate.VisualTree = factory;
            gridView.Columns.Add(ratingColumn);

            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Állapot",
                DisplayMemberBinding = new Binding("Status"),
                Width = 80
            });

            var commentColumn = new GridViewColumn
            {
                Header = "Megjegyzés",
                Width = 150
            };

            commentColumn.CellTemplate = new DataTemplate();
            var commentFactory = new FrameworkElementFactory(typeof(TextBlock));
            commentFactory.SetBinding(TextBlock.TextProperty, new Binding("Comments"));
            commentFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            commentFactory.SetValue(TextBlock.MaxWidthProperty, 140.0);
            commentFactory.SetValue(TextBlock.MarginProperty, new Thickness(5));
            commentColumn.CellTemplate.VisualTree = commentFactory;

            gridView.Columns.Add(commentColumn);

            var responseColumn = new GridViewColumn
            {
                Header = "Panasz",
                Width = 200

            };

            responseColumn.CellTemplate = new DataTemplate();
            var responseFactory = new FrameworkElementFactory(typeof(TextBlock));
            responseFactory.SetBinding(TextBlock.TextProperty, new Binding("Response"));
            responseFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            responseFactory.SetValue(TextBlock.MaxWidthProperty, 190.0);
            responseFactory.SetValue(TextBlock.MarginProperty, new Thickness(5));
            responseColumn.CellTemplate.VisualTree = responseFactory;

            gridView.Columns.Add(responseColumn);

            listView.ItemsSource = feedbacks;

            listView.ItemContainerStyle = new Style(typeof(ListViewItem))
            {
                Setters =
        {
            new Setter(ListViewItem.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch),
            new Setter(ListViewItem.HeightProperty, double.NaN),
            new Setter(ListViewItem.MinHeightProperty, 50.0)
        }
            };

            mainGrid.Children.Add(listView);

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 10, 10)
            };
            Grid.SetRow(buttonsPanel, 2);

            var editButton = new Button
            {
                Content = "Szerkesztés",
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A")),
                Foreground = Brushes.White
            };

            editButton.Click += async (s, e) =>
            {
                var selectedFeedback = listView.SelectedItem as Feedback;
                if (selectedFeedback == null)
                {
                    MessageBox.Show("Kérjük, válasszon ki egy visszajelzést a szerkesztéshez.",
                                  "Nincs kiválasztva", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                await ShowFeedbackEditDialog(selectedFeedback, feedbackWindow);
                listView.Items.Refresh();
            };

            var closeButton = new Button
            {
                Content = "Bezárás",
                Padding = new Thickness(15, 8, 15, 8),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Foreground = Brushes.White
            };

            closeButton.Click += (s, e) => feedbackWindow.Close();

            buttonsPanel.Children.Add(editButton);
            buttonsPanel.Children.Add(closeButton);
            mainGrid.Children.Add(buttonsPanel);

            feedbackWindow.Content = mainGrid;
            feedbackWindow.ShowDialog();
        }

        private async Task ShowFeedbackEditDialog(Feedback feedback, Window owner)
        {
            var editWindow = new Window
            {
                Title = "Visszajelzés szerkesztése",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            var headerText = new TextBlock
            {
                Text = "Visszajelzés szerkesztése",
                Margin = new Thickness(10),
                FontWeight = FontWeights.Bold,
                FontSize = 16
            };
            Grid.SetRow(headerText, 0);
            grid.Children.Add(headerText);

            var infoPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10, 5, 10, 10)
            };
            Grid.SetRow(infoPanel, 1);

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Dátum: {feedback.FeedbackDate?.ToString("yyyy.MM.dd")}",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 20, 0)
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Kategória: {feedback.Category}",
                FontWeight = FontWeights.SemiBold
            });

            grid.Children.Add(infoPanel);

            var ratingPanel = new StackPanel
            {
                Margin = new Thickness(10, 5, 10, 10)
            };
            Grid.SetRow(ratingPanel, 2);

            ratingPanel.Children.Add(new TextBlock
            {
                Text = "Értékelés (kattintson a csillagokra):",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            int ratingValue = feedback.Rating ?? 1;

            var starsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var ratingValueText = new TextBlock
            {
                Text = $"Kiválasztott értékelés: {ratingValue}/10",
                Margin = new Thickness(0, 10, 0, 0)
            };

            for (int i = 1; i <= 10; i++)
            {
                var star = new TextBlock
                {
                    Text = "★",
                    FontSize = 32,
                    Foreground = (i <= ratingValue) ? Brushes.Gold : Brushes.LightGray,
                    Margin = new Thickness(2, 0, 2, 0),
                    Cursor = Cursors.Hand,
                    Tag = i
                };

                star.MouseEnter += (s, e) =>
                {
                    if (s is TextBlock tb && tb.Tag != null)
                    {
                        int hoverRating = (int)tb.Tag;

                        int index = 0;
                        foreach (var starElement in starsPanel.Children.OfType<TextBlock>())
                        {
                            index++;
                            starElement.Foreground = (index <= hoverRating) ? Brushes.Gold : Brushes.LightGray;
                        }
                    }
                };

                star.MouseLeave += (s, e) =>
                {
                    int index = 0;
                    foreach (var starElement in starsPanel.Children.OfType<TextBlock>())
                    {
                        index++;
                        starElement.Foreground = (index <= ratingValue) ? Brushes.Gold : Brushes.LightGray;
                    }
                };

                star.MouseLeftButtonDown += (s, e) =>
                {
                    if (s is TextBlock tb && tb.Tag != null)
                    {
                        ratingValue = (int)tb.Tag;

                        int index = 0;
                        foreach (var starElement in starsPanel.Children.OfType<TextBlock>())
                        {
                            index++;
                            starElement.Foreground = (index <= ratingValue) ? Brushes.Gold : Brushes.LightGray;
                        }

                        ratingValueText.Text = $"Kiválasztott értékelés: {ratingValue}/10";
                    }
                };

                starsPanel.Children.Add(star);
            }

            ratingPanel.Children.Add(starsPanel);
            ratingPanel.Children.Add(ratingValueText);
            grid.Children.Add(ratingPanel);

            var statusPanel = new StackPanel
            {
                Margin = new Thickness(10, 5, 10, 10)
            };
            Grid.SetRow(statusPanel, 3);

            statusPanel.Children.Add(new TextBlock
            {
                Text = "Állapot:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var statusCombo = new ComboBox
            {
                Width = 150,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            statusCombo.Items.Add("Új");
            statusCombo.Items.Add("Feldolgozva");
            statusCombo.Items.Add("Megoldva");
            statusCombo.Items.Add("Elutasítva");
            statusCombo.SelectedItem = feedback.Status;

            statusPanel.Children.Add(statusCombo);
            grid.Children.Add(statusPanel);

            var commentPanel = new StackPanel
            {
                Margin = new Thickness(10, 5, 10, 10)
            };
            Grid.SetRow(commentPanel, 4);

            commentPanel.Children.Add(new TextBlock
            {
                Text = "Megjegyzés:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var commentTextBox = new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 100,
                Text = feedback.Comments ?? ""
            };
            commentPanel.Children.Add(commentTextBox);
            grid.Children.Add(commentPanel);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 10, 10)
            };
            Grid.SetRow(buttonPanel, 5);

            var cancelButton = new Button
            {
                Content = "Mégse",
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Foreground = Brushes.White
            };

            cancelButton.Click += (s, e) => editWindow.Close();

            var saveButton = new Button
            {
                Content = "Mentés",
                Padding = new Thickness(15, 8, 15, 8),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A")),
                Foreground = Brushes.White
            };

            saveButton.Click += async (s, e) =>
            {
                try
                {
                    saveButton.IsEnabled = false;

                    var updateData = new UpdateFeedback
                    {
                        Comment = commentTextBox.Text,
                        Status = statusCombo.SelectedItem?.ToString(),
                        Rating = ratingValue
                    };

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                    var content = new StringContent(
                        JsonSerializer.Serialize(updateData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                        System.Text.Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PutAsync($"Feedback/UpdateForFeedback/{feedback.FeedbackId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        feedback.Comments = updateData.Comment;
                        feedback.Status = updateData.Status;
                        feedback.Rating = updateData.Rating;

                        MessageBox.Show("Visszajelzés sikeresen frissítve!", "Mentés sikeres",
                                      MessageBoxButton.OK, MessageBoxImage.Information);

                        editWindow.Close();
                    }
                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Hiba a visszajelzés frissítésekor: {responseContent}",
                                      "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        saveButton.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    saveButton.IsEnabled = true;
                }
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(saveButton);
            grid.Children.Add(buttonPanel);

            editWindow.Content = grid;
            editWindow.ShowDialog();
        }

        private int currentPaymentBookingId = 0;

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            currentPaymentBookingId = (int)button.Tag;

            bookingsContainer.Visibility = Visibility.Collapsed;
            paymentPanel.Visibility = Visibility.Visible;
        }
        private async Task UpdatePaymentStatus(int bookingId, string status, string paymentMethod)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var updateDto = new UpdatePaymentInfo
                {
                    Status = status,
                    PaymentMethod = paymentMethod
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
                    { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync(
                    $"Payments/UpdatePaymentStatusByBookingId/{bookingId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a fizetési státusz frissítésekor: {errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A fizetési státusz sikeresen frissítve!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadBookingsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PaymentBackButton_Click(object sender, RoutedEventArgs e)
        {
            paymentPanel.Visibility = Visibility.Collapsed;
            bookingsContainer.Visibility = Visibility.Visible;
            currentPaymentBookingId = 0;
        }

        private async void CashButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdatePaymentStatus(currentPaymentBookingId, "Fizetve", "Készpénz");

            paymentPanel.Visibility = Visibility.Collapsed;
            bookingsContainer.Visibility = Visibility.Visible;
            currentPaymentBookingId = 0;
        }
        private async void CardButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdatePaymentStatus(currentPaymentBookingId, "Fizetve", "Bankkártya");

            paymentPanel.Visibility = Visibility.Collapsed;
            bookingsContainer.Visibility = Visibility.Visible;
            currentPaymentBookingId = 0;
        }
        private async void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            string paymentMethod = "Készpénz";

            if (CashRadioButton.IsChecked == true)
                paymentMethod = "Készpénz";
            else if (CardRadioButton.IsChecked == true)
                paymentMethod = "Bankkártya";
            else if (TransferRadioButton.IsChecked == true)
                paymentMethod = "Átutalás";
            else if (SzepCardRadioButton.IsChecked == true)
                paymentMethod = "SZÉP kártya";

            await UpdatePaymentStatus(currentPaymentBookingId, "Fizetve", paymentMethod);

           
            paymentPanel.Visibility = Visibility.Collapsed;
            bookingsContainer.Visibility = Visibility.Visible;
            currentPaymentBookingId = 0;
        }
        private void CancelPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            paymentPanel.Visibility = Visibility.Collapsed;
            bookingsContainer.Visibility = Visibility.Visible;
            currentPaymentBookingId = 0;
        }
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
    public int? RoomId { get; set; }
}

// Guest osztályok
public class Guest
{
    public int GuestId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int? UserId { get; set; }
}


public class CreateGuest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int? UserId { get; set; }
}


public class UpdateGuest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
}

// Booking osztályok
public class Booking
{
    public int BookingId { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? GuestId { get; set; }
    public int? RoomId { get; set; }
    public decimal? TotalPrice { get; set; }
    //public DateTime? BookingDate { get; set; }
    public string PaymentStatus { get; set; }

    public string PaymentMethod { get; set; }

    public int? NumberOfGuests { get; set; }
    public string Status { get; set; }
    public List<Payment> Payments { get; set; }
}

public class Payment
{
    public int PaymentId { get; set; }
    public int? BookingId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal? Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public string Currency { get; set; }
    public string PaymentNotes { get; set; }
    public DateTime? DateAdded { get; set; }
}

public class UpdatePaymentInfo
{
    public string Status { get; set; }
    public string PaymentMethod { get; set; }
}




public class CreateBookingDto
{
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int GuestId { get; set; }
    public int NumberOfGuests { get; set; }
    public string PaymentMethod { get; set; }
}

public class UpdateBooking
{
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
}

public class UpdateBookingStatus
{
    public string Status { get; set; }
}

public class BookingViewModel
{
    public int BookingId { get; set; }
    public int? GuestId { get; set; }
    public string GuestName { get; set; }
    public int? RoomId { get; set; }
    public string RoomNumber { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    //public DateTime? BookingDate { get; set; }
    public int? NumberOfGuests { get; set; }
    public decimal? TotalPrice { get; set; }
    public string PaymentStatus { get; set; }

    public string PaymentMethod { get; set; }

    public string Status { get; set; }

    public bool CanChangeStatus
    {
        get
        {
            return Status != "Finished" && Status != "Lemondva";
        }
    }

    public bool HasFeedback { get; set; }
    public bool IsPaymentEnabled
    {
        get
        {
            return (Status == "Jóváhagyva" || Status == "Checked In") &&
                    PaymentStatus?.Trim().ToLower() != "fizetve";
        }
    }



}



//Feedback osztályok
public class CreateFeedback
{
    public string Category { get; set; }
    public int? Rating { get; set; }
    public string Status { get; set; }
    public string Response { get; set; }
    public int GuestId { get; set; }
}

public class Feedback
{
    public int FeedbackId { get; set; }
    public DateTime? FeedbackDate { get; set; }
    public string Comments { get; set; }
    public string Category { get; set; }
    public int? Rating { get; set; }
    public string Status { get; set; }
    public string Response { get; set; }
    public DateTime? ResponseDate { get; set; }
    public DateTime? DateAdded { get; set; }
    public int? GuestId { get; set; }
}
public class UpdateFeedback
{
    public string Comment { get; set; }
    public string Status { get; set; }
    public int? Rating { get; set; }
}


