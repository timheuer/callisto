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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
    public abstract class MenuItemBase : Control
    {
        protected MenuItemBase() { }

        public Thickness MenuTextMargin
        {
            get { return (Thickness)GetValue(MenuTextMarginProperty); }
            set { SetValue(MenuTextMarginProperty, value); }
        }

        public static readonly DependencyProperty MenuTextMarginProperty =
            DependencyProperty.Register("MenuTextMargin", typeof(Thickness), typeof(MenuItemBase), null);

    }
}
