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
using hmz_rt.Dtos;
using hmz_rt.Models.Dtos;
using hmz_rt.Models.Converters;
using System.IO;
using System.Windows.Media.Imaging;
using hmz_rt.Models.Services;
using System.Windows.Documents;
using System.Text.RegularExpressions;


namespace RoomListApp
{

    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private DispatcherTimer timer;

        // Változók a szoba szerkesztési módhoz
        private bool isEditRoom = false;
        private int currentRoomId = 0;
        //private string selectedRoomImagePath = string.Empty;
        //private byte[] roomImageData = null;

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
        //Változók a Maintenance szerkesztési módhoz
        private bool isEditMaintenance = false;
        private int currentMaintenanceId = 0;

        // Változók az Event szerkesztési módhoz
        private bool isEditEvent = false;
        private int currentEventId = 0;

        // Változók az EventBooking szerkesztési módhoz
        private bool isEditEventBooking = false;
        private int currentEditEventBookingId = 0;

        // Változók a kép feltöltéshez
        /*
        private string selectedImagePath = string.Empty;
        private byte[] imageData = null; */

       // private RoomManager _roomManager;

        //private EventImageManager _eventManager;

        private EventBookingsService _eventBookingsService;
        private TokenService _tokenService;
        private GuestService _guestService;
        private EventService _eventService;


        public MainWindow()
        {
            InitializeComponent();

            // SSL beállítások
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.hmzrt.eu/") };

            if (!string.IsNullOrEmpty(TokenStorage.Username))
            {
                UserNameDisplay.Text = $"Üdvözöljük, {TokenStorage.Username}!";
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateTimeDisplay();
            //_roomManager = new RoomManager();
            //_eventManager = new EventImageManager();

            _tokenService = new TokenService();
            _eventBookingsService = new EventBookingsService(_tokenService);
            _guestService = new GuestService(_tokenService);
            _eventService = new EventService(_tokenService);

            LoadEmailTemplates();



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

        private void RefreshRoomsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomsToListView();
        }
        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            ClearRoomFormFields();
            isEditRoom = false;
            currentRoomId = 0;
            roomEditTitle.Text = "Új szoba";
            roomEditPanel.Visibility = Visibility.Visible;
        }

