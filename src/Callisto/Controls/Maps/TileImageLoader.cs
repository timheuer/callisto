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
using System.Diagnostics;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Callisto.Controls
{
    /// <summary>
    /// Loads map tile images.
    /// </summary>
    internal class TileImageLoader
    {
        internal void BeginGetTiles(TileLayer tileLayer, IEnumerable<Tile> tiles)
        {
            var imageTileSource = tileLayer.TileSource as ImageTileSource;

            foreach (var tile in tiles)
            {
                try
                {
                    ImageSource image;

                    if (imageTileSource != null)
                    {
                        image = imageTileSource.LoadImage(tile.XIndex, tile.Y, tile.ZoomLevel);
                    }
                    else
                    {
                        var uri = tileLayer.TileSource.GetUri(tile.XIndex, tile.Y, tile.ZoomLevel);

                        image = uri != null ? new BitmapImage(uri) : null;
                    }

                    if (image != null)
                    {
                        tile.SetImageSource(image, tileLayer.AnimateTileOpacity);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Creating tile image failed: {0}", ex.Message);
                }
            }
        }

        internal void CancelGetTiles()
        {
        }
    }
}
