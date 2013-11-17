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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    /// <summary>
    /// Fills a rectangular area with an ImageBrush from the Source property.
    /// </summary>
    public class MapImage : MapRectangle
    {
        private static readonly MatrixTransform ImageTransform = new MatrixTransform
        {
            Matrix = new Matrix(1d, 0d, 0d, -1d, 0d, 1d)
        };

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(ImageSource), typeof(MapImage),
            new PropertyMetadata(null, (o, e) => ((MapImage)o).SourceChanged((ImageSource)e.NewValue)));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void SourceChanged(ImageSource image)
        {
            var imageBrush = new ImageBrush
            {
                ImageSource = image,
                RelativeTransform = ImageTransform
            };

            Fill = imageBrush;
        }
    }
}
