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

using System;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. An item for a menu, including separators and contains the command point for the menu item
	/// </summary>
	/// <remarks>
	/// This control is deprecated in favor of using the <see cref="Windows.UI.Xaml.Controls.MenuFlyout"/> controls in Windows 8.1.
	/// </remarks>
	[Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as MenuFlyoutItem.")]
    [TemplateVisualState(Name = MenuItem.StateBase, GroupName = MenuItem.GroupCommon)]
    [TemplateVisualState(Name = MenuItem.StateHover, GroupName = MenuItem.GroupCommon)]
    [TemplateVisualState(Name = MenuItem.StatePressed, GroupName = MenuItem.GroupCommon)]
    [TemplateVisualState(Name = MenuItem.StateDisabled, GroupName = MenuItem.GroupCommon)]
    public class MenuItem : MenuItemBase
    {
		/// <summary>
		/// Identifies the <see cref="Text"/> dependency property
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MenuItem), null);
		/// <summary>
		/// Identifies the <see cref="Command"/> dependency property
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuItem), null);
		/// <summary>
		/// Identifies the <see cref="CommandParameter"/> dependency property
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuItem), null);

        private const string GroupCommon = "Common";
        private const string StateBase = "Base";
        private const string StateHover = "Hover";
        private const string StatePressed = "Pressed";
        private const string StateDisabled = "Disabled";

        private bool _isActive;

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItem"/> class.
		/// </summary>
        public MenuItem()
            : base()
        {
            this.DefaultStyleKey = typeof(MenuItem);
            _isActive = false;
        }

		/// <summary>
		/// Called before the PointerEntered event occurs.
		/// </summary>
		/// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isActive = true;
            UpdateState(true);
            base.OnPointerEntered(e);
            Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

		/// <summary>
		/// Called before the PointerExited event occurs.
		/// </summary>
		/// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            _isActive = false;
            UpdateState(true);
        }

		/// <summary>
		/// Called before the PointerMoved event occurs.
		/// </summary>
		/// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isActive = true;
            UpdateState(true);
            base.OnPointerMoved(e);
            Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

		/// <summary>
		/// Called before the GotFocus event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isActive = true;
            UpdateState(true);
        }

		/// <summary>
		/// Called before the LostFocus event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isActive = false;
            UpdateState(true);
        }

		/// <summary>
		/// Called before the KeyDown event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Space)
            {
                // Issue #84: https://github.com/timheuer/callisto/issues/84
                OnTapped(new TappedRoutedEventArgs());
            }
        }

		/// <summary>
		/// Called before the Tapped event occurs.
		/// </summary>
		/// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            VisualStateManager.GoToState(this, StateBase, true);
        }

		/// <summary>
		/// Called before the PointerReleased event occurs.
		/// </summary>
		/// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            VisualStateManager.GoToState(this, StateBase, true);
        }

		/// <summary>
		/// Called before the PointerPressed event occurs.
		/// </summary>
		/// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            VisualStateManager.GoToState(this, StatePressed, true);
        }

        private void UpdateState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, StateDisabled, useTransitions);
            }
            else
            {
                if (_isActive)
                {
                    VisualStateManager.GoToState(this, StateHover, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, StateBase, useTransitions);
                }
            }
        }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

		/// <summary>
		/// Gets or sets the command.
		/// </summary>
		public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

		/// <summary>
		/// Gets or sets the command parameter.
		/// </summary>
		public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
    }
}