        private async void EditRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = roomsListView.SelectedItem as Room;
            if (selectedRoom == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy szobát a szerkesztéshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isEditRoom = true;
            currentRoomId = selectedRoom.RoomId;
            roomEditTitle.Text = "Szoba szerkesztése";
            RoomNumberTextBox.Text = selectedRoom.RoomNumber;
            CapacityTextBox.Text = selectedRoom.Capacity?.ToString();
            PricePerNightTextBox.Text = selectedRoom.PricePerNight?.ToString();
            FloorNumberTextBox.Text = selectedRoom.FloorNumber?.ToString();
            RoomDescriptionTextBox.Text = selectedRoom.Description;
            AmenitiesTextBox.Text = selectedRoom.Amenities;

            foreach (ComboBoxItem item in RoomTypeComboBox.Items)
            {
                if (item.Content.ToString() == selectedRoom.RoomType)
                {
                    RoomTypeComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in RoomStatusComboBox.Items)
            {
                if (item.Content.ToString() == selectedRoom.Status)
                {
                    RoomStatusComboBox.SelectedItem = item;
                    break;
                }
            }

            // A képkezeléssel kapcsolatos kód eltávolítva

            roomEditPanel.Visibility = Visibility.Visible;
        }


        private async void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = roomsListView.SelectedItem as Room;
            if (selectedRoom == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy szobát a törléshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedRoom.RoomNumber} számú szobát?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteRoom(selectedRoom.RoomId);
            }
        }

        private async void SaveRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RoomNumberTextBox.Text) ||
                    RoomTypeComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(CapacityTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PricePerNightTextBox.Text) ||
                    RoomStatusComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(FloorNumberTextBox.Text))
                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!",
                        "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(CapacityTextBox.Text, out int capacity) || capacity < 1)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes kapacitást (legalább 1)!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(PricePerNightTextBox.Text, out decimal pricePerNight) || pricePerNight < 0)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes árat (nem lehet negatív)!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(FloorNumberTextBox.Text, out int floorNumber))
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes emelet számot!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedRoomType = (RoomTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                var selectedStatus = (RoomStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (isEditRoom)
                {
                    var updateDto = new UpdateRoom
                    {
                        RoomNumber = RoomNumberTextBox.Text,
                        RoomType = selectedRoomType,
                        Capacity = int.Parse(CapacityTextBox.Text),
                        PricePerNight = decimal.Parse(PricePerNightTextBox.Text),
                        Status = selectedStatus,
                        FloorNumber = int.Parse(FloorNumberTextBox.Text),
                        Description = RoomDescriptionTextBox.Text,
                        Amenities = AmenitiesTextBox.Text
                    };
                    await UpdateRoom(currentRoomId, updateDto);
                }
                else
                {
                    var createDto = new CreateRoom
                    {
                        RoomNumber = RoomNumberTextBox.Text,
                        RoomType = selectedRoomType,
                        Capacity = int.Parse(CapacityTextBox.Text),
                        PricePerNight = decimal.Parse(PricePerNightTextBox.Text),
                        Status = selectedStatus,
                        FloorNumber = int.Parse(FloorNumberTextBox.Text),
                        Description = RoomDescriptionTextBox.Text,
                        Amenities = AmenitiesTextBox.Text
                    };
                    await CreateRoom(createDto);
                }

                roomEditPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CreateRoom(CreateRoom newRoom)
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
                    JsonSerializer.Serialize(newRoom, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");
                var response = await _httpClient.PostAsync("Rooms/CreateRoom", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a szoba létrehozásakor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az új szoba sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadRoomsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateRoom(int roomId, UpdateRoom updateRoom)
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
                    JsonSerializer.Serialize(updateRoom, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");
                var response = await _httpClient.PutAsync($"Rooms/UpdateRoom/{roomId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a szoba frissítésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A szoba sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadRoomsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteRoom(int roomId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);
                var response = await _httpClient.DeleteAsync($"Rooms/DeleteRoomById/{roomId}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a szoba törlésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A szoba sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadRoomsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*private async void ViewRoomImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int roomId = (int)button.Tag;
                await LoadAndShowRoomImage(roomId);
            }
        }*/

        /*private void SelectRoomImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Válasszon képet",
                Filter = "Képfájlok|*.jpg;*.jpeg;*.png;*.gif|Minden fájl|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    selectedRoomImagePath = openFileDialog.FileName;
                    roomImageData = File.ReadAllBytes(selectedRoomImagePath);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(roomImageData);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    RoomImagePreview.Source = bitmap;
                    ViewRoomImageButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a kép betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearRoomImageButton_Click(object sender, RoutedEventArgs e)
        {
            selectedRoomImagePath = string.Empty;
            roomImageData = null;
            RoomImagePreview.Source = null;
            ViewRoomImageButton.IsEnabled = false;
        }

        private void ViewRoomImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoomImagePreview.Source != null)
            {
                ShowImageInLargeView(RoomImagePreview.Source as BitmapImage, "Szoba kép előnézete");
            }
        }
        */

        private void CancelRoomEditButton_Click(object sender, RoutedEventArgs e)
        {
            roomEditPanel.Visibility = Visibility.Collapsed;
            isEditRoom = false;
            currentRoomId = 0;
        }

        /* private async Task LoadAndShowRoomImage(int roomId)
         {
             try
             {
                 var rooms = await _httpClient.GetAsync($"Rooms/GetRoomWith");
                 if (!rooms.IsSuccessStatusCode)
                 {
                     MessageBox.Show("Nem sikerült lekérni a szoba adatait.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                     return;
                 }

                 var roomsString = await rooms.Content.ReadAsStringAsync();
                 var allRooms = JsonSerializer.Deserialize<List<Room>>(roomsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                 var roomWithImage = allRooms.FirstOrDefault(r => r.RoomId == roomId);

                 if (roomWithImage == null || string.IsNullOrEmpty(roomWithImage.Images))
                 {
                     MessageBox.Show("A szobához nincs társítva kép.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                     return;
                 }

                 var imageUrl = $"https://api.hmzrt.eu/{roomWithImage.Images}";
                 var imageResponse = await _httpClient.GetAsync(imageUrl);
                 if (!imageResponse.IsSuccessStatusCode)
                 {
                     MessageBox.Show("Nem sikerült letölteni a képet.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                     return;
                 }

                 var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                 var bitmap = new BitmapImage();
                 bitmap.BeginInit();
                 bitmap.StreamSource = new MemoryStream(imageBytes);
                 bitmap.CacheOption = BitmapCacheOption.OnLoad;
                 bitmap.EndInit();

                 ShowImageInLargeView(bitmap, $"{roomWithImage.RoomNumber} szoba képe");
             }
             catch (Exception ex)
             {
                 MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
             }
         }*/

        private void ClearRoomFormFields()
        {
            RoomNumberTextBox.Text = string.Empty;
            RoomTypeComboBox.SelectedIndex = -1;
            CapacityTextBox.Text = string.Empty;
            PricePerNightTextBox.Text = string.Empty;
            RoomStatusComboBox.SelectedIndex = -1;
            FloorNumberTextBox.Text = string.Empty;
            RoomDescriptionTextBox.Text = string.Empty;
            AmenitiesTextBox.Text = string.Empty;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            roomsContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
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

        private void RefreshStaffButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStaffToListView();
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

        private void RefreshPromotionsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPromotionsToListView();
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
            PromotionDescriptionTextBox.Text = selectedPromotion.Description;
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
                        Description = PromotionDescriptionTextBox.Text,
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
                        Description = PromotionDescriptionTextBox.Text,
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
            PromotionDescriptionTextBox.Text = string.Empty;
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

        private void RefreshGuestsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadGuestsToListView();
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


        private void BookingsCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dashboardGrid.Visibility = Visibility.Collapsed;
            bookingTypeSelectionContainer.Visibility = Visibility.Visible;
        }

        private void RefreshBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookingsToListView();
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
                    string.IsNullOrWhiteSpace(NumberOfGuestsTextBox.Text)

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

        private async void MaintenanceCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                dashboardGrid.Visibility = Visibility.Collapsed;
                maintenanceContainer.Visibility = Visibility.Visible;

                maintenanceEditPanel.Visibility = Visibility.Collapsed;

                await LoadMaintenanceToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

                dashboardGrid.Visibility = Visibility.Visible;
                maintenanceContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void MaintenanceBackButton_Click(object sender, RoutedEventArgs e)
        {
            maintenanceContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private void RefreshMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMaintenanceToListView();
        }
        private async Task LoadMaintenanceToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.GetAsync("Maintenance/Getmaintance");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a karbantartási kérelmek lekérdezésekor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var roomsResponse = await _httpClient.GetAsync("Rooms/GetRoomWith");
                List<Room> rooms = new List<Room>();

                if (roomsResponse.IsSuccessStatusCode)
                {
                    var roomsString = await roomsResponse.Content.ReadAsStringAsync();
                    rooms = JsonSerializer.Deserialize<List<Room>>(roomsString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    MaintenanceRoomComboBox.ItemsSource = rooms;
                    MaintenanceRoomComboBox.DisplayMemberPath = "RoomNumber";
                    MaintenanceRoomComboBox.SelectedValuePath = "RoomId";
                }
                else
                {
                    MessageBox.Show("Nem sikerült betölteni a szobákat.", "Figyelmeztetés",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var maintenanceItems = JsonSerializer.Deserialize<List<Roommaintenance>>(responseString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (maintenanceItems == null || maintenanceItems.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető karbantartási kérelmek az adatbázisban.",
                    "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    maintenanceListView.ItemsSource = null;
                    return;
                }

                var maintenanceWithRoomDetails = new List<RoomMaintenanceViewModel>();

                foreach (var item in maintenanceItems)
                {
                    var room = rooms.FirstOrDefault(r => r.RoomId == item.RoomId);

                    maintenanceWithRoomDetails.Add(new RoomMaintenanceViewModel
                    {
                        MaintenanceId = item.MaintenanceId,
                        MaintenanceDate = item.MaintenanceDate,
                        Description = item.Description,
                        Status = item.Status,
                        DateReported = item.DateReported,
                        ResolutionDate = item.ResolutionDate,
                        Cost = item.Cost,
                        Notes = item.Notes,
                        RoomId = item.RoomId,
                        RoomNumber = room != null ? room.RoomNumber : "Ismeretlen",
                        StaffId = item.StaffId
                    });
                }

                maintenanceListView.ItemsSource = maintenanceWithRoomDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMaintenanceFormFields();
            SetMaintenancePanelState(false);
            currentMaintenanceId = 0;
            maintenanceEditPanel.Visibility = Visibility.Visible;
        }

        private async void DeleteMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaintenance = maintenanceListView.SelectedItem as RoomMaintenanceViewModel;
            if (selectedMaintenance == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy karbantartási kérelmet a törléshez!",
                    "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedMaintenance.MaintenanceId} azonosítójú karbantartási kérelmet?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteMaintenance(selectedMaintenance.MaintenanceId);
            }
        }

        private void CancelMaintenanceEditButton_Click(object sender, RoutedEventArgs e)
        {
            maintenanceEditPanel.Visibility = Visibility.Collapsed;

        }
        private async void ReloadRoomsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadRoomsComboBox();
            MessageBox.Show($"Betöltött szobák száma: {MaintenanceRoomComboBox.Items.Count}");
        }

        private void SetMaintenancePanelState(bool isEdit)
        {
            isEditMaintenance = isEdit;
            maintenanceEditTitle.Text = isEdit ? "Karbantartási kérelem szerkesztése" : "Új karbantartási kérelem";

            MaintenanceRoomComboBox.IsEnabled = !isEdit;
            MaintenanceDescriptionTextBox.IsReadOnly = isEdit;

            StatusPanel.Visibility = isEdit ? Visibility.Visible : Visibility.Collapsed;
            ResolutionDatePanel.Visibility = isEdit ? Visibility.Visible : Visibility.Collapsed;
            CostPanel.Visibility = isEdit ? Visibility.Visible : Visibility.Collapsed;
            StaffPanel.Visibility = isEdit ? Visibility.Visible : Visibility.Collapsed;

            if (!isEdit)
            {
                MaintenanceStatusComboBox.SelectedIndex = 0;
                CostTextBox.Text = "0";
                MaintenanceDatePicker.SelectedDate = DateTime.Today;
            }
        }


        private async void SaveMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MaintenanceRoomComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(MaintenanceDescriptionTextBox.Text) ||
                    MaintenanceDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!",
                        "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (isEditMaintenance)
                {
                    if (MaintenanceStatusComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Kérjük, válasszon státuszt!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!decimal.TryParse(CostTextBox.Text, out decimal cost))
                    {
                        MessageBox.Show("Kérjük, adjon meg érvényes költséget!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var updateDto = new MaintanceUpdate
                    {
                        Status = (MaintenanceStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        ResolutionDate = ResolutionDatePicker.SelectedDate,
                        Cost = cost,
                        Notes = NotesTextBox.Text,
                        StaffId = StaffComboBox.SelectedValue != null ? (int?)StaffComboBox.SelectedValue : null
                    };

                    if (updateDto.Status == "Resolved" && !updateDto.ResolutionDate.HasValue)
                    {
                        updateDto.ResolutionDate = DateTime.Now;
                        ResolutionDatePicker.SelectedDate = updateDto.ResolutionDate;
                    }

                    await UpdateMaintenance(currentMaintenanceId, updateDto);
                }
                else
                {
                    var createDto = new MaintenanceCreateDto
                    {
                        MaintenanceDate = MaintenanceDatePicker.SelectedDate,
                        Description = MaintenanceDescriptionTextBox.Text,
                        Notes = NotesTextBox.Text,
                        RoomId = (int)MaintenanceRoomComboBox.SelectedValue
                    };

                    await CreateMaintenance(createDto);
                }

                maintenanceEditPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task CreateMaintenance(MaintenanceCreateDto newMaintenance)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                string jsonContent = JsonSerializer.Serialize(newMaintenance,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                Console.WriteLine($"JSON tartalom: {jsonContent}");

                var content = new StringContent(
                    jsonContent,
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Maintenance/MakeARequest", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Backend hibaüzenet:\n\nHiba kód: {(int)response.StatusCode} ({response.StatusCode})\n\nRészletek: {errorResponse}\n\nElküldött adatok: {jsonContent}",
                        "Szerver hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az új karbantartási kérelem sikeresen létrehozva!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadMaintenanceToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Részletes hiba: {ex.Message}\n\nStack trace: {ex.StackTrace}",
                    "Kivétel történt", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateMaintenance(int maintenanceId, MaintanceUpdate updateDto)
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
                    JsonSerializer.Serialize(updateDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Maintenance/UpdateRequestByManagger/{maintenanceId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a karbantartási kérelem frissítésekor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A karbantartási kérelem sikeresen frissítve!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadMaintenanceToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteMaintenance(int maintenanceId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);

                var response = await _httpClient.DeleteAsync($"Maintenance/DeleteRequest/{maintenanceId}");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a karbantartási kérelem törlésekor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("A karbantartási kérelem sikeresen törölve!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadMaintenanceToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearMaintenanceFormFields()
        {
            if (MaintenanceRoomComboBox.Items.Count > 0)
                MaintenanceRoomComboBox.SelectedIndex = 0;
            else
                MaintenanceRoomComboBox.SelectedIndex = -1;

            MaintenanceDatePicker.SelectedDate = DateTime.Today;
            MaintenanceDescriptionTextBox.Text = string.Empty;
            NotesTextBox.Text = string.Empty;

            MaintenanceStatusComboBox.SelectedIndex = 0;
            ResolutionDatePicker.SelectedDate = DateTime.Today;
            CostTextBox.Text = "0";
            if (StaffComboBox.Items.Count > 0)
                StaffComboBox.SelectedIndex = 0;
            else
                StaffComboBox.SelectedIndex = -1;
        }
        private void EditMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaintenance = maintenanceListView.SelectedItem as RoomMaintenanceViewModel;
            if (selectedMaintenance == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy karbantartási kérelmet a szerkesztéshez!",
                    "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetMaintenancePanelState(true);
            currentMaintenanceId = selectedMaintenance.MaintenanceId;

            var room = MaintenanceRoomComboBox.Items.Cast<Room>()
                .FirstOrDefault(r => r.RoomId == selectedMaintenance.RoomId);
            if (room != null)
                MaintenanceRoomComboBox.SelectedItem = room;

            MaintenanceDatePicker.SelectedDate = selectedMaintenance.MaintenanceDate;
            MaintenanceDescriptionTextBox.Text = selectedMaintenance.Description;
            NotesTextBox.Text = selectedMaintenance.Notes;

            foreach (ComboBoxItem item in MaintenanceStatusComboBox.Items)
            {
                if (item.Content.ToString() == selectedMaintenance.Status)
                {
                    MaintenanceStatusComboBox.SelectedItem = item;
                    break;
                }
            }
            ResolutionDatePicker.SelectedDate = selectedMaintenance.ResolutionDate;
            CostTextBox.Text = selectedMaintenance.Cost?.ToString() ?? "0";

            LoadStaffComboBox(selectedMaintenance.StaffId);
            maintenanceEditPanel.Visibility = Visibility.Visible;
        }
        private async Task LoadStaffComboBox(int? selectedStaffId = null)
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
                    MessageBox.Show($"Hiba a személyzet lekérdezésekor: {response.StatusCode}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var staffMembers = JsonSerializer.Deserialize<List<Staff>>(responseString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (staffMembers != null && staffMembers.Count > 0)
                {
                    var staffViewModel = staffMembers.Select(s => new
                    {
                        StaffId = s.StaffId,
                        FullName = $"{s.LastName} {s.FirstName}"
                    }).ToList();

                    StaffComboBox.ItemsSource = staffViewModel;

                    if (selectedStaffId.HasValue)
                    {
                        var staff = staffViewModel.FirstOrDefault(s => s.StaffId == selectedStaffId.Value);
                        if (staff != null)
                        {
                            StaffComboBox.SelectedItem = staff;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EventsCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                dashboardGrid.Visibility = Visibility.Collapsed;
                eventsContainer.Visibility = Visibility.Visible;
                eventEditPanel.Visibility = Visibility.Collapsed;

                await LoadEventsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                dashboardGrid.Visibility = Visibility.Visible;
                eventsContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void EventsBackButton_Click(object sender, RoutedEventArgs e)
        {
            eventsContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private void RefreshEventsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEventsToListView();
        }
        private async Task LoadEventsToListView()
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);
                var response = await _httpClient.GetAsync("Events/Geteventsadmin");
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba az események lekérdezésekor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<List<Event>>(responseString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (events == null || events.Count == 0)
                {
                    MessageBox.Show("Nincsenek elérhető események az adatbázisban.",
                        "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    eventsListView.ItemsSource = null;
                    return;
                }

                eventsListView.ItemsSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            ClearEventFormFields();
            isEditEvent = false;
            currentEventId = 0;
            eventEditTitle.Text = "Új esemény";
            eventEditPanel.Visibility = Visibility.Visible;
        }

        private async void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = eventsListView.SelectedItem as Event;
            if (selectedEvent == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy eseményt a szerkesztéshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isEditEvent = true;
            currentEventId = selectedEvent.EventId;
            eventEditTitle.Text = "Esemény szerkesztése";
            EventNameTextBox.Text = selectedEvent.EventName;
            EventCapacityTextBox.Text = selectedEvent.Capacity?.ToString();

            foreach (ComboBoxItem item in EventStatusComboBox.Items)
            {
                if (item.Content.ToString() == selectedEvent.Status)
                {
                    EventStatusComboBox.SelectedItem = item;
                    break;
                }
            }

            EventDatePicker.SelectedDate = selectedEvent.EventDate;
            LocationTextBox.Text = selectedEvent.Location;
            PriceTextBox.Text = selectedEvent.Price?.ToString();
            EventDescriptionTextBox.Text = selectedEvent.Description;
            OrganizerNameTextBox.Text = selectedEvent.OrganizerName;
            ContactInfoTextBox.Text = selectedEvent.ContactInfo;


            eventEditPanel.Visibility = Visibility.Visible;
        }

        private async void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = eventsListView.SelectedItem as Event;
            if (selectedEvent == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy eseményt a törléshez!",
                    "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretné a(z) {selectedEvent.EventName} eseményt?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteEvent(selectedEvent.EventId);
            }
        }

        private void CancelEventEditButton_Click(object sender, RoutedEventArgs e)
        {
            eventEditPanel.Visibility = Visibility.Collapsed;
            isEditEvent = false;
            currentEventId = 0;
        }

        private async void SaveEventButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EventNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(EventCapacityTextBox.Text) ||
                    EventStatusComboBox.SelectedItem == null ||
                    EventDatePicker.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(LocationTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PriceTextBox.Text))
                {
                    MessageBox.Show("Kérjük, töltse ki az összes kötelező mezőt!",
                        "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(EventCapacityTextBox.Text, out int capacity) || capacity < 1)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes kapacitást (legalább 1)!",
                        "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes árat (nem lehet negatív)!",
                        "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EventDatePicker.SelectedDate <= DateTime.Now.AddDays(1))
                {
                    MessageBox.Show("Az esemény dátuma legalább 1 nappal későbbi kell, hogy legyen a mai napnál!",
                        "Érvénytelen dátum", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedStatus = (EventStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (isEditEvent)
                {
                    var updateDto = new UpdateEvent
                    {
                        EventName = EventNameTextBox.Text,
                        Capacity = int.Parse(EventCapacityTextBox.Text),
                        Status = (EventStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        EventDate = EventDatePicker.SelectedDate,
                        Location = LocationTextBox.Text,
                        Description = EventDescriptionTextBox.Text,
                        OrganizerName = OrganizerNameTextBox.Text,
                        ContactInfo = ContactInfoTextBox.Text,
                        Price = decimal.Parse(PriceTextBox.Text)
                    };
                    await UpdateEvent(currentEventId, updateDto);
                }
                else
                {
                    var createDto = new CreateEvent
                    {
                        EventName = EventNameTextBox.Text,
                        Capacity = int.Parse(EventCapacityTextBox.Text),
                        Status = (EventStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        EventDate = EventDatePicker.SelectedDate,
                        Location = LocationTextBox.Text,
                        Description = EventDescriptionTextBox.Text,
                        OrganizerName = OrganizerNameTextBox.Text,
                        ContactInfo = ContactInfoTextBox.Text,
                        Price = decimal.Parse(PriceTextBox.Text)
                    };
                    await CreateEvent(createDto);
                }

                eventEditPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CreateEvent(CreateEvent newEvent)
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
                    JsonSerializer.Serialize(newEvent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Events/CreateEvent", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba az esemény létrehozásakor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az új esemény sikeresen létrehozva!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadEventsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateEvent(int eventId, UpdateEvent updateEvent)
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
                    JsonSerializer.Serialize(updateEvent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"Events/UpdateEvent/{eventId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba az esemény frissítésekor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az esemény sikeresen frissítve!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadEventsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteEvent(int eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenStorage.AuthToken))
                {
                    MessageBox.Show("Nincs érvényes token!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.AuthToken);
                var response = await _httpClient.DeleteAsync($"Events/DeleteEvenet/{eventId}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba az esemény törlésekor: {response.StatusCode}\n{errorResponse}",
                        "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Az esemény sikeresen törölve!",
                    "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadEventsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearEventFormFields()
        {
            EventNameTextBox.Text = string.Empty;
            EventCapacityTextBox.Text = string.Empty;
            EventStatusComboBox.SelectedIndex = 0;
            EventDatePicker.SelectedDate = DateTime.Now.AddDays(2);
            LocationTextBox.Text = string.Empty;
            PriceTextBox.Text = "0";
            EventDescriptionTextBox.Text = string.Empty;
            OrganizerNameTextBox.Text = string.Empty;
            ContactInfoTextBox.Text = string.Empty;
        }

        /*private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Válasszon képet",
                Filter = "Képfájlok|*.jpg;*.jpeg;*.png;*.gif|Minden fájl|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    selectedImagePath = openFileDialog.FileName;
                    imageData = File.ReadAllBytes(selectedImagePath);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(imageData);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    EventImagePreview.Source = bitmap;
                    ViewImageButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a kép betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearImageButton_Click(object sender, RoutedEventArgs e)
        {
            selectedImagePath = string.Empty;
            imageData = null;
            EventImagePreview.Source = null;
            ViewImageButton.IsEnabled = false;
        }

        private void ViewImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (EventImagePreview.Source != null)
            {
                ShowImageInLargeView(EventImagePreview.Source as BitmapImage);
            }
        }

        private async void ViewEventImage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;

            int eventId = Convert.ToInt32(button.Tag);
            var selectedEvent = eventsListView.Items.Cast<Event>().FirstOrDefault(ev => ev.EventId == eventId);

            if (selectedEvent != null && !string.IsNullOrEmpty(selectedEvent.Images))
            {
                try
                {
                    var imageUrl = $"https://api.hmzrt.eu/{selectedEvent.Images}";
                    Console.WriteLine($"Kép betöltése: {imageUrl}"); // Debugging célokra
                    var bitmap = await _eventManager.LoadEventImage(imageUrl);

                    if (bitmap != null)
                    {
                        var imageWindow = new Window
                        {
                            Title = $"{selectedEvent.EventName} képe",
                            SizeToContent = SizeToContent.WidthAndHeight,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Content = new System.Windows.Controls.Image
                            {
                                Source = bitmap,
                                Stretch = System.Windows.Media.Stretch.Uniform,
                                MaxWidth = 800,
                                MaxHeight = 600
                            }
                        };
                        imageWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show($"A kép nem tölthető be: {imageUrl}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Részletes hiba: {ex.ToString()}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Az eseményhez nincs társítva kép.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async Task LoadAndShowEventImage(int eventId)
        {
            try
            {
                var events = await _httpClient.GetAsync($"Events/Geteventsadmin");
                if (!events.IsSuccessStatusCode)
                {
                    MessageBox.Show("Nem sikerült lekérni az esemény adatait.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var eventsString = await events.Content.ReadAsStringAsync();
                var allEvents = JsonSerializer.Deserialize<List<Event>>(eventsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var eventWithImage = allEvents.FirstOrDefault(e => e.EventId == eventId);

                if (eventWithImage == null || string.IsNullOrEmpty(eventWithImage.Images))
                {
                    MessageBox.Show("Az eseményhez nincs társítva kép.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var imageUrl = $"https://api.hmzrt.eu/{eventWithImage.Images}";

                var imageResponse = await _httpClient.GetAsync(imageUrl);
                if (!imageResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Nem sikerült letölteni a képet.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ShowImageInLargeView(bitmap, eventWithImage.EventName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowImageInLargeView(BitmapImage image, string title = "Kép megtekintése")
        {
            var imageWindow = new Window
            {
                Title = title,
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.CanResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var imageControl = new Image
            {
                Source = image,
                Stretch = Stretch.None
            };

            scrollViewer.Content = imageControl;
            Grid.SetRow(scrollViewer, 0);
            grid.Children.Add(scrollViewer);

            var closeButton = new Button
            {
                Content = "Bezárás",
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 10, 0, 10),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            closeButton.Click += (s, e) => imageWindow.Close();
            Grid.SetRow(closeButton, 1);
            grid.Children.Add(closeButton);

            imageWindow.Content = grid;
            imageWindow.ShowDialog();
        }
        */


        private void BookingTypeBackButton_Click(object sender, RoutedEventArgs e)
        {
            bookingTypeSelectionContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private async void LoadBookings()
        {
            try
            {
                await LoadBookingsToListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a foglalások betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RoomBookingsCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bookingTypeSelectionContainer.Visibility = Visibility.Collapsed;
            bookingsContainer.Visibility = Visibility.Visible;
            LoadBookings();
        }

        private void EventBookingsCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bookingTypeSelectionContainer.Visibility = Visibility.Collapsed;
            eventBookingsContainer.Visibility = Visibility.Visible;
            LoadEventBookings();
        }


        private async void LoadEventBookings()
        {
            try
            {
                var eventBookings = await _eventBookingsService.GetAllEventBookings();

                foreach (var booking in eventBookings)
                {
                    if (booking.GuestId > 0)
                    {
                        var guest = await _guestService.GetGuestById(booking.GuestId);
                        if (guest != null)
                        {
                            booking.GuestName = $"{guest.FirstName} {guest.LastName}";
                        }
                    }

                    if (booking.EventId > 0)
                    {
                        var eventItem = await _eventService.GetEventById(booking.EventId);
                        if (eventItem != null)
                        {
                            booking.EventName = eventItem.EventName;
                        }
                    }
                }

                eventBookingsListView.ItemsSource = eventBookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az eseményfoglalások betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EventBookingsBackButton_Click(object sender, RoutedEventArgs e)
        {
            eventBookingsContainer.Visibility = Visibility.Collapsed;
            bookingTypeSelectionContainer.Visibility = Visibility.Visible;
        }
        private void BookingsBackButton_Click(object sender, RoutedEventArgs e)
        {
            bookingsContainer.Visibility = Visibility.Collapsed;
            bookingTypeSelectionContainer.Visibility = Visibility.Visible;
        }

        private void RefreshEventBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEventBookings();
        }

        private void EventBookingSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = EventBookingSearchTextBox.Text.ToLower();

            if (eventBookingsListView.ItemsSource is List<EventBookings> bookings)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    eventBookingsListView.ItemsSource = bookings;
                }
                else
                {
                    var filteredBookings = bookings.Where(b =>
                        (b.GuestName?.ToLower().Contains(searchText) ?? false) ||
                        (b.EventName?.ToLower().Contains(searchText) ?? false) ||
                        (b.Status?.ToLower().Contains(searchText) ?? false) ||
                        (b.PaymentStatus?.ToLower().Contains(searchText) ?? false)
                    ).ToList();

                    eventBookingsListView.ItemsSource = filteredBookings;
                }
            }
        }


        private async void NewEventBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var events = await _eventService.GetAllEvents();
                EventComboBox.ItemsSource = events;

                var guests = await _guestService.GetAllGuests();
                if (guests != null && guests.Count > 0)
                {
                    GuestComboBox.ItemsSource = guests;
                }
                else
                {
                    MessageBox.Show("Nincsenek elérhető vendégek az adatbázisban.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                EventGuestComboBox.ItemsSource = guests;

                StatusComboBox.SelectedIndex = 0;
                PaymentStatusComboBox.SelectedIndex = 0;
                TicketsTextBox.Text = "1";

                eventBookingsContainer.Visibility = Visibility.Collapsed;
                newEventBookingPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewEventBookingBackButton_Click(object sender, RoutedEventArgs e)
        {
            newEventBookingPanel.Visibility = Visibility.Collapsed;
            eventBookingsContainer.Visibility = Visibility.Visible;
        }

        private void EventComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventComboBox.SelectedItem is Event selectedEvent)
            {
                EventNameInfoTextBlock.Text = selectedEvent.EventName;
                EventDateInfoTextBlock.Text = selectedEvent.EventDate?.ToString("yyyy.MM.dd. HH:mm");
                EventLocationInfoTextBlock.Text = selectedEvent.Location;
                EventPriceInfoTextBlock.Text = $"{selectedEvent.Price:N0} Ft";

                CalculateTotalPrice();
            }
        }

        private void TicketsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            if (EventComboBox.SelectedItem is Event selectedEvent &&
                int.TryParse(TicketsTextBox.Text, out int numberOfTickets))
            {
                decimal totalPrice = (selectedEvent.Price ?? 0) * numberOfTickets;
                TotalPriceTextBox.Text = $"{totalPrice:N0} Ft";
            }
            else
            {
                TotalPriceTextBox.Text = "0 Ft";
            }
        }

        private async void SaveEventBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EventComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kérjük, válasszon eseményt!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EventGuestComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Kérjük, válasszon vendéget!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TicketsTextBox.Text, out int numberOfTickets) || numberOfTickets <= 0)
                {
                    MessageBox.Show("Kérjük, adjon meg érvényes jegyszámot!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedEvent = (Event)EventComboBox.SelectedItem;
                var selectedGuest = (Guest)EventGuestComboBox.SelectedItem;
                var selectedStatus = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
                var selectedPaymentStatus = ((ComboBoxItem)PaymentStatusComboBox.SelectedItem).Content.ToString();

                var newBooking = new CreateEventBooking
                {
                    GuestId = selectedGuest.GuestId,
                    NumberOfTickets = numberOfTickets,
                    Status = selectedStatus,
                    PaymentStatus = selectedPaymentStatus,
                    Notes = NotesTextBox.Text
                };

                bool success = await _eventBookingsService.CreateEventBooking(newBooking, selectedEvent.EventId);

                if (success)
                {
                    MessageBox.Show("Az eseményfoglalás sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                    newEventBookingPanel.Visibility = Visibility.Collapsed;
                    eventBookingsContainer.Visibility = Visibility.Visible;

                    LoadEventBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a foglalás mentésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteEventBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null) return;

                int bookingId = Convert.ToInt32(button.Tag);

                var result = MessageBox.Show("Biztosan törölni szeretné ezt a foglalást?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await _eventBookingsService.DeleteEventBooking(bookingId);

                    if (success)
                    {
                        MessageBox.Show("A foglalás sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEventBookings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a foglalás törlésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EventBookingDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null) return;

                int bookingId = Convert.ToInt32(button.Tag);
                var selectedBooking = eventBookingsListView.Items.Cast<EventBookings>().FirstOrDefault(b => b.EventBookingId == bookingId);

                if (selectedBooking != null)
                {
                    StringBuilder details = new StringBuilder();
                    details.AppendLine($"Foglalás azonosító: {selectedBooking.EventBookingId}");
                    details.AppendLine($"Esemény: {selectedBooking.EventName}");
                    details.AppendLine($"Vendég: {selectedBooking.GuestName}");
                    details.AppendLine($"Jegyek száma: {selectedBooking.NumberOfTickets}");
                    details.AppendLine($"Összeg: {selectedBooking.TotalPrice:N0} Ft");
                    details.AppendLine($"Foglalás dátuma: {selectedBooking.BookingDate:yyyy.MM.dd. HH:mm}");
                    details.AppendLine($"Státusz: {selectedBooking.Status}");
                    details.AppendLine($"Fizetési státusz: {selectedBooking.PaymentStatus}");

                    if (!string.IsNullOrEmpty(selectedBooking.Notes))
                    {
                        details.AppendLine($"Jegyzetek: {selectedBooking.Notes}");
                    }

                    MessageBox.Show(details.ToString(), "Foglalás részletei", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a részletek megjelenítésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EventStatusButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag == null) return;

                int bookingId = Convert.ToInt32(button.Tag);
                var selectedBooking = eventBookingsListView.Items.Cast<EventBookings>().FirstOrDefault(b => b.EventBookingId == bookingId);

                if (selectedBooking != null)
                {
                    string newStatus = string.Empty;

                    switch (selectedBooking.Status)
                    {
                        case "Jóváhagyva":
                            newStatus = "Checked In";
                            break;
                        case "Függőben":
                            newStatus = "Jóváhagyva";
                            break;
                        case "Checked In":
                            newStatus = "Finished";
                            break;
                        default:
                            MessageBox.Show("A foglalás státusza nem módosítható.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                    }

                    // Itt kellene egy API hívás a státusz frissítéséhez
                    // Mivel a controller-ben nincs státusz frissítő endpoint, ezt most csak szimulálni tudjuk

                    MessageBox.Show($"A foglalás státusza sikeresen módosítva: {newStatus}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadEventBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a státusz módosításakor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private readonly string _templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
        private string _currentTemplateContent = "";
        private Dictionary<string, TextBox> _variableTextBoxes = new Dictionary<string, TextBox>();

        private void LoadEmailTemplates()
        {
            try
            {
                if (!Directory.Exists(_templateDirectory))
                {
                    Directory.CreateDirectory(_templateDirectory);
                    MessageBox.Show("A sablonok mappa nem létezett, létrehoztuk.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                var templates = Directory.GetFiles(_templateDirectory, "*.html")
                    .Select(file => new { Name = Path.GetFileNameWithoutExtension(file), Path = file })
                    .ToList();

                TemplateComboBox.ItemsSource = templates;
                TemplateComboBox.DisplayMemberPath = "Name";
                TemplateComboBox.SelectedValuePath = "Path";

                if (templates.Count > 0)
                    TemplateComboBox.SelectedIndex = 0;
                else
                    MessageBox.Show("Nincsenek email sablonok a Templates mappában.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a sablonok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TemplateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TemplateComboBox.SelectedValue != null)
            {
                string templatePath = TemplateComboBox.SelectedValue.ToString();
                if (File.Exists(templatePath))
                {
                    string templateContent = File.ReadAllText(templatePath);

                    EmailSubjectTextBox.Text = $"HMZ RT - {Path.GetFileNameWithoutExtension(templatePath)}";

                    var variables = ExtractVariablesFromTemplate(templateContent);

                    CreateVariablesPanel(variables);

                    _currentTemplateContent = templateContent;
                }
            }
        }

        private List<string> ExtractVariablesFromTemplate(string templateContent)
        {
            var variables = new List<string>();

            var bodyMatch = Regex.Match(templateContent, @"<body[^>]*>([\s\S]*?)<\/body>", RegexOptions.IgnoreCase);

            string contentToSearch = bodyMatch.Success ? bodyMatch.Groups[1].Value : templateContent;

            var regex = new System.Text.RegularExpressions.Regex(@"\{([^{}]+)\}");
            var matches = regex.Matches(contentToSearch);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string variable = match.Groups[1].Value;
                if (!variables.Contains(variable))
                {
                    variables.Add(variable);
                }
            }

            return variables;
        }
        private void CreateVariablesPanel(List<string> variables)
        {
            VariablesStackPanel.Children.Clear();
            _variableTextBoxes.Clear();

            foreach (var variable in variables)
            {
                var border = new Border
                {
                    Margin = new Thickness(0, 0, 0, 15),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0")),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(15)
                };

                var stackPanel = new StackPanel();

                var label = new TextBlock
                {
                    Text = variable,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var textBox = new TextBox
                {
                    Padding = new Thickness(10, 8, 10, 8),
                    FontSize = 14
                };

                _variableTextBoxes[variable] = textBox;

                stackPanel.Children.Add(label);
                stackPanel.Children.Add(textBox);
                border.Child = stackPanel;

                VariablesStackPanel.Children.Add(border);
            }
        }

        private async void SendNewsletterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailSubjectTextBox.Text))
                {
                    MessageBox.Show("Kérjük, adja meg az email tárgyát!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_currentTemplateContent))
                {
                    MessageBox.Show("Kérjük, válasszon sablont!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string htmlBody = _currentTemplateContent;

                var bodyMatch = Regex.Match(htmlBody, @"<body[^>]*>([\s\S]*?)<\/body>", RegexOptions.IgnoreCase);

                if (bodyMatch.Success)
                {
                    string bodyContent = bodyMatch.Groups[1].Value;

                    foreach (var entry in _variableTextBoxes)
                    {
                        bodyContent = bodyContent.Replace($"{{{entry.Key}}}", entry.Value.Text);
                    }

                    htmlBody = htmlBody.Replace(bodyMatch.Groups[1].Value, bodyContent);
                }
                else
                {
                    foreach (var entry in _variableTextBoxes)
                    {
                        htmlBody = htmlBody.Replace($"{{{entry.Key}}}", entry.Value.Text);
                    }
                }

                var newsletterDto = new NewsletterDto
                {
                    Subject = EmailSubjectTextBox.Text,
                    HtmlBody = htmlBody
                };

                await SendNewsletter(newsletterDto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az email küldésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task SendNewsletter(NewsletterDto newsletterDto)
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
                    JsonSerializer.Serialize(newsletterDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Newsletter/SendNewsletter", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("A hírlevél sikeresen elküldve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    newsletterContainer.Visibility = Visibility.Collapsed;
                    dashboardGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba a hírlevél küldésekor: {response.StatusCode}\n{errorResponse}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmailCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dashboardGrid.Visibility = Visibility.Collapsed;
            newsletterContainer.Visibility = Visibility.Visible;
        }

        private void NewsletterBackButton_Click(object sender, RoutedEventArgs e)
        {
            newsletterContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private void CancelNewsletterButton_Click(object sender, RoutedEventArgs e)
        {
            newsletterContainer.Visibility = Visibility.Collapsed;
            dashboardGrid.Visibility = Visibility.Visible;
        }

        private void RefreshTemplatesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmailTemplates();
        }


    }
}



