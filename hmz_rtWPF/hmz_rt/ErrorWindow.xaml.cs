using System.Windows;

namespace RoomListApp
{
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string errorMessage)
        {
            InitializeComponent();
            txtErrorDetails.Text = errorMessage;
        }
    }
}
