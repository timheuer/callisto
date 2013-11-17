// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls.Common
{
    public static class TransformEx
    {
        public static Point Transform(this GeneralTransform transform, Point point)
        {
            return transform.TransformPoint(point);
        }
    }
}
