// Copyright (c) 2013 Tim Heuer
// Derivitive work from:
//      XAML Map Control - http://xamlmapcontrol.codeplex.com/
//      Copyright © Clemens Fischer 2012-2013
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
using System.Globalization;

namespace Callisto.Controls
{
    /// <summary>
    /// A geographic location with latitude and longitude values in degrees.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// TransformedLatitude is set by the Transform methods in MercatorTransform.
        /// It holds the transformed latitude value to avoid redundant recalculation. 
        /// </summary>
        internal double TransformedLatitude;

        private double _latitude;
        private double _longitude;

        public Location()
        {
        }

        public Location(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = Math.Min(Math.Max(value, -90d), 90d);
                TransformedLatitude = double.NaN;
            }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:F5},{1:F5}", _latitude, _longitude);
        }

        public static Location Parse(string s)
        {
            var tokens = s.Split(new[] { ',' });
            if (tokens.Length != 2)
            {
                throw new FormatException("Location string must be a comma-separated pair of double values");
            }

            return new Location(
                double.Parse(tokens[0], NumberStyles.Float, CultureInfo.InvariantCulture),
                double.Parse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        public static double NormalizeLongitude(double longitude)
        {
            return (longitude >= -180d && longitude <= 180d) ? longitude : ((longitude + 180d) % 360d + 360d) % 360d - 180d;
        }
    }
}