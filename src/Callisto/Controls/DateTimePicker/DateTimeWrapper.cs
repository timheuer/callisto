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

// BASE CODE PROVIDED UNDER THIS LICENSE
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
#if INCLUDE_EXPERIMENTAL
using System;
using System.Globalization;

namespace Callisto.Controls
{
    public class DateTimeWrapper
    {
        /// <summary>
        /// Gets the CultureInfo based on the first language in the OS language list preferences
        /// </summary>
        public CultureInfo PreferredCulture { get; private set; }

        /// <summary>
        /// Gets the DateTime being wrapped.
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Gets the 4-digit year as a string.
        /// </summary>
        public string YearNumber { get { return DateTime.ToString("yyyy", PreferredCulture); } }

        /// <summary>
        /// Gets the 2-digit month as a string.
        /// </summary>
        public string MonthNumber { get { return DateTime.ToString("MM", PreferredCulture); } }

        /// <summary>
        /// Gets the month name as a string.
        /// </summary>
        public string MonthName { get { return DateTime.ToString("MMMM", PreferredCulture); } }

        /// <summary>
        /// Gets the 2-digit day as a string.
        /// </summary>
        public string DayNumber { get { return DateTime.ToString("dd", PreferredCulture); } }

        /// <summary>
        /// Gets the day name as a string.
        /// </summary>
        public string DayName { get { return DateTime.ToString("dddd", PreferredCulture); } }

        /// <summary>
        /// Gets the hour as a string.
        /// </summary>
        public string HourNumber { get { return DateTime.ToString(CurrentCultureUsesTwentyFourHourClock() ? "%H" : "%h", PreferredCulture); } }

        /// <summary>
        /// Gets the 2-digit minute as a string.
        /// </summary>
        public string MinuteNumber { get { return DateTime.ToString("mm", PreferredCulture); } }

        /// <summary>
        /// Gets the AM/PM designator as a string.
        /// </summary>
        public string AmPmString { get { return DateTime.ToString("tt", PreferredCulture); } }

        /// <summary>
        /// Initializes a new instance of the DateTimeWrapper class.
        /// </summary>
        /// <param name="dateTime">DateTime to wrap.</param>
        public DateTimeWrapper(DateTime dateTime)
        {
            DateTime = dateTime;

            PreferredCulture = new CultureInfo(CurrentLanguageTag);

        }

        /// <summary>
        /// Returns a value indicating whether the current culture uses a 24-hour clock.
        /// </summary>
        /// <returns>True if it uses a 24-hour clock; false otherwise.</returns>
        public static bool CurrentCultureUsesTwentyFourHourClock()
        {
            CultureInfo ci = new CultureInfo(CurrentLanguageTag);
            return !ci.DateTimeFormat.LongTimePattern.Contains("t");
        }

        public static string CurrentLanguageTag
        {
            get
            {
                Windows.Globalization.Language lang = new Windows.Globalization.Language();
                return lang.LanguageTag;
                //return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages[0].ToString();
            }
        }
    }
}
#endif