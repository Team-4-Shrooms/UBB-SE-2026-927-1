using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MovieApp.Features.ReelsEditing.Views 
{
    public class EqualityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;

            return value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
