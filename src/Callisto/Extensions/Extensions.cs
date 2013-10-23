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

// SOME BASE CODE PROVIDED UNDER THIS LICENSE
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Callisto.OAuth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Callisto
{
    internal static partial class Extensions
    {
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) >= 0;
        }

        internal static IDictionary<string, object> ParseUrlQueryString(string query)
        {
            var result = new Dictionary<string, object>();

            // if string is null, empty or whitespace
            if (string.IsNullOrEmpty(query) || query.Trim().Length == 0)
            {
                return result;
            }

            string decoded = query;
            int decodedLength = decoded.Length;
            int namePos = 0;
            bool first = true;

            while (namePos <= decodedLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (int q = namePos; q < decodedLength; q++)
                {
                    if (valuePos == -1 && decoded[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (decoded[q] == '&' || decoded[q] == '#')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                if (first)
                {
                    first = false;
                    if (decoded[namePos] == '#')
                    {
                        namePos++;
                    }
                }

                string name, value;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    //name = UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1));
                    name = decoded.Substring(namePos, valuePos - namePos - 1);
                }

                if (valueEnd < 0)
                {
                    namePos = -1;
                    valueEnd = decoded.Length;
                }
                else
                {
                    namePos = valueEnd + 1;
                }

                //value = UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos));
                value = decoded.Substring(valuePos, valueEnd - valuePos);

                if (!string.IsNullOrEmpty(name))
                {
                    result[name] = value;
                }

                if (namePos == -1)
                {
                    break;
                }
            }

            return result;
        }

        public static string QueryString(this string query, string param)
        {
            var result = new Dictionary<string, object>();

            string decoded = query;
            int decodedLength = decoded.Length;
            int namePos = 0;
            bool first = true;

            while (namePos <= decodedLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (int q = namePos; q < decodedLength; q++)
                {
                    if (valuePos == -1 && decoded[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (decoded[q] == '&' || decoded[q] == '#')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                if (first)
                {
                    first = false;
                    if (decoded[namePos] == '#')
                    {
                        namePos++;
                    }
                }

                string name, value;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    //name = UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1));
                    name = decoded.Substring(namePos, valuePos - namePos - 1);
                }

                if (valueEnd < 0)
                {
                    namePos = -1;
                    valueEnd = decoded.Length;
                }
                else
                {
                    namePos = valueEnd + 1;
                }

                //value = UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos));
                value = decoded.Substring(valuePos, valueEnd - valuePos);

                if (!string.IsNullOrEmpty(name))
                {
                    result[name] = value;
                }

                if (namePos == -1)
                {
                    break;
                }
            }

            return result[param].ToString();
        }

        public static string Then(this string input, string value)
        {
            return String.Concat(input, value);
        }

        public static string ToLower(this Enum type)
        {
            return type.ToString().ToLower();
        }

        public static string ToUpper(this Enum type)
        {
            return type.ToString().ToUpper();
        }

        public static DateTime FromNow(this TimeSpan value)
        {
            return new DateTime((DateTime.Now + value).Ticks);
        }

        public static DateTime FromUnixTime(this long seconds)
        {
            var time = new DateTime(1970, 1, 1);
            time = time.AddSeconds(seconds);

            return time.ToLocalTime();
        }

        public static long ToUnixTime(this DateTime dateTime)
        {
            var timeSpan = (dateTime - new DateTime(1970, 1, 1));
            var timestamp = (long)timeSpan.TotalSeconds;

            return timestamp;
        }

        public static byte[] GetBytes(this string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        public static string PercentEncode(this string s)
        {
            var bytes = s.GetBytes();
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                // [DC]: Support proper encoding of special characters (\n\r\t\b)
                if ((b > 7 && b < 11) || b == 13)
                {
                    sb.Append(string.Format("%0{0:X}", b));
                }
                else
                {
                    sb.Append(string.Format("%{0:X}", b));
                }
            }
            return sb.ToString();
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return String.Format(format, args);
        }

        public static string FormatWithInvariantCulture(this string format, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static bool IsNullOrBlank(this string value)
        {
            return String.IsNullOrEmpty(value) ||
                   (!String.IsNullOrEmpty(value) && value.Trim() == String.Empty);
        }

        public static bool EqualsIgnoreCase(this string left, string right)
        {
            return String.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool EqualsAny(this string input, params string[] args)
        {
            return args.Aggregate(false, (current, arg) => current | input.Equals(arg));
        }

        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            return new[] { item };
        }

        public static IEnumerable<T> And<T>(this T item, T other)
        {
            return new[] { item, other };
        }

        public static IEnumerable<T> And<T>(this IEnumerable<T> items, T item)
        {
            foreach (var i in items)
            {
                yield return i;
            }

            yield return item;
        }

        public static K TryWithKey<T, K>(this IDictionary<T, K> dictionary, T key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(K);
        }

        public static IEnumerable<T> ToEnumerable<T>(this object[] items) where T : class
        {
            foreach (var item in items)
            {
                var record = item as T;
                yield return record;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static void AddRange(this IDictionary<string, string> collection, NameValueCollection range)
        {
            foreach (string key in range.AllKeys)
            {
                collection.Add(key, range[key]);
            }
        }

        public static string ToQueryString(this NameValueCollection collection)
        {
            var sb = new StringBuilder();
            if (collection.Count > 0)
            {
                sb.Append("?");
            }

            var count = 0;
            foreach (var key in collection.AllKeys)
            {
                sb.AppendFormat("{0}={1}", key, collection[key].UrlEncode());
                count++;

                if (count >= collection.Count)
                {
                    continue;
                }
                sb.Append("&");
            }
            return sb.ToString();
        }

        public static string Concatenate(this WebParameterCollection collection, string separator, string spacer)
        {
            var sb = new StringBuilder();

            var total = collection.Count;
            var count = 0;

            foreach (var item in collection)
            {
                sb.Append(item.Name);
                sb.Append(separator);
                sb.Append(item.Value);

                count++;
                if (count < total)
                {
                    sb.Append(spacer);
                }
            }

            return sb.ToString();
        }

        public static string UrlEncode(this string value)
        {
            return Uri.EscapeDataString(value);
        }

        public static string UrlDecode(this string value)
        {
            return Uri.UnescapeDataString(value);
        }

        public static Uri AsUri(this string value)
        {
            return new Uri(value);
        }

        public static string ToRequestValue(this OAuthSignatureMethod signatureMethod)
        {
            var value = signatureMethod.ToString().ToUpper();
            var shaIndex = value.IndexOf("SHA1");
            return shaIndex > -1 ? value.Insert(shaIndex, "-") : value;
        }

        public static OAuthSignatureMethod FromRequestValue(this string signatureMethod)
        {
            switch (signatureMethod)
            {
                case "HMAC-SHA1":
                    return OAuthSignatureMethod.HmacSha1;
                case "RSA-SHA1":
                    return OAuthSignatureMethod.RsaSha1;
                default:
                    return OAuthSignatureMethod.PlainText;
            }
        }

        public static string HashWith(this string input, MacAlgorithmProvider hashProvider, string key)
        {
            IBuffer keyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            CryptographicKey cryptoKey = hashProvider.CreateKey(keyMaterial);
            IBuffer hash = CryptographicEngine.Sign(cryptoKey, CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToBase64String(hash);
        }

        public static Windows.UI.Color FromName(string colorName)
        {
            var colorProperty = typeof(Windows.UI.Colors).GetRuntimeProperty(colorName.UpperFirst());
            
            if (colorProperty == null) throw new ArgumentException("This is not a known color name.  Use a proper hex color number.");

            return (Windows.UI.Color)colorProperty.GetValue(null);
        }

        public static string UpperFirst(this string nameValue)
        {
            if (string.IsNullOrEmpty(nameValue)) return string.Empty;

            char[] chars = nameValue.ToCharArray();
            chars[0] = char.ToUpperInvariant(chars[0]);
            return new string(chars);
        }

        public static Windows.UI.Color ToColor(this string hexValue)
        {
            if (!hexValue.Contains("#"))
            { // may be a named color, attempt that
                var foundColor = FromName(hexValue);
                if (foundColor != null) return foundColor;
            }

            hexValue = hexValue.Replace("#", string.Empty);

            // some loose validation (not bullet-proof)
            if (hexValue.Length < 6)
            {
                throw new ArgumentException("This does not appear to be a proper hex color number");
            }

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int startPosition = 0;

            // the case where alpha is provided
            if (hexValue.Length == 8)
            {
                a = byte.Parse(hexValue.Substring(0, 2), NumberStyles.HexNumber);
                startPosition = 2;
            }

            r = byte.Parse(hexValue.Substring(startPosition, 2), NumberStyles.HexNumber);
            g = byte.Parse(hexValue.Substring(startPosition + 2, 2), NumberStyles.HexNumber);
            b = byte.Parse(hexValue.Substring(startPosition + 4, 2), NumberStyles.HexNumber);

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }
    }
}