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
using Windows.UI.Xaml.Shapes;

namespace Callisto.Controls
{
    /// <summary>
    /// Base class for map shapes.
    /// </summary>
    public class MapPath : Path, IMapElement
    {
        private MapBase _parentMap;

        public MapPath()
        {
            MapPanel.AddParentMapHandlers(this);
        }

        public MapBase ParentMap
        {
            get { return _parentMap; }
            set
            {
                _parentMap = value;
                UpdateData();
            }
        }

        protected virtual void UpdateData()
        {
        }

        protected override Size MeasureOverride(Size constraint)
        {
            // base.MeasureOverride in WPF and WinRT sometimes return a Size with zero
            // width or height, whereas base.MeasureOverride in Silverlight occasionally
            // throws an ArgumentException, as it tries to create a Size from a negative
            // width or height, apparently resulting from a transformed Geometry.
            // In either case it seems to be sufficient to simply return a non-zero size.
            return new Size(1, 1);
        }
    }
}
