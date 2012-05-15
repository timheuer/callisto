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
using System.Globalization;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
    public sealed class TimePicker : DateTimePickerBase
    {
        private string _fallbackValueStringFormat;
        private ComboBox _primarySelector;
        private ComboBox _secondarySelector;
        private ComboBox _tertiarySelector;

        public TimePicker()
        {
            this.DefaultStyleKey = typeof(TimePicker);
            Value = DateTime.Now.AddHours(-12);

            Loaded += OnTimePickerLoaded;
        }

        void OnTimePickerLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int delta = 12;

            List<string> hours = new List<string>();// { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            for (int i = 0; i < 12; i++)
            {
                hours.Add((i + 1).ToString().PadLeft(2, '0'));
            }

            if (DateTimeWrapper.CurrentCultureUsesTwentyFourHourClock())
            {
                delta = 0;
                for (int i = 0; i < 12; i++)
                {
                    hours.Add((i + 13).ToString().PadLeft(2, '0'));
                }
                hours.Insert(0, "00");
                _tertiarySelector.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            List<string> minutes = new List<string>();
            List<string> ampm = new List<string>() { "AM", "PM" };
            for (int i = 0; i < 60; i++)
            {
                minutes.Add((i + 1).ToString().PadLeft(2,'0'));
            }
            _primarySelector.ItemsSource = hours;
            _secondarySelector.ItemsSource = minutes;
            _tertiarySelector.ItemsSource = ampm;

            //_primarySelector.SelectedIndex = Value.HasValue ? Value.Value.Hour - delta - 1 : DateTime.Now.Hour - delta - 1;
            //_secondarySelector.SelectedIndex = Value.HasValue ? Value.Value.Minute -1 : DateTime.Now.Minute - 1;
            //_tertiarySelector.SelectedIndex = GetAmPmValue(Value);
        }

        private int GetAmPmValue(DateTime? Value)
        {
            if (Value.HasValue) { return (Value.Value.Hour > 11) ? 1 : 0; }
            return (new Windows.Globalization.Calendar()).Period - 1;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _primarySelector = GetTemplateChild("PrimarySelector") as ComboBox;
            _secondarySelector = GetTemplateChild("SecondarySelector") as ComboBox;
            _tertiarySelector = GetTemplateChild("TertiarySelector") as ComboBox;
        }

        /// <summary>
        /// Gets the fallback value for the ValueStringFormat property.
        /// </summary>
        protected override string ValueStringFormatFallback
        {
            get
            {
                if (null == _fallbackValueStringFormat)
                {
                    // Need to convert LongTimePattern into ShortTimePattern to work around a platform bug
                    // such that only LongTimePattern respects the "24-hour clock" override setting.
                    // This technique is not perfect, but works for all the initially-supported languages.
                    string pattern = base.PreferredCulture.DateTimeFormat.LongTimePattern.Replace(":ss", "");
                    _fallbackValueStringFormat = "{0:" + pattern + "}";
                }
                return _fallbackValueStringFormat;
            }
        }
    }
}
#endif