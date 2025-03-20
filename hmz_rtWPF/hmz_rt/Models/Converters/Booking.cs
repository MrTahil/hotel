using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace hmz_rt.Models.Converters
{
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
}
