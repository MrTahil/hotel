using System;
using System.Windows;

namespace RoomListApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }



    }
}
