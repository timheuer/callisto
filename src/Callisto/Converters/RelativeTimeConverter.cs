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

using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Data;

namespace Callisto.Converters
{
    /// <summary>
    /// Time converter to display elapsed time relatively to the present.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class RelativeTimeConverter : IValueConverter
    {
        /// <summary>
        /// A minute defined in seconds.
        /// </summary>
        private const double Minute = 60.0;

        /// <summary>
        /// An hour defined in seconds.
        /// </summary>
        private const double Hour = 60.0 * Minute;

        /// <summary>
        /// A day defined in seconds.
        /// </summary>
        private const double Day = 24 * Hour;

        /// <summary>
        /// A week defined in seconds.
        /// </summary>
        private const double Week = 7 * Day;

        /// <summary>
        /// A month defined in seconds.
        /// </summary>
        private const double Month = 30.5 * Day;

        /// <summary>
        /// A year defined in seconds.
        /// </summary>
        private const double Year = 365 * Day;

        /// <summary>
        /// Abbreviation for the default culture used by resources files.
        /// </summary>
        private const string DefaultCulture = "en-US";

        /// <summary>
        /// Four different strings to express hours in plural.
        /// </summary>
        private string[] PluralHourStrings;

        /// <summary>
        /// Four different strings to express minutes in plural.
        /// </summary>
        private string[] PluralMinuteStrings;

        /// <summary>
        /// Four different strings to express seconds in plural.
        /// </summary>
        private string[] PluralSecondStrings;

        /// <summary>
        /// Wrapper for the ResourceLoader to get the string resources
        /// </summary>
        private static ResourceLoader TimeResources
        {
            get
            {
                return new Windows.ApplicationModel.Resources.ResourceLoader("Callisto/Resources");
            }
        }

        /// <summary>
        /// CultureInfo object using the user's preferred language list
        /// </summary>
        private static CultureInfo PreferredCulture
        {
            get
            {
                return new CultureInfo(CurrentLanguageTag);
            }
        }

        /// <summary>
        /// Gets the current BCP-47 language from the user's language preference list
        /// </summary>
        public static string CurrentLanguageTag
        {
            get
            {
                return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages[0].ToString();
            }
        }

        /// <summary>
        /// Resources use the culture in the system locale by default.
        /// The converter must use the culture specified the ConverterCulture.
        /// The ConverterCulture defaults to en-US when not specified.
        /// Thus, change the resources culture only if ConverterCulture is set.
        /// </summary>
        /// <param name="culture">The culture to use in the converter.</param>
        private void SetLocalizationCulture(CultureInfo culture)
        {
            //if (!culture.Name.Equals(DefaultCulture, StringComparison.Ordinal))
            //{
            //    ControlResources.Culture = culture;
            //}

            PluralHourStrings = new string[4] { 
                  TimeResources.GetString("XHoursAgo_2To4"), 
                  TimeResources.GetString("XHoursAgo_EndsIn1Not11"), 
                  TimeResources.GetString("XHoursAgo_EndsIn2To4Not12To14"), 
                  TimeResources.GetString("XHoursAgo_Other") 
              };

            PluralMinuteStrings = new string[4] { 
                  TimeResources.GetString("XMinutesAgo_2To4"), 
                  TimeResources.GetString("XMinutesAgo_EndsIn1Not11"), 
                  TimeResources.GetString("XMinutesAgo_EndsIn2To4Not12To14"), 
                  TimeResources.GetString("XMinutesAgo_Other")
              };

            PluralSecondStrings = new string[4] { 
                  TimeResources.GetString("XSecondsAgo_2To4"), 
                  TimeResources.GetString("XSecondsAgo_EndsIn1Not11"), 
                  TimeResources.GetString("XSecondsAgo_EndsIn2To4Not12To14"), 
                  TimeResources.GetString("XSecondsAgo_Other") 
              };
        }

        /// <summary>
        /// Returns a localized text string to express months in plural.
        /// </summary>
        /// <param name="month">Number of months.</param>
        /// <returns>Localized text string.</returns>
        private static string GetPluralMonth(int month)
        {
            if (month >= 2 && month <= 4)
            {
                return string.Format(PreferredCulture, TimeResources.GetString("XMonthsAgo_2To4"), month.ToString(PreferredCulture));
            }
            else if (month >= 5 && month <= 12)
            {
                return string.Format(PreferredCulture, TimeResources.GetString("XMonthsAgo_5To12"), month.ToString(PreferredCulture));
            }
            else
            {
                throw new ArgumentException(TimeResources.GetString("InvalidNumberOfMonths"));
            }
        }

        /// <summary>
        /// Returns a localized text string to express time units in plural.
        /// </summary>
        /// <param name="units">
        /// Number of time units, e.g. 5 for five months.
        /// </param>
        /// <param name="resources">
        /// Resources related to the specified time unit.
        /// </param>
        /// <returns>Localized text string.</returns>
        private static string GetPluralTimeUnits(int units, string[] resources)
        {
            int modTen = units % 10;
            int modHundred = units % 100;

            if (units <= 1)
            {
                throw new ArgumentException(TimeResources.GetString("InvalidNumberOfTimeUnits"));
            }
            else if (units >= 2 && units <= 4)
            {
                return string.Format(PreferredCulture, resources[0], units.ToString(PreferredCulture));
            }
            else if (modTen == 1 && modHundred != 11)
            {
                return string.Format(PreferredCulture, resources[1], units.ToString(PreferredCulture));
            }
            else if ((modTen >= 2 && modTen <= 4) && !(modHundred >= 12 && modHundred <= 14))
            {
                return string.Format(PreferredCulture, resources[2], units.ToString(PreferredCulture));
            }
            else
            {
                return string.Format(PreferredCulture, resources[3], units.ToString(PreferredCulture));
            }
        }

        /// <summary>
        /// Returns a localized text string for the day of week.
        /// </summary>
        /// <param name="dow">Day of week.</param>
        /// <returns>Localized text string.</returns>
        private static string GetDayOfWeek(DayOfWeek dow)
        {
            string result;
            
            switch (dow)
            {
                case DayOfWeek.Monday:
                    result = TimeResources.GetString("Monday");
                    break;
                case DayOfWeek.Tuesday:
                    result = TimeResources.GetString("Tuesday");
                    break;
                case DayOfWeek.Wednesday:
                    result = TimeResources.GetString("Wednesday");
                    break;
                case DayOfWeek.Thursday:
                    result = TimeResources.GetString("Thursday");
                    break;
                case DayOfWeek.Friday:
                    result = TimeResources.GetString("Friday");
                    break;
                case DayOfWeek.Saturday:
                    result = TimeResources.GetString("Saturday");
                    break;
                case DayOfWeek.Sunday:
                    result = TimeResources.GetString("Sunday");
                    break;
                default:
                    result = TimeResources.GetString("Sunday");
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns a localized text string to express "on {0}"
        /// where {0} is a day of the week, e.g. Sunday.
        /// </summary>
        /// <param name="dow">Day of week.</param>
        /// <returns>Localized text string.</returns>
        private static string GetOnDayOfWeek(DayOfWeek dow)
        {
            if (dow == DayOfWeek.Tuesday)
            {
                return string.Format(PreferredCulture, TimeResources.GetString("OnDayOfWeek_Tuesday"), GetDayOfWeek(dow));
            }
            else
            {
                return string.Format(PreferredCulture, TimeResources.GetString("OnDayOfWeek_Other"), GetDayOfWeek(dow));
            }
        }

        /// <summary>
        /// Converts a 
        /// <see cref="T:System.DateTime"/>
        /// object into a string the represents the elapsed time 
        /// relatively to the present.
        /// </summary>
        /// <param name="value">The given date and time.</param>
        /// <param name="targetType">
        /// The type corresponding to the binding property, which must be of
        /// <see cref="T:System.String"/>.
        /// </param>
        /// <param name="parameter">(Not used).</param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// When not specified, the converter uses the current culture
        /// as specified by the system locale.
        /// </param>
        /// <returns>The given date and time as a string.</returns>
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            // Target value must be a System.DateTime object.
            if (!(value is DateTime))
            {
                throw new ArgumentException(TimeResources.GetString("InvalidDateTimeArgument"));
            }

            string result;

            DateTime given = ((DateTime)value).ToLocalTime();

            DateTime current = DateTime.Now;

            TimeSpan difference = current - given;

            SetLocalizationCulture(PreferredCulture);

            if (DateTimeFormatHelper.IsFutureDateTime(current, given))
            {
                // Future dates and times are not supported, but to prevent crashing an app
                // if the time they receive from a server is slightly ahead of the phone's clock
                // we'll just default to the minimum, which is "2 seconds ago".
                result = GetPluralTimeUnits(2, PluralSecondStrings);
            }

            if (difference.TotalSeconds > Year)
            {
                // "over a year ago"
                result = TimeResources.GetString("OverAYearAgo");
            }
            else if (difference.TotalSeconds > (1.5 * Month))
            {
                // "x months ago"
                int nMonths = (int)((difference.TotalSeconds + Month / 2) / Month);
                result = GetPluralMonth(nMonths);
            }
            else if (difference.TotalSeconds >= (3.5 * Week))
            {
                // "about a month ago"
                result = TimeResources.GetString("AboutAMonthAgo");
            }
            else if (difference.TotalSeconds >= Week)
            {
                int nWeeks = (int)(difference.TotalSeconds / Week);
                if (nWeeks > 1)
                {
                    // "x weeks ago"
                    result = string.Format(PreferredCulture, TimeResources.GetString("XWeeksAgo_2To4"), nWeeks.ToString(PreferredCulture));
                }
                else
                {
                    // "about a week ago"
                    result = TimeResources.GetString("AboutAWeekAgo");
                }
            }
            else if (difference.TotalSeconds >= (5 * Day))
            {
                // "last <dayofweek>"    
                result = string.Format(PreferredCulture, TimeResources.GetString("LastDayOfWeek"), GetDayOfWeek(given.DayOfWeek));
            }
            else if (difference.TotalSeconds >= Day)
            {
                // "on <dayofweek>"
                result = GetOnDayOfWeek(given.DayOfWeek);
            }
            else if (difference.TotalSeconds >= (2 * Hour))
            {
                // "x hours ago"
                int nHours = (int)(difference.TotalSeconds / Hour);
                result = GetPluralTimeUnits(nHours, PluralHourStrings);
            }
            else if (difference.TotalSeconds >= Hour)
            {
                // "about an hour ago"
                result = TimeResources.GetString("AboutAnHourAgo");
            }
            else if (difference.TotalSeconds >= (2 * Minute))
            {
                // "x minutes ago"
                int nMinutes = (int)(difference.TotalSeconds / Minute);
                result = GetPluralTimeUnits(nMinutes, PluralMinuteStrings);
            }
            else if (difference.TotalSeconds >= Minute)
            {
                // "about a minute ago"
                result = TimeResources.GetString("AboutAMinuteAgo");
            }
            else
            {
                // "x seconds ago" or default to "2 seconds ago" if less than two seconds.
                int nSeconds = ((int)difference.TotalSeconds > 1.0) ? (int)difference.TotalSeconds : 2;
                result = GetPluralTimeUnits(nSeconds, PluralSecondStrings);
            }

            return result;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">(Not used).</param>
        /// <param name="targetType">(Not used).</param>
        /// <param name="parameter">(Not used).</param>
        /// <param name="culture">(Not used).</param>
        /// <returns>null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}