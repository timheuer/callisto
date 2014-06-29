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
	/// <summary>
	/// This implementation of an IValueConverter is intended to look at a specific color value
	/// and determine the contrast based on YIQ and return either <see cref="Windows.UI.Colors.Black"/>
	/// or <see cref="Windows.UI.Colors.White"/> based on the contrast.
	/// </summary>
	/// <remarks>
	/// This is internally used by SettingsFlyout to determine if the Foreground of the header text 
	/// should be White/Black based on the HeaderBrush color chosen.
	/// </remarks>
    public class ColorContrastConverter : IValueConverter
    {
		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The type of the target property. This uses a different type
		/// depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="language">The language of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
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

		/// <summary>
		/// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The type of the target property, specified by a helper structure that wraps the type name.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="language">The language of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
