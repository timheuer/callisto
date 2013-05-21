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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
    [TemplatePart(Name = CustomDialog.PART_BACK_BUTTON, Type = typeof(Button))]
    [TemplatePart(Name = CustomDialog.PART_ROOT_BORDER, Type = typeof(Border))]
    [TemplatePart(Name = CustomDialog.PART_ROOT_GRID, Type = typeof(Grid))]
    [TemplatePart(Name = CustomDialog.PART_CONTENT, Type = typeof(ContentPresenter))]
    public class CustomDialog : ContentControl
    {
        public CustomDialog()
        {
            DefaultStyleKey = typeof(CustomDialog);
        }

        protected override void OnApplyTemplate()
        {
            _rootGrid = (Grid)GetTemplateChild(PART_ROOT_GRID);
            _rootBorder = (Border)GetTemplateChild(PART_ROOT_BORDER);
            _backButton = (Button)GetTemplateChild(PART_BACK_BUTTON);

            ResizeContainers();

            if (_backButton != null)
            {
                _backButton.Click += (bbs, bba) =>
                    {
                        if (BackButtonClicked != null)
                        {
                            BackButtonClicked(bbs, bba);
                        }
                        else
                        {
                            IsOpen = false;
                        }
                    };
            }

            Window.Current.SizeChanged += OnWindowSizeChanged;
            Unloaded += OnUnloaded;

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

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Unloaded -= OnUnloaded;
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        #region Events
        public event RoutedEventHandler BackButtonClicked;
        #endregion

        #region Member Variables
        private Grid _rootGrid;
        private Border _rootBorder;
        private Button _backButton;
        #endregion

        #region Constants
        private const string PART_ROOT_BORDER = "PART_RootBorder";
        private const string PART_ROOT_GRID = "PART_RootGrid";
        private const string PART_BACK_BUTTON = "PART_BackButton";
        private const string PART_CONTENT = "PART_Content";
        #endregion

        #region Dependency Properties
        public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(CustomDialog), new PropertyMetadata(Visibility.Collapsed));

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

        public ICommand BackButtonCommand
        {
            get { return (ICommand)GetValue(BackButtonCommandProperty); }
            set { SetValue(BackButtonCommandProperty, value); }
        }

        public static readonly DependencyProperty BackButtonCommandProperty =
            DependencyProperty.Register("BackButtonCommand", typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(DependencyProperty.UnsetValue));

        public object BackButtonCommandParameter
        {
            get { return (object)GetValue(BackButtonCommandParameterProperty); }
            set { SetValue(BackButtonCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty BackButtonCommandParameterProperty =
            DependencyProperty.Register("BackButtonCommandParameter", typeof(object), typeof(CustomDialog), new PropertyMetadata(DependencyProperty.UnsetValue));
        #endregion
    }
}
