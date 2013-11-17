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
using Windows.Foundation;

namespace Callisto.Controls
{
    /// <summary>
    /// Defines a normal cylindrical projection. Latitude and longitude values in degrees are
    /// transformed to cartesian coordinates with origin at latitude = 0 and longitude = 0.
    /// Longitude values are transformed identically to x values in the interval [-180 .. 180].
    /// Latitude values in the interval [-MaxLatitude .. MaxLatitude] are transformed to y values in
    /// the interval [-180 .. 180] according to the actual projection, e.g. the Mercator projection.
    /// </summary>
    public abstract class MapTransform
    {
        /// <summary>
        /// Gets the absolute value of the minimum and maximum latitude that can be transformed.
        /// </summary>
        public abstract double MaxLatitude { get; }

        /// <summary>
        /// Gets the scale factor of the map projection at the specified geographic location
        /// relative to the scale at latitude zero.
        /// </summary>
        public abstract double RelativeScale(Location location);

        /// <summary>
        /// Transforms a geographic location to a cartesian coordinate point.
        /// </summary>
        public abstract Point Transform(Location location);

        /// <summary>
        /// Transforms a cartesian coordinate point to a geographic location.
        /// </summary>
        public abstract Location Transform(Point point);
    }
}
