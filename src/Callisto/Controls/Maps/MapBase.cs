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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Callisto.Controls.Common;

namespace Callisto.Controls
{
    /// <summary>
    /// The map control. Draws map content provided by the TileLayers or the TileLayer property.
    /// The visible map area is defined by the Center and ZoomLevel properties. The map can be rotated
    /// by an angle that is given by the Heading property.
    /// MapBase is a MapPanel and hence can contain map overlays like other MapPanels or MapItemsControls.
    /// </summary>
    public class MapBase : MapPanel
    {
        public static TimeSpan AnimationDuration = TimeSpan.FromSeconds(0.5);
        public static EasingFunctionBase AnimationEasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(MapBase), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        private TextBlock _attributionBlock;
        private StackPanel _controlsHost;
        private MapZoomControls _zoomControls;
        private MapMyLocationControl _myLocation;

        internal void Initialize()
        {
            Background = new SolidColorBrush(Colors.Transparent);
            Clip = new RectangleGeometry();
            Children.Add(_tileContainer);

            SetupTileAttribution();
            SetupScale();
            SetupZoomControls();

            SizeChanged += OnRenderSizeChanged;
        }

        private void SetupZoomControls()
        {
            _controlsHost = new StackPanel
            {
                RequestedTheme = ElementTheme.Light,
                HorizontalAlignment =
                    (FlowDirection == FlowDirection.LeftToRight) ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Children.Add(_controlsHost);

            if (ZoomControlsEnabled)
            {
                _zoomControls = new MapZoomControls();
                _controlsHost.Children.Insert(0, _zoomControls);
            }

            if (CurrentLocationEnabled)
            {
                _myLocation = new MapMyLocationControl();
                _controlsHost.Children.Add(_myLocation);
            }
        }

        private MapScale _scale;

        private void SetupScale()
        {
            _scale = new MapScale { MinWidth = 50 };
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                _scale.HorizontalAlignment = HorizontalAlignment.Right;
            }
            Children.Add(_scale);
        }

        private void SetupTileAttribution()
        {
            _attributionBlock = new TextBlock
                {
                    FontSize = 10,
                    Margin = new Thickness(10, 2, 10, 2)
                };

            _attributionBlock.SetValue(AutomationProperties.NameProperty, "Map source attribution");

            Border attributionBorder = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                Opacity = 0.8,
                HorizontalAlignment =
                    (FlowDirection == FlowDirection.LeftToRight)
                        ? HorizontalAlignment.Right
                        : HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Child = _attributionBlock
            };


            Children.Add(attributionBorder);
        }

