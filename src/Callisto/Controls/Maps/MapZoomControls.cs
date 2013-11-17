// Copyright (c) 2013 Tim Heuer
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
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Callisto.Controls.Common;

namespace Callisto.Controls
{
    [TemplatePart(Name = ZOOMIN_PARTNAME, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ZOOMOUT_PARTNAME, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = CURRENTLOCATION_PARTNAME, Type = typeof(FrameworkElement))]
    public class MapZoomControls : Control
    {
        private AppBarButton _zoomInButton;
        private AppBarButton _zoomOutButton;
        private AppBarButton _currentLocationButton;
        private MapBase _hostedMap;

        private const string ZOOMIN_PARTNAME = "ZoomInButton";
        private const string ZOOMOUT_PARTNAME = "ZoomOutButton";
        private const string CURRENTLOCATION_PARTNAME = "CurrentLocationButton";

        public MapZoomControls()
        {
            DefaultStyleKey = typeof(MapZoomControls);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ResourceLoader rl = new ResourceLoader("Callisto/Resources");

            _hostedMap = this.Ancestors<MapBase>().First() as MapBase;

            _zoomInButton = GetTemplateChild(ZOOMIN_PARTNAME) as AppBarButton;
            _zoomOutButton = GetTemplateChild(ZOOMOUT_PARTNAME) as AppBarButton;
            _currentLocationButton = GetTemplateChild(CURRENTLOCATION_PARTNAME) as AppBarButton;

            if (_currentLocationButton != null)
            {
                var currentLocationText = rl.GetString("CurrentLocation");
                _currentLocationButton.SetValue(ToolTipService.ToolTipProperty, currentLocationText);
                _currentLocationButton.SetValue(AutomationProperties.NameProperty, currentLocationText);
                _currentLocationButton.Click += OnCurrentLocationClicked;

            }

            if (_zoomInButton != null)
            {
                var zoomInText = rl.GetString("ZoomIn");
                _zoomInButton.SetValue(AutomationProperties.NameProperty, zoomInText);
                _zoomInButton.SetValue(ToolTipService.ToolTipProperty, zoomInText);
                _zoomInButton.Click += OnZoomInClicked;
            }
            if (_zoomOutButton != null)
            {
                var zoomOutText = rl.GetString("ZoomOut");
                _zoomOutButton.SetValue(ToolTipService.ToolTipProperty, zoomOutText);
                _zoomOutButton.SetValue(AutomationProperties.NameProperty, zoomOutText);
                _zoomOutButton.Click += OnZoomOutClicked;
            }
        }

        async private void OnCurrentLocationClicked(object sender, RoutedEventArgs e)
        {
            var gl = new Geolocator();
            var location = await gl.GetGeopositionAsync();
            _hostedMap.Center = new Location(location.Coordinate.Latitude, location.Coordinate.Longitude);
            _hostedMap.ZoomLevel = (_hostedMap.MaxZoomLevel < 11) ? _hostedMap.MaxZoomLevel : 11;
        }

        private void OnZoomOutClicked(object sender, RoutedEventArgs e)
        {
            _hostedMap.ZoomMap(_hostedMap.LocationToViewportPoint(_hostedMap.Center), _hostedMap.ZoomLevel - 1);
        }

        private void OnZoomInClicked(object sender, RoutedEventArgs e)
        {
            _hostedMap.ZoomMap(_hostedMap.LocationToViewportPoint(_hostedMap.Center), _hostedMap.ZoomLevel + 1);
        }
    }
}
