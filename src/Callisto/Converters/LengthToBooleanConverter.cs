﻿//
// Copyright (c) 2012 Tim Heuer
//
// Licensed under the Microsoft Public License (Ms-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
 
using System;
using Windows.UI.Xaml.Data;

namespace Callisto.Converters
{
	/// <summary>
	/// Returns <c>true</c> is a string value has a length greater than 0.
	/// </summary>
    public class StringLengthToBooleanConverter : IValueConverter
    {
		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The type of the target property. This uses a different type
		/// depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX).</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="language">The language of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (((string)value).Length > 0);
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
		/// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
