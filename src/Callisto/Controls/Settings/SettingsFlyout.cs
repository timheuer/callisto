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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Callisto.Controls
{
    public sealed class SettingsFlyout : ContentControl
    {
        #region Member Variables
        private Popup _hostPopup;
        private Rect _windowBounds;
        private double _settingsWidth;
        private Button _backButton;
        private Grid _contentGrid;
        private Border _rootBorder;
        private ScrollViewer _contentScrollViewer;
        const int CONTENT_HORIZONTAL_OFFSET = 100;
        #endregion Member Variables

        #region Overrides
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // make sure we listen at the right time to add/remove the back button event handlers
            if(_backButton != null)
            {
                _backButton.Tapped -= OnBackButtonTapped;
            }
            _backButton = GetTemplateChild("SettingsBackButton") as Button;
            if(_backButton != null)
            {
                _backButton.Tapped += OnBackButtonTapped;
            }

            // need to get these grids in order to set the offsets correctly in RTL situations
            if (_contentGrid == null)
            {
                _contentGrid = GetTemplateChild("SettingsFlyoutContentGrid") as Grid;
            }
            _contentGrid.Transitions = new TransitionCollection();
            _contentGrid.Transitions.Add(new EntranceThemeTransition()
            {
                FromHorizontalOffset = (SettingsPane.Edge == SettingsEdgeLocation.Right) ? CONTENT_HORIZONTAL_OFFSET : (CONTENT_HORIZONTAL_OFFSET * -1)
            });

            // need the root border for RTL scenarios
            _rootBorder = GetTemplateChild("PART_RootBorder") as Border;

            // need the content scrollviewer to set the fixed width to be the same size as flyout
            _contentScrollViewer = GetTemplateChild("PART_ContentScrollViewer") as ScrollViewer;
            
        }
        #endregion Overrides

        #region Constructor
        public SettingsFlyout()
        {
            this.DefaultStyleKey = typeof(SettingsFlyout);

            _windowBounds = Window.Current.Bounds;

            this.Loaded += OnLoaded;

            _hostPopup = new Popup();
            _hostPopup.ChildTransitions = new TransitionCollection();
            _hostPopup.ChildTransitions.Add(new PaneThemeTransition() { Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ? EdgeTransitionLocation.Right : EdgeTransitionLocation.Left });
            _hostPopup.Closed += OnHostPopupClosed;
            _hostPopup.IsLightDismissEnabled = true;
            _hostPopup.Height = _windowBounds.Height;
            _hostPopup.Child = this;
            _hostPopup.SetValue(Canvas.TopProperty, 0);

            this.Height = _windowBounds.Height;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.Activated += OnCurrentWindowActivated;

            // in RTL languages on the OS, the SettingsPane comes from the left edge
            if (SettingsPane.Edge == SettingsEdgeLocation.Left)
            {
                _rootBorder.BorderThickness = new Thickness(0, 0, 1, 0);
            }

            _settingsWidth = (double)this.FlyoutWidth;
            
            // setting all the widths to be the size of flyout
            _hostPopup.Width = this.Width = _contentScrollViewer.Width = _settingsWidth;
            
            // ensure it comes from the correct edge location
            _hostPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (_windowBounds.Width - _settingsWidth) : 0);
        }
        
        private void OnBackButtonTapped(object sender, object e)
        {
            // BUG #47: need to map back button to custom and ability to disable

            if (_hostPopup != null)
            {
                _hostPopup.IsOpen = false;
            }

            // TEMP: wrapping this to ensure back button doesn't happen in snap/portrait
            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
            
        }
        
        void OnHostPopupClosed(object sender, object e)
        {
            _hostPopup.Child = null;
            Window.Current.Activated -= OnCurrentWindowActivated;
            this.Content = null;

            if (null != Closed)
            {
                Closed(this, e);
            }
        }

        void OnCurrentWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                this.IsOpen = false;
            }
        }
        #endregion Constructor

        #region Dependency Properties
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SettingsFlyout), new PropertyMetadata(false, (obj, args) =>
                {
                    if (args.NewValue != args.OldValue)
                    {
                        SettingsFlyout f = (SettingsFlyout)obj;
                        f._hostPopup.IsOpen = (bool)args.NewValue;
                    }
                }));

        public SolidColorBrush HeaderBrush
        {
            get { return (SolidColorBrush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderBrushProperty =
            DependencyProperty.Register("HeaderBrush", typeof(SolidColorBrush), typeof(SettingsFlyout), null);

        public SettingsFlyoutWidth FlyoutWidth
        {
            get { return (SettingsFlyoutWidth)GetValue(FlyoutWidthProperty); }
            set { SetValue(FlyoutWidthProperty, value); }
        }

        public static readonly DependencyProperty FlyoutWidthProperty =
            DependencyProperty.Register("FlyoutWidth", typeof(SettingsFlyoutWidth), typeof(SettingsFlyout), new PropertyMetadata(SettingsFlyoutWidth.Narrow));


        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(SettingsFlyout), new PropertyMetadata(null));


        public ImageSource SmallLogoImageSource
        {
            get { return (ImageSource)GetValue(SmallLogoImageSourceProperty); }
            set { SetValue(SmallLogoImageSourceProperty, value); }
        }

        public static readonly DependencyProperty SmallLogoImageSourceProperty =
            DependencyProperty.Register("SmallLogoImageSource", typeof(ImageSource), typeof(SettingsFlyout), null);
        
        #endregion Dependency Properties

        #region Events
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification="Runtime EventHandler")]
        public event EventHandler<object> Closed;
        #endregion

        #region Enums
        public enum SettingsFlyoutWidth
        {
            Narrow = 346,
            Wide = 646
        }
        #endregion Enums
    }
}
