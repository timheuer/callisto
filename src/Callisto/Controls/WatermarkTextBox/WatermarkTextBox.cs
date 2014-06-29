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

using Callisto.Controls.Common;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. WaterMarkTextBox is a derivitave of TextBox that adds only the functionality of providing a default text
	/// 'hint' in the text input area if no text exists. This is helpful in providing some tip to your users e.g.,
	/// "Enter your name here..." which will automatically go away when the user starts typing. The Watermark has
	/// no affect on the Text property or other aspects of the TextBox control.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This control is deprecated in favor of using the  PlaceholderText  property on the  TextBox  controls built in to Windows 8.1
	/// </para>
	/// <para>
	/// WaterMarkTextBox is a derivitave of TextBox that adds only the functionality of providing a default text 'hint' in
	/// the text input area if no text exists. This is helpful in providing some tip to your users e.g.,
	/// "Enter your name here..." which will automatically go away when the user starts typing. The Watermark has no affect
	/// on the Text property or other aspects of the TextBox control.
	/// </para>
	/// </remarks>
    [Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as PlaceholderText on TextBox.")]
    [TemplatePart(Name = WatermarkTextBox.PART_ELEMENT_CONTENT_NAME, Type = typeof(ContentControl))]
    public sealed class WatermarkTextBox : TextBox
    {
        private const string PART_ELEMENT_CONTENT_NAME = "PART_Watermark";
        internal TextBlock ElementContent { get; set; }
        internal bool IsHovered;
        internal bool HasFocusInternal;
        private ResourceLoader _resources;

		/// <summary>
		/// Initializes a new instance of the <see cref="WatermarkTextBox"/> class.
		/// </summary>
        public WatermarkTextBox()
        {
            this.DefaultStyleKey = typeof(WatermarkTextBox);

            _resources = new ResourceLoader("Callisto/Resources");
            // set some default text
            // TODO: Localize this string.
            Watermark = _resources.GetString("WatermarkTextBoxDefault");

            this.GotFocus += OnGotFocus;
            this.LostFocus += OnLostFocus;
            this.PointerEntered += OnPointerEntered;
            this.PointerExited += OnPointerExited;
            this.TextChanged += OnTextChanged;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();
            ChangeVisualState(false);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeVisualState();
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsHovered = false;

            if (!HasFocusInternal)
            {
                ChangeVisualState();
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsHovered = true;

            if (!HasFocusInternal)
            {
                ChangeVisualState();
            }
        }


        internal void ChangeVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the textbox.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            // Update the CommonStates group
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsHovered)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StatePointerOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the FocusStates group
            if (HasFocusInternal && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            // Update the WatermarkStates group
            if (this.Watermark != null && string.IsNullOrEmpty(this.Text) && !HasFocusInternal)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateWatermarked, VisualStates.StateUnwatermarked);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnwatermarked);
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            HasFocusInternal = false;
            ChangeVisualState();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                HasFocusInternal = true;

                ChangeVisualState();
            }
        }

        private void OnWatermarkChanged()
        {
            if (ElementContent != null)
            {
                Control watermarkControl = this.Watermark as Control;
                if (watermarkControl != null)
                {
                    watermarkControl.IsTabStop = false;
                    watermarkControl.IsHitTestVisible = false;
                }
            }
        }

		/// <summary>
		/// Invoked whenever application code or internal processes (such as a rebuilding layout pass)
		/// call ApplyTemplate. In simplest terms, this means the method is called just before a UI
		/// element displays in your app. Override this method to influence the default post-template 
		/// logic of a class.
		/// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementContent = (TextBlock)GetTemplateChild(PART_ELEMENT_CONTENT_NAME);

            // if the PlaceholderText value (new in Win8.1) is set, we will use that as Watermark
            if (!string.IsNullOrEmpty(PlaceholderText)) Watermark = PlaceholderText;

            OnWatermarkChanged();

            ChangeVisualState(false);
        }

		/// <summary>
		/// Gets or sets the watermark.
		/// </summary>
		public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="Watermark"/> dependency property
		/// </summary>
		public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new PropertyMetadata(null, OnWatermarkPropertyChanged));

        private static void OnWatermarkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBox wmt = d as WatermarkTextBox;

            if (wmt != null)
            {
                Control watermarkControl = wmt.Watermark as Control;
                if (watermarkControl != null)
                {
                    watermarkControl.IsTabStop = true;
                    watermarkControl.IsHitTestVisible = true;
                }
            }
        }

    }
}
