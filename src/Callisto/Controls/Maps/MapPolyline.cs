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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    /// <summary>
    /// A polyline or polygon created from a collection of Locations.
    /// </summary>
    public class MapPolyline : MapPath
    {
        public MapPolyline()
        {
            Data = new PathGeometry();
        }

        protected override void UpdateData()
        {
            var geometry = (PathGeometry)Data;
            var locations = Locations;
            Location first;

            if (ParentMap != null && locations != null && (first = locations.FirstOrDefault()) != null)
            {
                var figure = new PathFigure
                {
                    StartPoint = ParentMap.MapTransform.Transform(first),
                    IsClosed = IsClosed,
                    IsFilled = IsClosed
                };

                var segment = new PolyLineSegment();

                foreach (var location in locations.Skip(1))
                {
                    segment.Points.Add(ParentMap.MapTransform.Transform(location));
                }

                if (segment.Points.Count > 0)
                {
                    figure.Segments.Add(segment);
                }

                geometry.Figures.Clear();
                geometry.Figures.Add(figure);
                geometry.Transform = ParentMap.ViewportTransform;
            }
            else
            {
                geometry.Figures.Clear();
                geometry.ClearValue(Geometry.TransformProperty);
            }
        }

        // For WinRT, the Locations dependency property type is declared as object
        // instead of IEnumerable. See http://stackoverflow.com/q/10544084/1136211
        private static readonly Type LocationsPropertyType = typeof(object);

        public static readonly DependencyProperty LocationsProperty = DependencyProperty.Register(
            "Locations", LocationsPropertyType, typeof(MapPolyline),
            new PropertyMetadata(null, LocationsPropertyChanged));

        public static readonly DependencyProperty IsClosedProperty = DependencyProperty.Register(
            "IsClosed", typeof(bool), typeof(MapPolyline),
            new PropertyMetadata(false, (o, e) => ((MapPolyline)o).UpdateData()));

        /// <summary>
        /// Gets or sets the locations that define the polyline points.
        /// </summary>
        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the polyline is closed, i.e. is a polygon.
        /// </summary>
        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }

        private void LocationCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateData();
        }

        private static void LocationsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var mapPolyline = (MapPolyline)obj;
            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;

            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= mapPolyline.LocationCollectionChanged;
            }

            if (newCollection != null)
            {
                newCollection.CollectionChanged += mapPolyline.LocationCollectionChanged;
            }

            mapPolyline.UpdateData();
        }
    }
}
