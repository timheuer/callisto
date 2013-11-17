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
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    /// <summary>
    /// Fills a rectangular area with map tiles from a TileSource.
    /// </summary>
    [ContentProperty(Name = "TileSource")]
    public class TileLayer : Panel
    {
        internal void Initialize()
        {
            RenderTransform = _transform;
        }

        protected Panel TileContainer
        {
            get { return Parent as Panel; }
        }

        protected void RenderTiles()
        {
            Children.Clear();
            foreach (var tile in _tiles)
            {
                Children.Add(tile.Image);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var tile in _tiles)
            {
                tile.Image.Measure(availableSize);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var tile in _tiles)
            {
                var tileSize = (double)(256 << (_zoomLevel - tile.ZoomLevel));
                tile.Image.Width = tileSize;
                tile.Image.Height = tileSize;
                tile.Image.Arrange(new Rect(tileSize * tile.X - 256 * _grid.X, tileSize * tile.Y - 256 * _grid.Y, tileSize, tileSize));
            }

            return finalSize;
        }
        public static TileLayer Default
        {
            get
            {
                return new TileLayer
                {
                    SourceName = "OpenStreetMap",
                    Description = "© {y} OpenStreetMap Contributors, CC-BY-SA",
                    TileSource = new TileSource("http://{c}.tile.openstreetmap.org/{z}/{x}/{y}.png")
                };
            }
        }

        private readonly MatrixTransform _transform = new MatrixTransform();
        private readonly TileImageLoader _tileImageLoader = new TileImageLoader();
        private List<Tile> _tiles = new List<Tile>();
        private string _description = string.Empty;
        private Int32Rect _grid;
        private int _zoomLevel;

        public TileLayer()
        {
            MinZoomLevel = 1;
            MaxZoomLevel = 18;
            MaxParallelDownloads = 8;
            LoadLowerZoomLevels = true;
            AnimateTileOpacity = true;
            Initialize();
        }

        public string SourceName { get; set; }
        public TileSource TileSource { get; set; }
        public int MinZoomLevel { get; set; }
        public int MaxZoomLevel { get; set; }
        public int MaxParallelDownloads { get; set; }
        public bool LoadLowerZoomLevels { get; set; }
        public bool AnimateTileOpacity { get; set; }
        public Brush Foreground { get; set; }

        public string Description
        {
            get { return _description; }
            set { _description = value.Replace("{y}", DateTime.Now.Year.ToString()); }
        }

        public string TileSourceUriFormat
        {
            get { return TileSource != null ? TileSource.UriFormat : string.Empty; }
            set { TileSource = new TileSource(value); }
        }

        internal void SetTransformMatrix(Matrix transformMatrix)
        {
            _transform.Matrix = transformMatrix;
        }

        protected internal virtual void UpdateTiles(int zoomLevel, Int32Rect grid)
        {
            _grid = grid;
            _zoomLevel = zoomLevel;

            _tileImageLoader.CancelGetTiles();

            if (TileSource == null) return;
            SelectTiles();
            RenderTiles();
            _tileImageLoader.BeginGetTiles(this, _tiles.Where(t => !t.HasImageSource));
        }

        protected internal virtual void ClearTiles()
        {
            _tileImageLoader.CancelGetTiles();
            _tiles.Clear();
            RenderTiles();
        }

        protected void SelectTiles()
        {
            var maxZoomLevel = Math.Min(_zoomLevel, MaxZoomLevel);
            var minZoomLevel = maxZoomLevel;
            var container = TileContainer;

            if (LoadLowerZoomLevels && container != null && container.Children.IndexOf(this) == 0)
            {
                minZoomLevel = MinZoomLevel;
            }

            var newTiles = new List<Tile>();

            for (var z = minZoomLevel; z <= maxZoomLevel; z++)
            {
                var tileSize = 1 << (_zoomLevel - z);
                var x1 = (int)Math.Floor((double)_grid.X / tileSize); // may be negative
                var x2 = (_grid.X + _grid.Width - 1) / tileSize;
                var y1 = Math.Max(_grid.Y / tileSize, 0);
                var y2 = Math.Min((_grid.Y + _grid.Height - 1) / tileSize, (1 << z) - 1);

                for (var y = y1; y <= y2; y++)
                {
                    for (var x = x1; x <= x2; x++)
                    {
                        var tile = _tiles.FirstOrDefault(t => t.ZoomLevel == z && t.X == x && t.Y == y);

                        if (tile == null)
                        {
                            tile = new Tile(z, x, y);

                            var equivalentTile = _tiles.FirstOrDefault(
                                t => t.ImageSource != null && t.ZoomLevel == z && t.XIndex == tile.XIndex && t.Y == y);

                            if (equivalentTile != null)
                            {
                                // do not animate to avoid flicker when crossing date line
                                tile.SetImageSource(equivalentTile.ImageSource, false);
                            }
                        }

                        newTiles.Add(tile);
                    }
                }
            }

            _tiles = newTiles;
        }
    }
}
