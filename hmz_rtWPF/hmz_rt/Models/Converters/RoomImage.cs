using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace hmz_rt.Models.Converters
{
    public class RoomManager
    {
        private readonly HttpClient _httpClient;
        private readonly string _cacheFolder;

        public RoomManager()
        {
            _httpClient = new HttpClient();
            _cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourAppName", "ImageCache");
            Directory.CreateDirectory(_cacheFolder);
        }

        public async Task<BitmapImage> LoadRoomImage(string imageUrl)
        {
            try
            {
                string fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                string localPath = Path.Combine(_cacheFolder, fileName);

                if (!File.Exists(localPath))
                {
                    byte[] imageData = await _httpClient.GetByteArrayAsync(imageUrl);
                    await Task.Run(() => File.WriteAllBytes(localPath, imageData));
                }

                return new BitmapImage(new Uri(localPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a kép betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}