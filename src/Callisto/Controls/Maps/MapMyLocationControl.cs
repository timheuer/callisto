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
    [TemplatePart(Name = CURRENTLOCATION_PARTNAME, Type = typeof(FrameworkElement))]
    public class MapMyLocationControl : Control
    {
        private AppBarButton _currentLocationButton;
        private MapBase _hostedMap;

        private const string CURRENTLOCATION_PARTNAME = "CurrentLocationButton";

        public MapMyLocationControl()
        {
            DefaultStyleKey = typeof(MapMyLocationControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ResourceLoader rl = new ResourceLoader("Callisto/Resources");

            _hostedMap = this.Ancestors<MapBase>().First() as MapBase;

            _currentLocationButton = GetTemplateChild(CURRENTLOCATION_PARTNAME) as AppBarButton;

            if (_currentLocationButton != null)
            {
                var currentLocationText = rl.GetString("CurrentLocation");
                _currentLocationButton.SetValue(ToolTipService.ToolTipProperty, currentLocationText);
                _currentLocationButton.SetValue(AutomationProperties.NameProperty, currentLocationText);
                _currentLocationButton.Click += OnCurrentLocationClicked;
            }
        }

        async private void OnCurrentLocationClicked(object sender, RoutedEventArgs e)
        {
            var gl = new Geolocator();
            var location = await gl.GetGeopositionAsync();
            _hostedMap.Center = new Location(location.Coordinate.Latitude, location.Coordinate.Longitude);
            _hostedMap.ZoomLevel = (_hostedMap.MaxZoomLevel < 11) ? _hostedMap.MaxZoomLevel : 11;
        }
    }
}
