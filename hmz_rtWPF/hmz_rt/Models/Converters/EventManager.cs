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
    public class EventImageManager // Átnevezve az ütközés elkerülésére
    {
        private readonly HttpClient _httpClient;
        private readonly string _cacheFolder;

        public EventImageManager() {
            _httpClient = new HttpClient();
            _cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourAppName", "EventImageCache");
            Directory.CreateDirectory(_cacheFolder);
        }

        public async Task<BitmapImage> LoadEventImage(string imageUrl) {
            try {
                string fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                string localPath = Path.Combine(_cacheFolder, fileName);

                if (!File.Exists(localPath))
                {
                    byte[] imageData = await _httpClient.GetByteArrayAsync(imageUrl);
                    await Task.Run(() => File.WriteAllBytes(localPath, imageData));
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(localPath);
                bitmap.EndInit();
                bitmap.Freeze(); // Ez javítja a teljesítményt és megakadályozza a szálak közötti hozzáférési problémákat

                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az esemény képének betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // Opcionális: Kép törlése a gyorsítótárból
        public void ClearImageCache(string imageUrl)
        {
            try
            {
                string fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                string localPath = Path.Combine(_cacheFolder, fileName);

                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a kép törlésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Opcionális: Teljes gyorsítótár törlése
        public void ClearAllCache()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_cacheFolder);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a gyorsítótár törlésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

