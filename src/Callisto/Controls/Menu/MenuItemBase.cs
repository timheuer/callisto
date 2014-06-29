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
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. Base class for the menu items
	/// </summary>
	/// <remarks>
	/// This control is deprecated in favor of using the <see cref="Windows.UI.Xaml.Controls.MenuFlyout"/> controls in Windows 8.1.
	/// </remarks>
	[Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as MenuFlyoutItem.")]
    public abstract class MenuItemBase : Control
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItemBase"/> class.
		/// </summary>
        protected MenuItemBase() { }

		/// <summary>
		/// Gets or sets the menu text margin.
		/// </summary>
		public Thickness MenuTextMargin
        {
            get { return (Thickness)GetValue(MenuTextMarginProperty); }
            set { SetValue(MenuTextMarginProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="MenuTextMargin"/> dependency property
		/// </summary>
        public static readonly DependencyProperty MenuTextMarginProperty =
            DependencyProperty.Register("MenuTextMargin", typeof(Thickness), typeof(MenuItemBase), null);

    }
}
