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

using Windows.ApplicationModel.Store;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Callisto.Controls
{
    /// <summary>
    /// Default input event handling.
    /// </summary>
    public class Map : MapBase
    {
        public static readonly DependencyProperty MouseWheelZoomChangeProperty = DependencyProperty.Register(
            "MouseWheelZoomChange", typeof(double), typeof(Map), new PropertyMetadata(1d));

        private Point? _mousePosition;

        public Map()
        {
            ManipulationMode = ManipulationModes.Scale | ManipulationModes.ScaleInertia |
                ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;

            ManipulationDelta += OnManipulationDelta;
            PointerWheelChanged += OnPointerWheelChanged;
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerCanceled += OnPointerReleased;
            PointerCaptureLost += OnPointerReleased;
            PointerMoved += OnPointerMoved;
            DoubleTapped += OnDoubleTapped;
        }

        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var newPoint = e.GetPosition(this);
            ZoomMap(newPoint, ZoomLevel+1);
            // TODO: Need a ZoomAndCenterMap function
        }

        /// <summary>
        /// Gets or sets the amount by which the ZoomLevel property changes during a MouseWheel event.
        /// </summary>
        public double MouseWheelZoomChange
        {
            get { return (double)GetValue(MouseWheelZoomChangeProperty); }
            set { SetValue(MouseWheelZoomChangeProperty, value); }
        }

        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            var zoomChange = MouseWheelZoomChange * point.Properties.MouseWheelDelta / 120d;
            ZoomMap(point.Position, TargetZoomLevel + zoomChange);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                CapturePointer(e.Pointer))
            {
                _mousePosition = e.GetCurrentPoint(this).Position;
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _mousePosition = null;
                ReleasePointerCapture(e.Pointer);
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_mousePosition.HasValue)
            {
                var position = e.GetCurrentPoint(this).Position;
                TranslateMap(new Point(position.X - _mousePosition.Value.X, position.Y - _mousePosition.Value.Y));
                _mousePosition = position;
            }
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Mouse)
            {
                TransformMap(e.Position, e.Delta.Translation, e.Delta.Rotation, e.Delta.Scale);
            }
        }
    }
}