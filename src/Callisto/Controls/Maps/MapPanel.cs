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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    internal interface IMapElement
    {
        MapBase ParentMap { get; set; }
    }

    /// <summary>
    /// Positions child elements on a Map, at a position specified by the attached property Location.
    /// The Location is transformed into a viewport position by the MapBase.LocationToViewportPoint
    /// method and applied to a child element's RenderTransform as an appropriate TranslateTransform.
    /// </summary>
    public class MapPanel : Panel, IMapElement
    {
        public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached(
            "Location", typeof(Location), typeof(MapPanel), new PropertyMetadata(null, LocationPropertyChanged));

        public static readonly DependencyProperty ViewportPositionProperty = DependencyProperty.RegisterAttached(
            "ViewportPosition", typeof(Point?), typeof(MapPanel), null);

        public static Location GetLocation(UIElement element)
        {
            return (Location)element.GetValue(LocationProperty);
        }

        public static void SetLocation(UIElement element, Location value)
        {
            element.SetValue(LocationProperty, value);
        }

        public static Point? GetViewportPosition(UIElement element)
        {
            return (Point?)element.GetValue(ViewportPositionProperty);
        }

        private MapBase _parentMap;

        public MapBase ParentMap
        {
            get { return _parentMap; }
            set
            {
                if (_parentMap != null && _parentMap != this)
                {
                    _parentMap.ViewportChanged -= OnViewportChanged;
                }

                _parentMap = value;

                if (_parentMap != null && _parentMap != this)
                {
                    _parentMap.ViewportChanged += OnViewportChanged;
                    OnViewportChanged();
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in InternalChildren)
            {
                element.Measure(availableSize);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in InternalChildren)
            {
                var location = GetLocation(element);

                ArrangeElement(element, finalSize, location != null);

                if (location != null)
                {
                    SetViewportPosition(element, _parentMap, location);
                }
            }

            return finalSize;
        }

        protected virtual void OnViewportChanged()
        {
            foreach (UIElement element in InternalChildren)
            {
                var location = GetLocation(element);

                if (location != null)
                {
                    SetViewportPosition(element, _parentMap, location);
                }
            }
        }

        private void OnViewportChanged(object sender, EventArgs e)
        {
            OnViewportChanged();
        }

        private static void ParentMapPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var mapElement = obj as IMapElement;

            if (mapElement != null)
            {
                mapElement.ParentMap = e.NewValue as MapBase;
            }
        }

        private static void LocationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var element = obj as UIElement;

            if (element != null)
            {
                var mapElement = element as IMapElement;
                var parentMap = mapElement != null ? mapElement.ParentMap : GetParentMap(element);
                var location = e.NewValue as Location;

                if ((location != null) != (e.OldValue != null))
                {
                    ArrangeElement(element, null, location != null);
                }

                SetViewportPosition(element, parentMap, location);
            }
        }

        private static void SetViewportPosition(UIElement element, MapBase parentMap, Location location)
        {
            Point viewportPosition;

            if (parentMap != null && location != null)
            {
                viewportPosition = parentMap.LocationToViewportPoint(location);
                element.SetValue(ViewportPositionProperty, viewportPosition);
            }
            else
            {
                viewportPosition = new Point();
                element.ClearValue(ViewportPositionProperty);
            }

            var translateTransform = element.RenderTransform as TranslateTransform;

            if (translateTransform == null)
            {
                var transformGroup = element.RenderTransform as TransformGroup;

                if (transformGroup == null)
                {
                    translateTransform = new TranslateTransform();
                    element.RenderTransform = translateTransform;
                }
                else
                {
                    if (transformGroup.Children.Count > 0)
                    {
                        translateTransform = transformGroup.Children[transformGroup.Children.Count - 1] as TranslateTransform;
                    }

                    if (translateTransform == null)
                    {
                        translateTransform = new TranslateTransform();
                        transformGroup.Children.Add(translateTransform);
                    }
                }
            }

            translateTransform.X = viewportPosition.X;
            translateTransform.Y = viewportPosition.Y;
        }

        private static void ArrangeElement(UIElement element, Size? panelSize, bool hasLocation)
        {
            var rect = new Rect(0d, 0d, element.DesiredSize.Width, element.DesiredSize.Height);
            var frameworkElement = element as FrameworkElement;

            if (frameworkElement != null)
            {
                if (hasLocation)
                {
                    switch (frameworkElement.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            rect.X = -rect.Width / 2d;
                            break;

                        case HorizontalAlignment.Right:
                            rect.X = -rect.Width;
                            break;
                    }

                    switch (frameworkElement.VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            rect.Y = -rect.Height / 2d;
                            break;

                        case VerticalAlignment.Bottom:
                            rect.Y = -rect.Height;
                            break;
                    }
                }
                else
                {
                    if (!panelSize.HasValue)
                    {
                        var panel = frameworkElement.Parent as Panel;
                        panelSize = panel != null ? panel.RenderSize : new Size();
                    }

                    switch (frameworkElement.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Center:
                            rect.X = (panelSize.Value.Width - rect.Width) / 2d;
                            break;

                        case HorizontalAlignment.Right:
                            rect.X = panelSize.Value.Width - rect.Width;
                            break;

                        case HorizontalAlignment.Stretch:
                            rect.Width = panelSize.Value.Width;
                            break;
                    }

                    switch (frameworkElement.VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            rect.Y = (panelSize.Value.Height - rect.Height) / 2d;
                            break;

                        case VerticalAlignment.Bottom:
                            rect.Y = panelSize.Value.Height - rect.Height;
                            break;

                        case VerticalAlignment.Stretch:
                            rect.Height = panelSize.Value.Height;
                            break;
                    }
                }
            }

            element.Arrange(rect);
        }

        public static readonly DependencyProperty ParentMapProperty = DependencyProperty.RegisterAttached(
            "ParentMap", typeof(MapBase), typeof(MapPanel), new PropertyMetadata(null, ParentMapPropertyChanged));

        public MapPanel()
        {
            if (!(this is MapBase))
            {
                AddParentMapHandlers(this);
            }
        }

        private UIElementCollection InternalChildren
        {
            get { return Children; }
        }

        /// <summary>
        /// Helper method to work around missing property value inheritance in Silverlight and WinRT.
        /// Adds Loaded and Unloaded handlers to the specified FrameworkElement, which set and clear
        /// the value of the MapPanel.ParentMap attached property.
        /// </summary>
        public static void AddParentMapHandlers(FrameworkElement element)
        {
            element.Loaded += (o, e) => GetParentMap(element);
            element.Unloaded += (o, e) => element.ClearValue(ParentMapProperty);
        }

        public static MapBase GetParentMap(UIElement element)
        {
            var parentMap = (MapBase)element.GetValue(ParentMapProperty);

            if (parentMap == null && (parentMap = FindParentMap(element)) != null)
            {
                element.SetValue(ParentMapProperty, parentMap);
            }

            return parentMap;
        }

        private static MapBase FindParentMap(UIElement element)
        {
            MapBase parentMap = null;
            var parentElement = VisualTreeHelper.GetParent(element) as UIElement;

            if (parentElement != null)
            {
                parentMap = parentElement as MapBase;

                if (parentMap == null)
                {
                    parentMap = GetParentMap(parentElement);
                }
            }

            return parentMap;
        }

        internal void SetParentMap()
        {
            SetValue(ParentMapProperty, this);
        }
    }
}
