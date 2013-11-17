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
using System.Globalization;
using System.Text;
using Windows.Foundation;

namespace Callisto.Controls
{
    /// <summary>
    /// Provides the URI of a map tile.
    /// </summary>
    public class TileSource
    {
        public const int TileSize = 256;
        public const double EarthRadius = 6378137d; // WGS 84 semi major axis

        private Func<int, int, int, Uri> _getUri;
        private string _uriFormat = string.Empty;

        public TileSource()
        {
            MetersPerDegree = EarthRadius * Math.PI / 180d;
        }

        public TileSource(string uriFormat)
            : this()
        {
            UriFormat = uriFormat;
        }

        public double MetersPerDegree { get; protected set; }

        public string UriFormat
        {
            get { return _uriFormat; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("The value of the UriFormat property must not be null or empty or white-space only.");
                }

                _uriFormat = value;

                if (_uriFormat.Contains("{x}") && _uriFormat.Contains("{y}") && _uriFormat.Contains("{z}"))
                {
                    if (_uriFormat.Contains("{c}"))
                    {
                        _getUri = GetOpenStreetMapUri;
                    }
                    else if (_uriFormat.Contains("{i}"))
                    {
                        _getUri = GetGoogleMapsUri;
                    }
                    else if (_uriFormat.Contains("{n}"))
                    {
                        _getUri = GetMapQuestUri;
                    }
                    else
                    {
                        _getUri = GetBasicUri;
                    }
                }
                else if (_uriFormat.Contains("{q}")) // {i} is optional
                {
                    _getUri = GetQuadKeyUri;
                }
                else if (_uriFormat.Contains("{W}") && _uriFormat.Contains("{S}") && _uriFormat.Contains("{E}") && _uriFormat.Contains("{N}"))
                {
                    _getUri = GetBoundingBoxUri;
                }
                else if (_uriFormat.Contains("{w}") && _uriFormat.Contains("{s}") && _uriFormat.Contains("{e}") && _uriFormat.Contains("{n}"))
                {
                    _getUri = GetLatLonBoundingBoxUri;
                }
                else if (_uriFormat.Contains("{x}") && _uriFormat.Contains("{v}") && _uriFormat.Contains("{z}"))
                {
                    _getUri = GetTmsUri;
                }
            }
        }

        public virtual Uri GetUri(int x, int y, int zoomLevel)
        {
            return _getUri != null ? _getUri(x, y, zoomLevel) : null;
        }

        private Uri GetBasicUri(int x, int y, int zoomLevel)
        {
            return new Uri(UriFormat.
                Replace("{x}", x.ToString()).
                Replace("{y}", y.ToString()).
                Replace("{z}", zoomLevel.ToString()));
        }

        private Uri GetOpenStreetMapUri(int x, int y, int zoomLevel)
        {
            var hostIndex = (x + y) % 3;

            return new Uri(UriFormat.
                Replace("{c}", "abc".Substring(hostIndex, 1)).
                Replace("{x}", x.ToString()).
                Replace("{y}", y.ToString()).
                Replace("{z}", zoomLevel.ToString()));
        }

        private Uri GetGoogleMapsUri(int x, int y, int zoomLevel)
        {
            var hostIndex = (x + y) % 4;

            return new Uri(UriFormat.
                Replace("{i}", hostIndex.ToString()).
                Replace("{x}", x.ToString()).
                Replace("{y}", y.ToString()).
                Replace("{z}", zoomLevel.ToString()));
        }

        private Uri GetMapQuestUri(int x, int y, int zoomLevel)
        {
            var hostIndex = (x + y) % 4 + 1;

            return new Uri(UriFormat.
                Replace("{n}", hostIndex.ToString()).
                Replace("{x}", x.ToString()).
                Replace("{y}", y.ToString()).
                Replace("{z}", zoomLevel.ToString()));
        }

        private Uri GetTmsUri(int x, int y, int zoomLevel)
        {
            y = (1 << zoomLevel) - 1 - y;

            return new Uri(UriFormat.
                Replace("{x}", x.ToString()).
                Replace("{v}", y.ToString()).
                Replace("{z}", zoomLevel.ToString()));
        }

        private Uri GetQuadKeyUri(int x, int y, int zoomLevel)
        {
            var key = new StringBuilder { Length = zoomLevel };

            for (var z = zoomLevel - 1; z >= 0; z--, x /= 2, y /= 2)
            {
                key[z] = (char)('0' + 2 * (y % 2) + (x % 2));
            }

            return new Uri(UriFormat.
                Replace("{i}", key.ToString(key.Length - 1, 1)).
                Replace("{q}", key.ToString()));
        }

        private Uri GetBoundingBoxUri(int x, int y, int zoomLevel)
        {
            var m = MetersPerDegree;
            var n = (double)(1 << zoomLevel);
            var x1 = m * (x * 360d / n - 180d);
            var x2 = m * ((x + 1) * 360d / n - 180d);
            var y1 = m * (180d - (y + 1) * 360d / n);
            var y2 = m * (180d - y * 360d / n);

            return new Uri(UriFormat.
                Replace("{W}", x1.ToString(CultureInfo.InvariantCulture)).
                Replace("{S}", y1.ToString(CultureInfo.InvariantCulture)).
                Replace("{E}", x2.ToString(CultureInfo.InvariantCulture)).
                Replace("{N}", y2.ToString(CultureInfo.InvariantCulture)));
        }

        private Uri GetLatLonBoundingBoxUri(int x, int y, int zoomLevel)
        {
            var t = new MercatorTransform();
            var n = (double)(1 << zoomLevel);
            var x1 = x * 360d / n - 180d;
            var x2 = (x + 1) * 360d / n - 180d;
            var y1 = 180d - (y + 1) * 360d / n;
            var y2 = 180d - y * 360d / n;
            var p1 = t.Transform(new Point(x1, y1));
            var p2 = t.Transform(new Point(x2, y2));

            return new Uri(UriFormat.
                Replace("{w}", p1.Longitude.ToString(CultureInfo.InvariantCulture)).
                Replace("{s}", p1.Latitude.ToString(CultureInfo.InvariantCulture)).
                Replace("{e}", p2.Longitude.ToString(CultureInfo.InvariantCulture)).
                Replace("{n}", p2.Latitude.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
