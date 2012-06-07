﻿//
// Copyright (c) 2012 Tim Heuer
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

// BASE CODE PROVIDED UNDER THIS LICENSE
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls.Primitives
{
    /// <summary>
    /// Clips the content of the control in a given direction.
    /// </summary>
    public class LinearClipper : Clipper
    {
        #region public ExpandDirection ExpandDirection
        /// <summary>
        /// Gets or sets the clipped edge.
        /// </summary>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection) GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                "ExpandDirection",
                typeof(ExpandDirection),
                typeof(LinearClipper),
                new PropertyMetadata(ExpandDirection.Right, OnExpandDirectionChanged));

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandDirectionView that changed its ExpandDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExpandDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearClipper source = (LinearClipper)d;
            ExpandDirection oldValue = (ExpandDirection)e.OldValue;
            ExpandDirection newValue = (ExpandDirection)e.NewValue;
            source.OnExpandDirectionChanged(oldValue, newValue);
        }

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnExpandDirectionChanged(ExpandDirection oldValue, ExpandDirection newValue)
        {
            ClipContent();
        }
        #endregion public ExpandDirection ExpandDirection

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        protected override void ClipContent()
        {
            if (ExpandDirection == ExpandDirection.Right)
            {
                double width = this.RenderSize.Width * RatioVisible;
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, this.RenderSize.Height) };
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                double width = this.RenderSize.Width * RatioVisible;
                double rightSide = this.RenderSize.Width - width;
                this.Clip = new RectangleGeometry { Rect = new Rect(rightSide, 0, width, this.RenderSize.Height) };
            }
            else if (ExpandDirection == ExpandDirection.Up)
            {
                double height = this.RenderSize.Height * RatioVisible;
                double bottom = this.RenderSize.Height - height;
                this.Clip = new RectangleGeometry { Rect = new Rect(0, bottom, this.RenderSize.Width, height) };
            }
            else if (ExpandDirection == ExpandDirection.Down)
            {
                double height = this.RenderSize.Height * RatioVisible;
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.RenderSize.Width, height) };
            }
        }
    }
}