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

using System.Collections.ObjectModel;
using System.Linq;

namespace Callisto.Controls
{
    public class TileLayerCollection : ObservableCollection<TileLayer>
    {
        public TileLayer this[string sourceName]
        {
            get { return this.FirstOrDefault(t => t.SourceName == sourceName); }
        }
    }
}
