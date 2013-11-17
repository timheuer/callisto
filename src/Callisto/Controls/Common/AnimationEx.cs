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
using Windows.UI.Xaml.Media.Animation;

namespace Callisto.Controls.Common
{
    public static class AnimationEx
    {
        public static void BeginAnimation(this DependencyObject obj, DependencyProperty property, DoubleAnimation animation)
        {
            animation.EnableDependentAnimation = true;

            if (property == UIElement.OpacityProperty)
            {
                BeginAnimation(obj, "Opacity", animation);
            }
            else if (property == MapBase.ZoomLevelProperty)
            {
                BeginAnimation(obj, "ZoomLevel", animation);
            }
            else if (property == MapBase.HeadingProperty)
            {
                BeginAnimation(obj, "Heading", animation);
            }
        }

        public static void BeginAnimation(this DependencyObject obj, DependencyProperty property, PointAnimation animation)
        {
            animation.EnableDependentAnimation = true;

            if (property == MapBase.CenterPointProperty)
            {
                BeginAnimation(obj, "CenterPoint", animation);
            }
        }

        private static void BeginAnimation(DependencyObject obj, string property, Timeline animation)
        {
            Storyboard.SetTargetProperty(animation, property);
            Storyboard.SetTarget(animation, obj);
            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
    }
}