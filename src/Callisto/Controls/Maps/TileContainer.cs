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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Callisto.Controls.Common;

namespace Callisto.Controls
{
    internal class TileContainer : Panel
    {
        private Matrix GetViewportTransformMatrix(Matrix transform)
        {
            return transform.RotateAt(_rotation, _viewportOrigin.X, _viewportOrigin.Y);
        }

        /// <summary>
        /// Gets a transform matrix with origin at tileGrid.X and tileGrid.Y to minimize rounding errors.
        /// </summary>
        private Matrix GetTileLayerTransformMatrix()
        {
            var scale = Math.Pow(2d, _zoomLevel - _tileZoomLevel);

            return new Matrix(1d, 0d, 0d, 1d, _tileGrid.X * TileSource.TileSize, _tileGrid.Y * TileSource.TileSize)
                .Scale(scale, scale)
                .Translate(_tileLayerOffset.X, _tileLayerOffset.Y)
                .RotateAt(_rotation, _viewportOrigin.X, _viewportOrigin.Y);
        }

        private Matrix GetTileIndexMatrix(int numTiles)
        {
            var scale = numTiles / 360d;

            return ViewportTransform.Matrix
                .Invert() // view to map coordinates
                .Translate(180d, -180d)
                .Scale(scale, -scale); // map coordinates to tile indices
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (TileLayer tileLayer in Children)
            {
                tileLayer.Measure(availableSize);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (TileLayer tileLayer in Children)
            {
                tileLayer.Arrange(new Rect());
            }

            return finalSize;
        }

        // relative scaled tile size ranges from 0.75 to 1.5 (192 to 384 pixels)
        private static double zoomLevelSwitchDelta = -Math.Log(0.75, 2d);

        internal static TimeSpan UpdateInterval = TimeSpan.FromSeconds(0.5);

        private readonly DispatcherTimer _updateTimer;
        private Size _viewportSize;
        private Point _viewportOrigin;
        private Point _tileLayerOffset;
        private double _rotation;
        private double _zoomLevel;
        private int _tileZoomLevel;
        private Int32Rect _tileGrid;

        public readonly MatrixTransform ViewportTransform = new MatrixTransform();

        public TileContainer()
        {
            _updateTimer = new DispatcherTimer { Interval = UpdateInterval };
            _updateTimer.Tick += UpdateTiles;
        }

        public void AddTileLayers(int index, IEnumerable<TileLayer> tileLayers)
        {
            var tileLayerTransform = GetTileLayerTransformMatrix();

            foreach (var tileLayer in tileLayers)
            {
                if (index < Children.Count)
                {
                    Children.Insert(index, tileLayer);
                }
                else
                {
                    Children.Add(tileLayer);
                }

                index++;
                tileLayer.SetTransformMatrix(tileLayerTransform);
                tileLayer.UpdateTiles(_tileZoomLevel, _tileGrid);
            }
        }

        public void RemoveTileLayers(int index, int count)
        {
            while (count-- > 0)
            {
                ((TileLayer)Children[index]).ClearTiles();
                Children.RemoveAt(index);
            }
        }

        public void ClearTileLayers()
        {
            foreach (TileLayer tileLayer in Children)
            {
                tileLayer.ClearTiles();
            }

            Children.Clear();
        }

        public double SetViewportTransform(double mapZoomLevel, double mapRotation, Point mapOrigin, Point vpOrigin, Size vpSize)
        {
            var scale = Math.Pow(2d, _zoomLevel) * TileSource.TileSize / 360d;
            var oldMapOriginX = (_viewportOrigin.X - _tileLayerOffset.X) / scale - 180d;

            if (_zoomLevel != mapZoomLevel)
            {
                _zoomLevel = mapZoomLevel;
                scale = Math.Pow(2d, _zoomLevel) * TileSource.TileSize / 360d;
            }

            _rotation = mapRotation;
            _viewportSize = vpSize;
            _viewportOrigin = vpOrigin;

            var transformOffsetX = _viewportOrigin.X - mapOrigin.X * scale;
            var transformOffsetY = _viewportOrigin.Y + mapOrigin.Y * scale;

            _tileLayerOffset.X = transformOffsetX - 180d * scale;
            _tileLayerOffset.Y = transformOffsetY - 180d * scale;

            ViewportTransform.Matrix = GetViewportTransformMatrix(new Matrix(scale, 0d, 0d, -scale, transformOffsetX, transformOffsetY));

            if (Math.Sign(mapOrigin.X) != Math.Sign(oldMapOriginX) && Math.Abs(mapOrigin.X) > 90d)
            {
                // immediately handle map origin leap when map center moves across the date line
                UpdateTiles(this, EventArgs.Empty);
            }
            else
            {
                var tileLayerTransform = GetTileLayerTransformMatrix();

                foreach (TileLayer tileLayer in Children)
                {
                    tileLayer.SetTransformMatrix(tileLayerTransform);
                }

                _updateTimer.Start();
            }

            return scale;
        }

        private void UpdateTiles(object sender, object e)
        {
            _updateTimer.Stop();

            var zoom = (int)Math.Floor(_zoomLevel + zoomLevelSwitchDelta);
            var transform = GetTileIndexMatrix(1 << zoom);

            // tile indices of visible rectangle
            var p1 = transform.Transform(new Point(0d, 0d));
            var p2 = transform.Transform(new Point(_viewportSize.Width, 0d));
            var p3 = transform.Transform(new Point(0d, _viewportSize.Height));
            var p4 = transform.Transform(new Point(_viewportSize.Width, _viewportSize.Height));

            // index ranges of visible tiles
            var x1 = (int)Math.Floor(Math.Min(p1.X, Math.Min(p2.X, Math.Min(p3.X, p4.X))));
            var y1 = (int)Math.Floor(Math.Min(p1.Y, Math.Min(p2.Y, Math.Min(p3.Y, p4.Y))));
            var x2 = (int)Math.Floor(Math.Max(p1.X, Math.Max(p2.X, Math.Max(p3.X, p4.X))));
            var y2 = (int)Math.Floor(Math.Max(p1.Y, Math.Max(p2.Y, Math.Max(p3.Y, p4.Y))));
            var grid = new Int32Rect(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

            if (_tileZoomLevel != zoom || _tileGrid != grid)
            {
                _tileZoomLevel = zoom;
                _tileGrid = grid;
                var tileLayerTransform = GetTileLayerTransformMatrix();

                foreach (TileLayer tileLayer in Children)
                {
                    tileLayer.SetTransformMatrix(tileLayerTransform);
                    tileLayer.UpdateTiles(_tileZoomLevel, _tileGrid);
                }
            }
        }
    }
}
