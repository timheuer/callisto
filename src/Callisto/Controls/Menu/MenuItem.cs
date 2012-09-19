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

using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Callisto.Controls
{
    [TemplateVisualState(Name = MenuItem.StateBase, GroupName = MenuItem.GroupCommon)]
    [TemplateVisualState(Name = MenuItem.StateHover, GroupName = MenuItem.GroupCommon)]
    [TemplateVisualState(Name = MenuItem.StatePressed, GroupName = MenuItem.GroupCommon)]
    [TemplateVisualState(Name = MenuItem.StateDisabled, GroupName = MenuItem.GroupCommon)]
    public class MenuItem : MenuItemBase
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MenuItem), null);
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuItem), null);
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuItem), null);

        private const string GroupCommon = "Common";
        private const string StateBase = "Base";
        private const string StateHover = "Hover";
        private const string StatePressed = "Pressed";
        private const string StateDisabled = "Disabled";

        private bool _isActive;

        public MenuItem()
            : base()
        {
            this.DefaultStyleKey = typeof(MenuItem);
            _isActive = false;
        }

        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isActive = true;
            UpdateState(true);
            base.OnPointerEntered(e);
            Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            _isActive = false;
            UpdateState(true);
        }

        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isActive = true;
            UpdateState(true);
            base.OnPointerMoved(e);
            Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isActive = true;
            UpdateState(true);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isActive = false;
            UpdateState(true);
        }

        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Space)
            {
                // Issue #84: https://github.com/timheuer/callisto/issues/84
                OnTapped(new TappedRoutedEventArgs());
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            VisualStateManager.GoToState(this, StateBase, true);
        }

        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            VisualStateManager.GoToState(this, StateBase, true);
        }

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

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
    }
}
