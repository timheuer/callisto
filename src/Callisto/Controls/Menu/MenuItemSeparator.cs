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
namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. A MenuItem separator
	/// </summary>
	/// <remarks>
	/// This control is deprecated in favor of using the <see cref="Windows.UI.Xaml.Controls.MenuFlyout"/> controls in Windows 8.1.
	/// </remarks>
	/// <seealso cref="MenuItem"/>
	/// <seealso cref="Menu"/>
    [Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as MenuItemSeparator.")]
    public sealed class MenuItemSeparator : MenuItemBase
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItemSeparator"/> class.
		/// </summary>
        public MenuItemSeparator()
        {
            DefaultStyleKey = typeof(MenuItemSeparator);
            IsTabStop = false;
        }
    }
}
