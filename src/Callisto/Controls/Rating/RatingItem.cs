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

using Callisto.Controls.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    /// <summary>
    /// An item used in a rating control.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerPressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerExited, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = StateFilled, GroupName = GroupFill)]
    [TemplateVisualState(Name = StateEmpty, GroupName = GroupFill)]
    [TemplateVisualState(Name = StatePartial, GroupName = GroupFill)]
    public class RatingItem : ButtonBase, IUpdateVisualState
    {
        /// <summary>
        /// The state in which the item is filled.
        /// </summary>
        private const string StateFilled = "Filled";

        /// <summary>
        /// The state in which the item is empty.
        /// </summary>
        private const string StateEmpty = "Empty";

        /// <summary>
        /// The group that contains fill states.
        /// </summary>
        private const string GroupFill = "FillStates";

        /// <summary>
        /// The state in which the item is partially filled.
        /// </summary>
        private const string StatePartial = "Partial";

        /// <summary>
        /// The interaction helper used to get the common states working.
        /// </summary>
        private InteractionHelper _interactionHelper;

        #region public double DisplayValue
        /// <summary>
        /// A value indicating whether the actual value is being set.
        /// </summary>
        private bool _settingDisplayValue;

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        public double DisplayValue
        {
            get { return (double)GetValue(DisplayValueProperty); }
            internal set
            {
                _settingDisplayValue = true;
                try
                {
                    SetValue(DisplayValueProperty, value);
                }
                finally
                {
                    _settingDisplayValue = false;
                }
            }
        }

        /// <summary>
        /// Identifies the DisplayValue dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(
                "DisplayValue",
                typeof(double),
                typeof(RatingItem),
                new PropertyMetadata(0.0, OnDisplayValueChanged));

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary>
        /// <param name="d">RatingItem that changed its DisplayValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDisplayValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RatingItem source = (RatingItem)d;
            source.OnDisplayValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary> 
        /// <param name="oldValue">The old value.</param> 
        /// <param name="newValue">The new value.</param>
        private void OnDisplayValueChanged(double oldValue, double newValue)
        {
            if (!_settingDisplayValue)
            {
                _settingDisplayValue = true;

                this.DisplayValue = oldValue;

                throw new InvalidOperationException(string.Format("Invalid attempt to change read-only property '{0}.'", "DisplayValue"));
            }
            else
            {
                if (newValue <= 0.0)
                {
                    VisualStates.GoToState(this, true, StateEmpty);
                }
                else if (newValue >= 1.0)
                {
                    VisualStates.GoToState(this, true, StateFilled);
                }
                else
                {
                    VisualStates.GoToState(this, true, StatePartial);
                }
            }
        }
        #endregion public double DisplayValue

        #region PointerOverFill
        /// <summary>
        /// Sets the PointerOver brush color
        /// </summary>
        public SolidColorBrush PointerOverFill
        {
            get { return (SolidColorBrush)GetValue(PointerOverFillProperty); }
            set { SetValue(PointerOverFillProperty, value); }
        }

        public static readonly DependencyProperty PointerOverFillProperty =
            DependencyProperty.Register("PointerOverFill", typeof(SolidColorBrush), typeof(RatingItem), null); 
        #endregion PointerOverFill

        #region PointerPressedFill
        /// <summary>
        /// Sets the PointerPressed brush color
        /// </summary>
        public SolidColorBrush PointerPressedFill
        {
            get { return (SolidColorBrush)GetValue(PointerPressedFillProperty); }
            set { SetValue(PointerPressedFillProperty, value); }
        }

        public static readonly DependencyProperty PointerPressedFillProperty =
            DependencyProperty.Register("PointerPressedFill", typeof(SolidColorBrush), typeof(RatingItem), null);
        #endregion

        #region ReadonlyFill

        public static readonly DependencyProperty ReadOnlyFillProperty =
            DependencyProperty.Register("ReadOnlyFill", typeof (SolidColorBrush), typeof (RatingItem), null);

        public SolidColorBrush ReadOnlyFill
        {
            get { return (SolidColorBrush) GetValue(ReadOnlyFillProperty); }
            set { SetValue(ReadOnlyFillProperty, value); }
        }
        #endregion

        /// <summary>
        /// Gets or sets the parent rating of this rating item.
        /// </summary>
        internal Rating ParentRating { get; set; }

        #region public double Value
        /// <summary>
        /// Gets or sets the value property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Value is the logical name for this property.")]
        internal double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        internal static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(RatingItem),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Selects a value and raises the value selected event.
        /// </summary>
        internal void SelectValue()
        {
            if (!this.IsEnabled)
            {
                this.Value = 1.0;
                OnTapped(null);
            }
        }

        #endregion public double Value

        /// <summary>
        /// Initializes a new instance of the RatingItem class.
        /// </summary>
        public RatingItem()
        {
            this.DefaultStyleKey = typeof(RatingItem);

            _interactionHelper = new InteractionHelper(this);
        }

        /// <summary>
        /// Provides handling for the RatingItem's PointerPressed event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (_interactionHelper.AllowPointerPressed(e))
            {
                _interactionHelper.OnPointerPressedBase();
            }
            base.OnPointerPressed(e);
        }

        /// <summary>
        /// Provides handling for the RatingItem's PointerReleased event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (_interactionHelper.AllowPointerPressed(e))
            {
                _interactionHelper.OnPointerReleasedBase();
            }
            base.OnPointerReleased(e);
        }

        /// <summary>
        /// This method is invoked when the pointer enters the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (_interactionHelper.AllowPointerEnter(e))
            {
                _interactionHelper.UpdateVisualStateBase(true);
            }
            base.OnPointerEntered(e);
        }

        /// <summary>
        /// This method is invoked when the pointer leaves the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (_interactionHelper.AllowPointerExit(e))
            {
                _interactionHelper.UpdateVisualStateBase(true);
            }
            base.OnPointerExited(e);
        }

        /// <summary>
        /// Sets the value to 1.0 when clicked.
        /// </summary>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to use 
        /// transitions.</param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            _interactionHelper.UpdateVisualStateBase(useTransitions);
        }
    }
}
