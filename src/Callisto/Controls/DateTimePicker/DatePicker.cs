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
#if INCLUDE_EXPERIMENTAL
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
    public sealed class DatePicker : DateTimePickerBase
    {
        private ComboBox _primarySelector;
        private ComboBox _secondarySelector;
        private ComboBox _tertiarySelector;

        public DatePicker()
        {
            this.DefaultStyleKey = typeof(DatePicker);
            this.Value = DateTime.Now.Date;

            Loaded += OnDatePickerLoaded;
        }

        void OnDatePickerLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _primarySelector = GetTemplateChild("PrimarySelector") as ComboBox;
            _secondarySelector = GetTemplateChild("SecondarySelector") as ComboBox;
            _tertiarySelector = GetTemplateChild("TertiarySelector") as ComboBox;
        }
    }
}
#endif