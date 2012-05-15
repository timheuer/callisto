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
using System.Text;
using System.Text.RegularExpressions;

namespace Callisto.Converters
{
    /// <summary>
    /// Helper methods and constants for the date time converters.
    /// </summary>
    internal static class DateTimeFormatHelper
    {
        /// <summary>
        /// An hour defined in minutes.
        /// </summary>
        private const double Hour = 60.0;

        /// <summary>
        /// A day defined in minutes.
        /// </summary>
        private const double Day = 24 * Hour;

        /// <summary>
        /// Format pattern for single-character meridiem designators.
        /// e.g. 4:30p
        /// </summary>
        private const string SingleMeridiemDesignator = "t";

        /// <summary>
        /// Format pattern for double-character meridiem designators.
        /// e.g. 4:30 p.m.
        /// </summary>
        private const string DoubleMeridiemDesignator = "tt";

        /// <summary>
        /// Date and time information used when getting the super short time
        /// pattern. Syncs with the current culture.
        /// </summary>
        private static DateTimeFormatInfo formatInfo_GetSuperShortTime = null;

        /// <summary>
        /// Date and time information used when getting the month and day
        /// pattern. Syncs with the current culture.
        /// </summary>
        private static DateTimeFormatInfo formatInfo_GetMonthAndDay = null;

        /// <summary>
        /// Date and time information used when getting the short time
        /// pattern. Syncs with the current culture.
        /// </summary>
        private static DateTimeFormatInfo formatInfo_GetShortTime = null;

        /// <summary>
        /// Lock object used to delimit a critical region when getting
        /// the super short time pattern.
        /// </summary>
        private static Object lock_GetSuperShortTime = new Object();

        /// <summary>
        /// Lock object used to delimit a critical region when getting
        /// the month and day pattern.
        /// </summary>
        private static Object lock_GetMonthAndDay = new Object();

        /// <summary>
        /// Lock object used to delimit a critical region when getting
        /// the short time pattern.
        /// </summary>
        private static Object lock_GetShortTime = new Object();

        /// <summary>
        /// Regular expression used to get the month and day pattern.
        /// </summary>
        private static readonly Regex rxMonthAndDay = new Regex("(d{1,2}[^A-Za-z]M{1,3})|(M{1,3}[^A-Za-z]d{1,2})");

        /// <summary>
        /// Regular expression used to get the seconds pattern with separator.
        /// </summary>
        private static readonly Regex rxSeconds = new Regex("([^A-Za-z]s{1,2})");