        private void OnRenderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Clip != null) Clip.Rect = new Rect(0d, 0d, RenderSize.Width, RenderSize.Height);
            ResetTransformOrigin();
            UpdateTransform();
        }

        private void SetTransformMatrixes(double scale)
        {
            _scaleTransform.Matrix = new Matrix(scale, 0d, 0d, scale, 0d, 0d);
            _rotateTransform.Matrix = Matrix.Identity.Rotate(Heading);
            _scaleRotateTransform.Matrix = _scaleTransform.Matrix.Multiply(_rotateTransform.Matrix);
        }

        public static readonly DependencyProperty TileLayersProperty = DependencyProperty.Register(
            "TileLayers", typeof(TileLayerCollection), typeof(MapBase), new PropertyMetadata(null,
                (o, e) => ((MapBase)o).TileLayersPropertyChanged((TileLayerCollection)e.OldValue, (TileLayerCollection)e.NewValue)));

        public static readonly DependencyProperty TileLayerProperty = DependencyProperty.Register(
            "TileLayer", typeof(TileLayer), typeof(MapBase), new PropertyMetadata(null,
                (o, e) => ((MapBase)o).TileLayerPropertyChanged((TileLayer)e.NewValue)));

        public static readonly DependencyProperty TileOpacityProperty = DependencyProperty.Register(
            "TileOpacity", typeof(double), typeof(MapBase), new PropertyMetadata(1d,
                (o, e) => ((MapBase)o)._tileContainer.Opacity = (double)e.NewValue));

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Location), typeof(MapBase), new PropertyMetadata(new Location(),
                (o, e) => ((MapBase)o).CenterPropertyChanged((Location)e.NewValue)));

        public static readonly DependencyProperty TargetCenterProperty = DependencyProperty.Register(
            "TargetCenter", typeof(Location), typeof(MapBase), new PropertyMetadata(new Location(),
                (o, e) => ((MapBase)o).TargetCenterPropertyChanged((Location)e.NewValue)));

        internal static readonly DependencyProperty CenterPointProperty = DependencyProperty.Register(
            "CenterPoint", typeof(Point), typeof(MapBase), new PropertyMetadata(new Point(),
                (o, e) => ((MapBase)o).CenterPointPropertyChanged((Point)e.NewValue)));

        public static readonly DependencyProperty MinZoomLevelProperty = DependencyProperty.Register(
            "MinZoomLevel", typeof(double), typeof(MapBase), new PropertyMetadata(1d,
                (o, e) => ((MapBase)o).MinZoomLevelPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty MaxZoomLevelProperty = DependencyProperty.Register(
            "MaxZoomLevel", typeof(double), typeof(MapBase), new PropertyMetadata(18d,
                (o, e) => ((MapBase)o).MaxZoomLevelPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register(
            "ZoomLevel", typeof(double), typeof(MapBase), new PropertyMetadata(1d,
                (o, e) => ((MapBase)o).ZoomLevelPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty TargetZoomLevelProperty = DependencyProperty.Register(
            "TargetZoomLevel", typeof(double), typeof(MapBase), new PropertyMetadata(1d,
                (o, e) => ((MapBase)o).TargetZoomLevelPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
            "Heading", typeof(double), typeof(MapBase), new PropertyMetadata(0d,
                (o, e) => ((MapBase)o).HeadingPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty TargetHeadingProperty = DependencyProperty.Register(
            "TargetHeading", typeof(double), typeof(MapBase), new PropertyMetadata(0d,
                (o, e) => ((MapBase)o).TargetHeadingPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty CenterScaleProperty = DependencyProperty.Register(
            "CenterScale", typeof(double), typeof(MapBase), null);

        private readonly TileContainer _tileContainer = new TileContainer();
        private readonly MapTransform _mapTransform = new MercatorTransform();
        private readonly MatrixTransform _scaleTransform = new MatrixTransform();
        private readonly MatrixTransform _rotateTransform = new MatrixTransform();
        private readonly MatrixTransform _scaleRotateTransform = new MatrixTransform();
        private Location _transformOrigin;
        private Point _viewportOrigin;
        private PointAnimation _centerAnimation;
        private DoubleAnimation _zoomLevelAnimation;
        private DoubleAnimation _headingAnimation;
        private Brush _storedBackground;
        private Brush _storedForeground;
        private bool _internalPropertyChange;

        public MapBase()
        {
            SetParentMap();

            TileLayers = new TileLayerCollection();
            Initialize();

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Raised when the current viewport has changed.
        /// </summary>
        public event EventHandler ViewportChanged;

        /// <summary>
        /// Gets or sets the map foreground Brush.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileLayers used by this Map.
        /// </summary>
        public TileLayerCollection TileLayers
        {
            get { return (TileLayerCollection)GetValue(TileLayersProperty); }
            set { SetValue(TileLayersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the base TileLayer used by this Map, i.e. TileLayers[0].
        /// </summary>
        public TileLayer TileLayer
        {
            get { return (TileLayer)GetValue(TileLayerProperty); }
            set { SetValue(TileLayerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the opacity of the tile layers.
        /// </summary>
        public double TileOpacity
        {
            get { return (double)GetValue(TileOpacityProperty); }
            set { SetValue(TileOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the location of the center point of the Map.
        /// </summary>
        public Location Center
        {
            get { return (Location)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target value of a Center animation.
        /// </summary>
        public Location TargetCenter
        {
            get { return (Location)GetValue(TargetCenterProperty); }
            set { SetValue(TargetCenterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum value of the ZoomLevel and TargetZommLevel properties.
        /// Must be greater than or equal to zero and less than or equal to MaxZoomLevel.
        /// </summary>
        public double MinZoomLevel
        {
            get { return (double)GetValue(MinZoomLevelProperty); }
            set { SetValue(MinZoomLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum value of the ZoomLevel and TargetZommLevel properties.
        /// Must be greater than or equal to MinZoomLevel and less than or equal to 20.
        /// </summary>
        public double MaxZoomLevel
        {
            get { return (double)GetValue(MaxZoomLevelProperty); }
            set { SetValue(MaxZoomLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map zoom level.
        /// </summary>
        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target value of a ZoomLevel animation.
        /// </summary>
        public double TargetZoomLevel
        {
            get { return (double)GetValue(TargetZoomLevelProperty); }
            set { SetValue(TargetZoomLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the map heading, or clockwise rotation angle in degrees.
        /// </summary>
        public double Heading
        {
            get { return (double)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target value of a Heading animation.
        /// </summary>
        public double TargetHeading
        {
            get { return (double)GetValue(TargetHeadingProperty); }
            set { SetValue(TargetHeadingProperty, value); }
        }

        /// <summary>
        /// Gets the map scale at the Center location as viewport coordinate units (pixels) per meter.
        /// </summary>
        public double CenterScale
        {
            get { return (double)GetValue(CenterScaleProperty); }
            private set { SetValue(CenterScaleProperty, value); }
        }

        /// <summary>
        /// Gets the transformation from geographic coordinates to cartesian map coordinates.
        /// </summary>
        public MapTransform MapTransform
        {
            get { return _mapTransform; }
        }

        /// <summary>
        /// Gets the transformation from cartesian map coordinates to viewport coordinates.
        /// </summary>
        public Transform ViewportTransform
        {
            get { return _tileContainer.ViewportTransform; }
        }

        /// <summary>
        /// Gets the scaling transformation from meters to viewport coordinate units (pixels)
        /// at the viewport center point.
        /// </summary>
        public Transform ScaleTransform
        {
            get { return _scaleTransform; }
        }

        /// <summary>
        /// Gets the transformation that rotates by the value of the Heading property.
        /// </summary>
        public Transform RotateTransform
        {
            get { return _rotateTransform; }
        }

        /// <summary>
        /// Gets the combination of ScaleTransform and RotateTransform
        /// </summary>
        public Transform ScaleRotateTransform
        {
            get { return _scaleRotateTransform; }
        }

        /// <summary>
        /// Gets the conversion factor from longitude degrees to meters, at latitude = 0.
        /// </summary>
        public double MetersPerDegree
        {
            get
            {
                return (TileLayer != null && TileLayer.TileSource != null) ?
                    TileLayer.TileSource.MetersPerDegree : (TileSource.EarthRadius * Math.PI / 180d);
            }
        }

        /// <summary>
        /// Gets the map scale at the specified location as viewport coordinate units (pixels) per meter.
        /// </summary>
        public double GetMapScale(Location location)
        {
            return _mapTransform.RelativeScale(location) * Math.Pow(2d, ZoomLevel) * TileSource.TileSize / (MetersPerDegree * 360d);
        }

        /// <summary>
        /// Transforms a geographic location to a viewport coordinates point.
        /// </summary>
        public Point LocationToViewportPoint(Location location)
        {
            return ViewportTransform.Transform(_mapTransform.Transform(location));
        }

        /// <summary>
        /// Transforms a viewport coordinates point to a geographic location.
        /// </summary>
        public Location ViewportPointToLocation(Point point)
        {
            return _mapTransform.Transform(ViewportTransform.Inverse.Transform(point));
        }

        /// <summary>
        /// Sets a temporary origin location in geographic coordinates for scaling and rotation transformations.
        /// This origin location is automatically removed when the Center property is set by application code.
        /// </summary>
        public void SetTransformOrigin(Location origin)
        {
            _transformOrigin = origin;
            _viewportOrigin = LocationToViewportPoint(origin);
        }

        /// <summary>
        /// Sets a temporary origin point in viewport coordinates for scaling and rotation transformations.
        /// This origin point is automatically removed when the Center property is set by application code.
        /// </summary>
        public void SetTransformOrigin(Point origin)
        {
            _viewportOrigin.X = Math.Min(Math.Max(origin.X, 0d), RenderSize.Width);
            _viewportOrigin.Y = Math.Min(Math.Max(origin.Y, 0d), RenderSize.Height);
            _transformOrigin = ViewportPointToLocation(_viewportOrigin);
        }

        /// <summary>
        /// Removes the temporary transform origin point set by SetTransformOrigin.
        /// </summary>
        public void ResetTransformOrigin()
        {
            _transformOrigin = null;
            _viewportOrigin = new Point(RenderSize.Width / 2d, RenderSize.Height / 2d);
        }

        /// <summary>
        /// Changes the Center property according to the specified translation in viewport coordinates.
        /// </summary>
        public void TranslateMap(Point translation)
        {
            if (translation.X != 0d || translation.Y != 0d)
            {
                if (_transformOrigin != null)
                {
                    _viewportOrigin.X += translation.X;
                    _viewportOrigin.Y += translation.Y;
                    UpdateTransform();
                }
                else
                {
                    Center = ViewportPointToLocation(new Point(_viewportOrigin.X - translation.X, _viewportOrigin.Y - translation.Y));
                }
            }
        }

        /// <summary>
        /// Changes the Center, Heading and ZoomLevel properties according to the specified
        /// viewport coordinate translation, rotation and scale delta values. Rotation and scaling
        /// is performed relative to the specified origin point in viewport coordinates.
        /// </summary>
        public void TransformMap(Point origin, Point translation, double rotation, double scale)
        {
            SetTransformOrigin(origin);

            _viewportOrigin.X += translation.X;
            _viewportOrigin.Y += translation.Y;

            if (rotation != 0d)
            {
                var heading = (((Heading + rotation) % 360d) + 360d) % 360d;
                InternalSetValue(HeadingProperty, heading);
                InternalSetValue(TargetHeadingProperty, heading);
            }

            if (scale != 1d)
            {
                var zoomLevel = Math.Min(Math.Max(ZoomLevel + Math.Log(scale, 2d), MinZoomLevel), MaxZoomLevel);
                InternalSetValue(ZoomLevelProperty, zoomLevel);
                InternalSetValue(TargetZoomLevelProperty, zoomLevel);
            }

            UpdateTransform();
            ResetTransformOrigin();
        }

        /// <summary>
        /// Sets the value of the TargetZoomLevel property while retaining the specified origin point
        /// in viewport coordinates.
        /// </summary>
        public void ZoomMap(Point origin, double zoomLevel)
        {
            SetTransformOrigin(origin);

            var targetZoomLevel = TargetZoomLevel;
            TargetZoomLevel = zoomLevel;

            if (TargetZoomLevel == targetZoomLevel) // TargetZoomLevel might be coerced
            {
                ResetTransformOrigin();
            }
        }

        protected override void OnViewportChanged()
        {
            base.OnViewportChanged();

            UpdateMapScale();

            if (ViewportChanged != null)
            {
                ViewportChanged(this, EventArgs.Empty);
            }
        }

        private void UpdateMapScale()
        {
            var length = 0d;
            var scaleWidth = 50d;

            length = scaleWidth / ParentMap.CenterScale;
            var magnitude = Math.Pow(10d, Math.Floor(Math.Log10(length)));

            if (length / magnitude < 2d)
            {
                length = 2d * magnitude;
            }
            else if (length / magnitude < 5d)
            {
                length = 5d * magnitude;
            }
            else
            {
                length = 10d * magnitude;
            }

            scaleWidth = length * ParentMap.CenterScale + 1 + 2 + 2;

            var scaleText = length >= 1000d ? string.Format("{0:0} km", length / 1000d) : string.Format("{0:0} m", length);

            _scale.SetScale(scaleWidth, scaleText);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            if (TileLayer == null)
            {
                TileLayer = TileLayer.Default;
            }

            UpdateTransform();
        }

        private void TileLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _tileContainer.AddTileLayers(e.NewStartingIndex, e.NewItems.Cast<TileLayer>());
                    break;

                case NotifyCollectionChangedAction.Remove:
                    _tileContainer.RemoveTileLayers(e.OldStartingIndex, e.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    _tileContainer.RemoveTileLayers(e.NewStartingIndex, e.OldItems.Count);
                    _tileContainer.AddTileLayers(e.NewStartingIndex, e.NewItems.Cast<TileLayer>());
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _tileContainer.ClearTileLayers();
                    if (e.NewItems != null)
                    {
                        _tileContainer.AddTileLayers(0, e.NewItems.Cast<TileLayer>());
                    }
                    break;
            }

            UpdateTileLayer();
        }

        private void TileLayersPropertyChanged(TileLayerCollection oldTileLayers, TileLayerCollection newTileLayers)
        {
            _tileContainer.ClearTileLayers();

            if (oldTileLayers != null)
            {
                oldTileLayers.CollectionChanged -= TileLayerCollectionChanged;
            }

            if (newTileLayers != null)
            {
                newTileLayers.CollectionChanged += TileLayerCollectionChanged;
                _tileContainer.AddTileLayers(0, newTileLayers);
            }

            UpdateTileLayer();
        }

        private void TileLayerPropertyChanged(TileLayer tileLayer)
        {
            if (tileLayer != null)
            {
                if (TileLayers == null)
                {
                    TileLayers = new TileLayerCollection();
                }

                if (TileLayers.Count == 0)
                {
                    TileLayers.Add(tileLayer);
                }
                else if (TileLayers[0] != tileLayer)
                {
                    TileLayers[0] = tileLayer;
                }

                if (TileLayer != null)
                    if (TileLayer.Description != null) _attributionBlock.Text = TileLayer.Description;
            }

            if (tileLayer != null && tileLayer.Background != null)
            {
                if (_storedBackground == null)
                {
                    _storedBackground = Background;
                }

                Background = tileLayer.Background;
            }
            else if (_storedBackground != null)
            {
                Background = _storedBackground;
                _storedBackground = null;
            }

            if (tileLayer != null && tileLayer.Foreground != null)
            {
                if (_storedForeground == null)
                {
                    _storedForeground = Foreground;
                }

                Foreground = tileLayer.Foreground;
            }
            else if (_storedForeground != null)
            {
                Foreground = _storedForeground;
                _storedForeground = null;
            }
        }

        public bool ZoomControlsEnabled
        {
            get { return (bool)GetValue(ZoomControlsEnabledProperty); }
            set { SetValue(ZoomControlsEnabledProperty, value); }
        }

        public static readonly DependencyProperty ZoomControlsEnabledProperty =
            DependencyProperty.Register("ZoomControlsEnabled", typeof(bool), typeof(MapBase), new PropertyMetadata(true, OnZoomControlsEnabledChanged));

        private static void OnZoomControlsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            var map = (MapBase)d;
            if (map._zoomControls != null)
            {
                map._zoomControls.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                if (!(bool)e.NewValue) return;
                map._zoomControls = new MapZoomControls();
                map._controlsHost.Children.Insert(0, map._zoomControls);
            }
        }

        public bool CurrentLocationEnabled
        {
            get { return (bool)GetValue(CurrentLocationEnabledProperty); }
            set { SetValue(CurrentLocationEnabledProperty, value); }
        }

        public static readonly DependencyProperty CurrentLocationEnabledProperty =
            DependencyProperty.Register("CurrentLocationEnabled", typeof(bool), typeof(MapBase), new PropertyMetadata(false, OnCurrentLocationEnabledChanged));

        private static void OnCurrentLocationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            var map = (MapBase)d;
            if (map._myLocation != null)
            {
                map._myLocation.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                if (!(bool)e.NewValue) return;
                map._myLocation = new MapMyLocationControl();
                map._controlsHost.Children.Add(map._myLocation);
            }
        }

        private void UpdateTileLayer()
        {
            TileLayer tileLayer = null;

            if (TileLayers != null)
            {
                tileLayer = TileLayers.FirstOrDefault();
            }

            if (TileLayer != null && TileLayer != tileLayer)
            {
                TileLayer = tileLayer;
            }
        }

        private void InternalSetValue(DependencyProperty property, object value)
        {
            _internalPropertyChange = true;
            SetValue(property, value);
            _internalPropertyChange = false;
        }

        private bool CoerceLocation(Location location, double latitudeEpsilon = 0d)
        {
            var maxLatitude = _mapTransform.MaxLatitude + latitudeEpsilon;
            var latitude = Math.Min(Math.Max(location.Latitude, -maxLatitude), maxLatitude);
            var longitude = Location.NormalizeLongitude(location.Longitude);

            if (location.Latitude != latitude || location.Longitude != longitude)
            {
                location.Latitude = latitude;
                location.Longitude = longitude;
                return true;
            }

            return false;
        }

        private void CoerceCenterProperty(DependencyProperty property, Location center)
        {
            if (CoerceLocation(center))
            {
                InternalSetValue(property, center);
            }
        }

        private void CenterPropertyChanged(Location center)
        {
            if (!_internalPropertyChange)
            {
                CoerceCenterProperty(CenterProperty, center);
                ResetTransformOrigin();
                UpdateTransform();

                if (_centerAnimation == null)
                {
                    InternalSetValue(TargetCenterProperty, center);
                    InternalSetValue(CenterPointProperty, new Point(center.Longitude, center.Latitude));
                }
            }
        }

        private void TargetCenterPropertyChanged(Location targetCenter)
        {
            if (!_internalPropertyChange)
            {
                CoerceCenterProperty(TargetCenterProperty, targetCenter);

                if (!targetCenter.Equals(Center))
                {
                    if (_centerAnimation != null)
                    {
                        _centerAnimation.Completed -= CenterAnimationCompleted;
                    }

                    // animate private CenterPoint property by PointAnimation
                    _centerAnimation = new PointAnimation
                    {
                        From = new Point(Center.Longitude, Center.Latitude),
                        To = new Point(targetCenter.Longitude, targetCenter.Latitude),
                        Duration = AnimationDuration,
                        EasingFunction = AnimationEasingFunction,
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    _centerAnimation.Completed += CenterAnimationCompleted;
                    this.BeginAnimation(CenterPointProperty, _centerAnimation);
                }
            }
        }

        private void CenterAnimationCompleted(object sender, object e)
        {
            if (_centerAnimation != null)
            {
                _centerAnimation.Completed -= CenterAnimationCompleted;
                _centerAnimation = null;

                InternalSetValue(CenterProperty, TargetCenter);
                InternalSetValue(CenterPointProperty, new Point(TargetCenter.Longitude, TargetCenter.Latitude));

                ResetTransformOrigin();
                UpdateTransform();
            }
        }

        private void CenterPointPropertyChanged(Point centerPoint)
        {
            if (!_internalPropertyChange)
            {
                InternalSetValue(CenterProperty, new Location(centerPoint.Y, centerPoint.X));
                ResetTransformOrigin();
                UpdateTransform();
            }
        }

        private void MinZoomLevelPropertyChanged(double minZoomLevel)
        {
            var coercedValue = Math.Min(Math.Max(minZoomLevel, 0d), MaxZoomLevel);

            if (coercedValue != minZoomLevel)
            {
                InternalSetValue(MinZoomLevelProperty, coercedValue);
            }
            else if (ZoomLevel < minZoomLevel)
            {
                ZoomLevel = minZoomLevel;
            }
        }

        private void MaxZoomLevelPropertyChanged(double maxZoomLevel)
        {
            var coercedValue = Math.Min(Math.Max(maxZoomLevel, MinZoomLevel), 22d);

            if (coercedValue != maxZoomLevel)
            {
                InternalSetValue(MaxZoomLevelProperty, coercedValue);
            }
            else if (ZoomLevel > maxZoomLevel)
            {
                ZoomLevel = maxZoomLevel;
            }
        }

        private bool CoerceZoomLevelProperty(DependencyProperty property, ref double zoomLevel)
        {
            var coercedValue = Math.Min(Math.Max(zoomLevel, MinZoomLevel), MaxZoomLevel);

            if (coercedValue != zoomLevel)
            {
                InternalSetValue(property, coercedValue);
                return true;
            }

            return false;
        }

        private void ZoomLevelPropertyChanged(double zoomLevel)
        {
            if (!_internalPropertyChange &&
                !CoerceZoomLevelProperty(ZoomLevelProperty, ref zoomLevel))
            {
                UpdateTransform();

                if (_zoomLevelAnimation == null)
                {
                    InternalSetValue(TargetZoomLevelProperty, zoomLevel);
                }
            }
        }

        private void TargetZoomLevelPropertyChanged(double targetZoomLevel)
        {
            if (!_internalPropertyChange &&
                !CoerceZoomLevelProperty(TargetZoomLevelProperty, ref targetZoomLevel) &&
                targetZoomLevel != ZoomLevel)
            {
                if (_zoomLevelAnimation != null)
                {
                    _zoomLevelAnimation.Completed -= ZoomLevelAnimationCompleted;
                }

                _zoomLevelAnimation = new DoubleAnimation
                {
                    To = targetZoomLevel,
                    Duration = AnimationDuration,
                    EasingFunction = AnimationEasingFunction,
                    FillBehavior = FillBehavior.HoldEnd
                };

                _zoomLevelAnimation.Completed += ZoomLevelAnimationCompleted;
                this.BeginAnimation(ZoomLevelProperty, _zoomLevelAnimation);
            }
        }

        private void ZoomLevelAnimationCompleted(object sender, object e)
        {
            if (_zoomLevelAnimation != null)
            {
                _zoomLevelAnimation.Completed -= ZoomLevelAnimationCompleted;
                _zoomLevelAnimation = null;

                InternalSetValue(ZoomLevelProperty, TargetZoomLevel);

                UpdateTransform();
                ResetTransformOrigin();
            }
        }

        private void CoerceHeadingProperty(DependencyProperty property, ref double heading)
        {
            var coercedValue = (heading >= -180d && heading <= 360d) ?
                heading : (((heading % 360d) + 360d) % 360d);

            if (coercedValue != heading)
            {
                InternalSetValue(property, coercedValue);
            }
        }

        private void HeadingPropertyChanged(double heading)
        {
            if (!_internalPropertyChange)
            {
                CoerceHeadingProperty(HeadingProperty, ref heading);
                UpdateTransform();

                if (_headingAnimation == null)
                {
                    InternalSetValue(TargetHeadingProperty, heading);
                }
            }
        }

        private void TargetHeadingPropertyChanged(double targetHeading)
        {
            if (!_internalPropertyChange)
            {
                CoerceHeadingProperty(TargetHeadingProperty, ref targetHeading);

                if (targetHeading != Heading)
                {
                    var delta = targetHeading - Heading;

                    if (delta > 180d)
                    {
                        delta -= 360d;
                    }
                    else if (delta < -180d)
                    {
                        delta += 360d;
                    }

                    if (_headingAnimation != null)
                    {
                        _headingAnimation.Completed -= HeadingAnimationCompleted;
                    }

                    _headingAnimation = new DoubleAnimation
                    {
                        By = delta,
                        Duration = AnimationDuration,
                        EasingFunction = AnimationEasingFunction,
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    _headingAnimation.Completed += HeadingAnimationCompleted;
                    this.BeginAnimation(HeadingProperty, _headingAnimation);
                }
            }
        }

        private void HeadingAnimationCompleted(object sender, object e)
        {
            if (_headingAnimation != null)
            {
                _headingAnimation.Completed -= HeadingAnimationCompleted;
                _headingAnimation = null;

                InternalSetValue(HeadingProperty, TargetHeading);

                UpdateTransform();
            }
        }

        private void UpdateTransform()
        {
            var center = Center;
            var scale = SetViewportTransform(_transformOrigin ?? center);

            if (_transformOrigin != null)
            {
                center = ViewportPointToLocation(new Point(RenderSize.Width / 2d, RenderSize.Height / 2d));

                var coerced = CoerceLocation(center, 1e-3);

                InternalSetValue(CenterProperty, center);

                if (coerced)
                {
                    ResetTransformOrigin();
                    scale = SetViewportTransform(center);
                }
            }

            scale *= _mapTransform.RelativeScale(center) / MetersPerDegree; // Pixels per meter at center latitude
            CenterScale = scale;
            SetTransformMatrixes(scale);

            OnViewportChanged();
        }

        private double SetViewportTransform(Location origin)
        {
            return _tileContainer.SetViewportTransform(ZoomLevel, Heading, _mapTransform.Transform(origin), _viewportOrigin, RenderSize);
        }
    }
}
