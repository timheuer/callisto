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
using Windows.UI.Xaml;

namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. A menu item that can be toggled.
	/// </summary>
	/// <remarks>
	/// This control is deprecated in favor of using the <see cref="Windows.UI.Xaml.Controls.MenuFlyout"/> controls in Windows 8.1.
	/// </remarks>
    [Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as ToggleMenuFlyoutItem.")]
    public sealed class ToggleMenuItem : MenuItem
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ToggleMenuItem"/> class.
		/// </summary>
        public ToggleMenuItem()
        {
            DefaultStyleKey = typeof(ToggleMenuItem);
        }

		/// <summary>
		/// Gets or sets a value indicating whether the menu item is checked.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is checked; otherwise, <c>false</c>.
		/// </value>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="IsChecked"/> dependency property
		/// </summary>
		public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleMenuItem), null);
    }
}
