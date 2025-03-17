using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace hmz_rt
{
    public class PaymentMethodSelector : Window
    {
        public string SelectedPaymentMethod { get; private set; }

        public PaymentMethodSelector()
        {
            Title = "Fizetési mód választása";
            Width = 300;
            Height = 180;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Grid grid = new Grid();
            grid.Margin = new Thickness(15);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            TextBlock titleText = new TextBlock
            {
                Text = "Válassz fizetési módot:",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Button cashButton = new Button
            {
                Content = "Készpénz",
                Padding = new Thickness(15, 5, 15, 5),
                Margin = new Thickness(5),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A")),
                Foreground = Brushes.White
            };
            cashButton.Click += (s, e) => { SelectedPaymentMethod = "Készpénz"; DialogResult = true; };

            Button cardButton = new Button
            {
                Content = "Bankkártya",
                Padding = new Thickness(15, 5, 15, 5),
                Margin = new Thickness(5),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                Foreground = Brushes.White
            };
            cardButton.Click += (s, e) => { SelectedPaymentMethod = "Bankkártya"; DialogResult = true; };

            Button cancelButton = new Button
            {
                Content = "Mégse",
                Padding = new Thickness(15, 5, 15, 5),
                Margin = new Thickness(5),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                Foreground = Brushes.White
            };
            cancelButton.Click += (s, e) => { DialogResult = false; };

            buttonPanel.Children.Add(cashButton);
            buttonPanel.Children.Add(cardButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(titleText, 0);
            Grid.SetRow(buttonPanel, 1);

            grid.Children.Add(titleText);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }
    }
}
