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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
    public sealed class CustomDialog : ContentControl
    {
        public CustomDialog()
        {
            DefaultStyleKey = typeof(CustomDialog);
        }

        protected override void OnApplyTemplate()
        {
            _rootGrid = (Grid)GetTemplateChild(PART_ROOT_GRID);
            _rootBorder = (Border)GetTemplateChild(PART_ROOT_BORDER);
            ResizeContainers();

            // TODO: Need to detach this event?
            Window.Current.SizeChanged += OnWindowSizeChanged;

            base.OnApplyTemplate();
        }

        private void ResizeContainers()
        {
            if (_rootGrid != null)
            {
                _rootGrid.Width = Window.Current.Bounds.Width;
                _rootGrid.Height = Window.Current.Bounds.Height;
            }

            if (_rootBorder != null) _rootBorder.Width = Window.Current.Bounds.Width;
        }

        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            ResizeContainers();
        }

        #region Member Variables
        private Grid _rootGrid;
        private Border _rootBorder;
        #endregion

        #region Constants
        private const string PART_ROOT_BORDER = "PART_RootBorder";
        private const string PART_ROOT_GRID = "PART_RootGrid";
        #endregion

        #region Dependency Properties
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CustomDialog), new PropertyMetadata(false, OnIsOpenPropertyChanged));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CustomDialog dlg = d as CustomDialog;
                if (dlg != null)
                {
                    dlg.ApplyTemplate();
                }
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomDialog), null);
        #endregion
    }
}
