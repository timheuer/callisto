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
//
using System;
using Windows.Foundation;

namespace Callisto.Controls
{
    /// <summary>
    /// Transforms latitude and longitude values in degrees to cartesian coordinates
    /// according to the Mercator projection.
    /// </summary>
    public class MercatorTransform : MapTransform
    {
        private static readonly double maxLatitude = Math.Atan(Math.Sinh(Math.PI)) / Math.PI * 180d;

        public override double MaxLatitude
        {
            get { return maxLatitude; }
        }

        public override double RelativeScale(Location location)
        {
            if (location.Latitude <= -90d)
            {
                return double.NegativeInfinity;
            }

            if (location.Latitude >= 90d)
            {
                return double.PositiveInfinity;
            }

            return 1d / Math.Cos(location.Latitude * Math.PI / 180d);
        }

        public override Point Transform(Location location)
        {
            if (double.IsNaN(location.TransformedLatitude))
            {
                if (location.Latitude <= -90d)
                {
                    location.TransformedLatitude = double.NegativeInfinity;
                }
                else if (location.Latitude >= 90d)
                {
                    location.TransformedLatitude = double.PositiveInfinity;
                }
                else
                {
                    var lat = location.Latitude * Math.PI / 180d;
                    location.TransformedLatitude = Math.Log(Math.Tan(lat) + 1d / Math.Cos(lat)) / Math.PI * 180d;
                }
            }

            return new Point(location.Longitude, location.TransformedLatitude);
        }

        public override Location Transform(Point point)
        {
            var location = new Location(Math.Atan(Math.Sinh(point.Y * Math.PI / 180d)) / Math.PI * 180d, point.X);
            location.TransformedLatitude = point.Y;
            return location;
        }
    }
}
