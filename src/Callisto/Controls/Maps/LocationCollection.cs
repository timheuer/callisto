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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Callisto.Controls
{
    /// <summary>
    /// A collection of geographic locations.
    /// </summary>
    public class LocationCollection : ObservableCollection<Location>
    {
        public LocationCollection()
        {
        }

        public LocationCollection(IEnumerable<Location> locations)
            : base(locations)
        {
        }

        public LocationCollection(List<Location> locations)
            : base(locations)
        {
        }

        public static LocationCollection Parse(string s)
        {
            var strings = s.Split(separator: new[] { ' ', ';' }, options: StringSplitOptions.RemoveEmptyEntries);

            return new LocationCollection(strings.Select(l => Location.Parse(l)));
        }
    }
}
