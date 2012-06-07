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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls.Primitives
{
    /// <summary>
    /// Clips a ratio of its content.
    /// </summary>
    public abstract class Clipper : ContentControl
    {
        #region public double RatioVisible
        /// <summary>
        /// Gets or sets the percentage of the item visible.
        /// </summary>
        public double RatioVisible
        {
            get { return (double)GetValue(RatioVisibleProperty); }
            set { SetValue(RatioVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the RatioVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty RatioVisibleProperty =
            DependencyProperty.Register(
                "RatioVisible",
                typeof(double),
                typeof(Clipper),
                new PropertyMetadata(1.0, OnRatioVisibleChanged));

        /// <summary>
        /// RatioVisibleProperty property changed handler.
        /// </summary>
        /// <param name="d">PartialView that changed its RatioVisible.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRatioVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clipper source = (Clipper)d;
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            source.OnRatioVisibleChanged(oldValue, newValue);
        }

        /// <summary>
        /// RatioVisibleProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnRatioVisibleChanged(double oldValue, double newValue)
        {
            if (newValue >= 0.0 && newValue <= 1.0)
            {
                ClipContent();
            }
            else
            {
                if (newValue < 0.0)
                {
                    this.RatioVisible = 0.0;
                }
                else if (newValue > 1.0)
                {
                    this.RatioVisible = 1.0;
                }
            }
        }

        #endregion public double RatioVisible

        /// <summary>
        /// Initializes a new instance of the Clipper class.
        /// </summary>
        protected Clipper()
        {
            this.SizeChanged += delegate { ClipContent(); };
        }

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        protected abstract void ClipContent();
    }
}