        /// <summary>
        /// Gets the number representing the day of the week from a given
        /// <see cref="T:System.DateTime"/>
        /// object, relative to the first day of the week 
        /// according to the current culture.
        /// </summary>
        /// <param name="dt">Date information.</param>
        /// <returns>
        /// A number representing the day of the week.
        /// e.g. if Monday is the first day of the week according to the
        /// relative culture, Monday returns 0, Tuesday returns 1, etc.
        /// </returns>
        public static int GetRelativeDayOfWeek(DateTime dt)
        {
            return ((int)dt.DayOfWeek - (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 7) % 7;
        }

        #region DateTime comparison methods

        /// <summary>
        /// Indicates if a given
        /// <see cref="T:System.DateTime"/>
        /// object represents a future instance when compared to another
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="relative">Relative date and time.</param>
        /// <param name="given">Given date and time.</param>
        /// <returns>
        /// True if given date and time represents a future instance.
        /// </returns>
        public static bool IsFutureDateTime(DateTime relative, DateTime given)
        {
            return relative < given;
        }

        /// <summary>
        /// Indicates if a given
        /// <see cref="T:System.DateTime"/>
        /// object represents a past year when compared to another
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="relative">Relative date and time.</param>
        /// <param name="given">Given date and time.</param>
        /// <returns>
        /// True if given date and time is set to a past year.
        /// </returns>
        public static bool IsAnOlderYear(DateTime relative, DateTime given)
        {
            return relative.Year > given.Year;
        }

        /// <summary>
        /// Indicates if a given
        /// <see cref="T:System.DateTime"/>
        /// object represents a past week when compared to another
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="relative">Relative date and time.</param>
        /// <param name="given">Given date and time.</param>
        /// <returns>
        /// True if given date and time is set to a past week.
        /// </returns>
        public static bool IsAnOlderWeek(DateTime relative, DateTime given)
        {
            if (IsAtLeastOneWeekOld(relative, given))
            {
                return true;
            }
            else
            {
                return GetRelativeDayOfWeek(given) > GetRelativeDayOfWeek(relative);
            }
        }

        /// <summary>
        /// Indicates if a given
        /// <see cref="T:System.DateTime"/>
        /// object is at least one week behind from another
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="relative">Relative date and time.</param>
        /// <param name="given">Given date and time.</param>
        /// <returns>
        /// True if given date and time is at least one week behind.
        /// </returns>
        public static bool IsAtLeastOneWeekOld(DateTime relative, DateTime given)
        {
            return ((int)(relative - given).TotalMinutes >= 7 * Day);
        }

        /// <summary>
        /// Indicates if a given
        /// <see cref="T:System.DateTime"/>
        /// object corresponds to a past day in the same week as another
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="relative">Relative date and time.</param>
        /// <param name="given">Given date and time.</param>
        /// <returns>
        /// True if given date and time is a past day in the relative week.
        /// </returns>
        public static bool IsPastDayOfWeek(DateTime relative, DateTime given)
        {
            return GetRelativeDayOfWeek(relative) > GetRelativeDayOfWeek(given);
        }

        /// <summary>
        /// Indicates if a given
        /// <see cref="T:System.DateTime"/>
        /// object corresponds to a past day in the same week as another
        /// <see cref="T:System.DateTime"/>
        /// object and at least three hours behind it.
        /// </summary>
        /// <param name="relative">Relative date and time.</param>
        /// <param name="given">Given date and time.</param>
        /// <returns>
        /// True if given date and time is a past day in the relative week 
        /// and at least three hours behind the relative time.
        /// </returns>
        public static bool IsPastDayOfWeekWithWindow(DateTime relative, DateTime given)
        {
            return IsPastDayOfWeek(relative, given) && ((int)(relative - given).TotalMinutes > 3 * Hour);
        }

        #endregion

        #region Culture awareness methods

        /// <summary>
        /// Identifies if the current culture is set to Japanese.
        /// </summary>
        /// <returns>
        /// True if current culture is set to Japanese.
        /// </returns>
        public static bool IsCurrentCultureJapanese()
        {
            return (CultureInfo.CurrentCulture.Name.StartsWith("ja", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Identifies if the current culture is set to Korean.
        /// </summary>
        /// <returns>
        /// True if current culture is set to Korean.
        /// </returns>
        public static bool IsCurrentCultureKorean()
        {
            return (CultureInfo.CurrentCulture.Name.StartsWith("ko", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Identifies if the current culture is set to Turkish.
        /// </summary>
        /// <returns>
        /// True if current culture is set to Turkish.
        /// </returns>
        public static bool IsCurrentCultureTurkish()
        {
            return (CultureInfo.CurrentCulture.Name.StartsWith("tr", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Identifies if the current culture is set to Hungarian.
        /// </summary>
        /// <returns>
        /// True if current culture is set to Hungarian.
        /// </returns>
        public static bool IsCurrentCultureHungarian()
        {
            return (CultureInfo.CurrentCulture.Name.StartsWith("hu", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Identifies if the current UI culture is set to French.
        /// </summary>
        /// <returns>
        /// True if current UI culture is set to French.
        /// </returns>
        public static bool IsCurrentUICultureFrench()
        {
            return (CultureInfo.CurrentUICulture.Name.Equals("fr-FR", StringComparison.Ordinal));
        }

        #endregion

        #region String generating methods

        /// <summary>
        /// Gets the abbreviated day from a
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="dt">Date information.</param>
        /// <returns>e.g. "Mon" for Monday when en-US.</returns>
        public static string GetAbbreviatedDay(DateTime dt)
        {
            if (DateTimeFormatHelper.IsCurrentCultureJapanese() || DateTimeFormatHelper.IsCurrentCultureKorean())
            {
                return "(" + dt.ToString("ddd", CultureInfo.CurrentCulture) + ")";
            }
            else
            {
                return dt.ToString("ddd", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Gets the time from a 
        /// <see cref="T:System.DateTime"/>
        /// object in short Metro format.
        /// </summary>
        /// <remarks>
        /// Metro design guidelines normalize strings to lowercase.
        /// </remarks>
        /// <param name="dt">Time information.</param>
        /// <returns>"10:20p" for 10:20 p.m. when en-US.</returns>
        public static string GetSuperShortTime(DateTime dt)
        {
            if (formatInfo_GetSuperShortTime == null)
            {
                lock (lock_GetSuperShortTime)
                {
                    StringBuilder result = new StringBuilder(string.Empty);
                    string seconds;

                    formatInfo_GetSuperShortTime = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();

                    result.Append(formatInfo_GetSuperShortTime.LongTimePattern);
                    seconds = rxSeconds.Match(result.ToString()).Value;
                    result.Replace(" ", string.Empty);
                    result.Replace(seconds, string.Empty);
                    if (!(DateTimeFormatHelper.IsCurrentCultureJapanese()
                        || DateTimeFormatHelper.IsCurrentCultureKorean()
                        || DateTimeFormatHelper.IsCurrentCultureHungarian()))
                    {
                        result.Replace(DoubleMeridiemDesignator, SingleMeridiemDesignator);
                    }

                    formatInfo_GetSuperShortTime.ShortTimePattern = result.ToString();
                }
            }

            return dt.ToString("t", formatInfo_GetSuperShortTime).ToLowerInvariant();
        }

        /// <summary>
        /// Gets the month and day from a
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="dt">Date information.</param>
        /// <returns>"05/24" for May 24th when en-US.</returns>
        public static string GetMonthAndDay(DateTime dt)
        {
            if (formatInfo_GetMonthAndDay == null)
            {
                lock (lock_GetMonthAndDay)
                {
                    StringBuilder result = new StringBuilder(string.Empty);

                    formatInfo_GetMonthAndDay = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();

                    result.Append(rxMonthAndDay.Match(formatInfo_GetMonthAndDay.ShortDatePattern).Value);
                    if (result.ToString().Contains("."))
                    {
                        result.Append(".");
                    }

                    formatInfo_GetMonthAndDay.ShortDatePattern = result.ToString();
                }
            }

            return dt.ToString("d", formatInfo_GetMonthAndDay);
        }

        /// <summary>
        /// Gets the date in short pattern from a
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="dt">Date information.</param>
        /// <returns>
        /// Date in short pattern as defined by the system locale.
        /// </returns>
        public static string GetShortDate(DateTime dt)
        {
            return dt.ToString("d", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the time in short pattern from a
        /// <see cref="T:System.DateTime"/>
        /// object.
        /// </summary>
        /// <param name="dt">Date information.</param>
        /// <returns>
        /// Time in short pattern as defined by the system locale.
        /// </returns>
        public static string GetShortTime(DateTime dt)
        {
            if (formatInfo_GetShortTime == null)
            {
                lock (lock_GetShortTime)
                {
                    StringBuilder result = new StringBuilder(string.Empty);
                    string seconds;

                    formatInfo_GetShortTime = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();

                    result.Append(formatInfo_GetSuperShortTime.LongTimePattern);
                    seconds = rxSeconds.Match(result.ToString()).Value;
                    result.Replace(seconds, string.Empty);

                    formatInfo_GetShortTime.ShortTimePattern = result.ToString();
                }
            }

            return dt.ToString("t", formatInfo_GetShortTime);
        }

        #endregion
    }
}