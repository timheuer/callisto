using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Callisto.Converters
{
    public class ColorContrastConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = (SolidColorBrush)value;
            var yiq = ((brush.Color.R * 299) + (brush.Color.G * 587) + (brush.Color.B * 114)) / 1000;
            Color contrastColor;
            bool invert = (parameter != null) && System.Convert.ToBoolean(parameter);

            // check to see if we actually need to invert
            contrastColor = invert
                                ? ((yiq >= 128) ? Colors.White : Colors.Black)
                                : ((yiq >= 128) ? Colors.Black : Colors.White);

            return new SolidColorBrush(contrastColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
