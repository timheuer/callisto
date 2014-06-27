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
	/// <summary>
	/// CustomDialog is a control intended to be used full-screen and to mimic the MessageDialog behavior.
	/// Currently in WinRT, the MessageDialog is limited to text/button content only. If that is your
	/// scenario, you should absolutely use that API. CustomDialog exists to serve the more advanced
	/// scenario of needing richer content in your UI modal dialog
	/// This is a "UI modal" dialog meaning that it is intended to block UI interaction until the dialog 
	/// is dismissed. It does not create true modal behavior, so actions in the background could still
	/// be executing.
	/// </summary>
	/// <remarks>
	/// CustomDialog is a ContentControl. The properties on the control itself that you want to be aware of are
	/// <see cref="Title"/> (required), <see cref="Control.Background"/>, and <see cref="BackButtonVisibility  "/>
	/// to set to your desired behavior.
	/// </remarks>
    [TemplatePart(Name = CustomDialog.PART_BACK_BUTTON, Type = typeof(Button))]
    [TemplatePart(Name = CustomDialog.PART_ROOT_BORDER, Type = typeof(Border))]
    [TemplatePart(Name = CustomDialog.PART_ROOT_GRID, Type = typeof(Grid))]
    [TemplatePart(Name = CustomDialog.PART_CONTENT, Type = typeof(ContentPresenter))]
    public class CustomDialog : ContentControl
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomDialog"/> class.
		/// </summary>
        public CustomDialog()
        {
            DefaultStyleKey = typeof(CustomDialog);
        }

		/// <summary>
		/// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
		/// call ApplyTemplate. In simplest terms, this means the method is called just before a UI
		/// element displays in your app. Override this method to influence the default post-template logic of a class.
		/// </summary>
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
		/// <summary>
		/// Occurs when the back button was clicked.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the back button visibility.
		/// </summary>
		public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="BackButtonVisibility"/> dependency property
		/// </summary>
		public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(CustomDialog), new PropertyMetadata(Visibility.Collapsed));

		/// <summary>
		/// Gets or sets a value indicating whether the dialog is open.
		/// </summary>
		/// <value>
		///   <c>true</c> if this dialog is open; otherwise, <c>false</c>.
		/// </value>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="IsOpen"/> dependency property
		/// </summary>
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

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="Title"/> dependency property
		/// </summary>
		public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomDialog), null);

		/// <summary>
		/// Gets or sets the back button command.
		/// </summary>
        public ICommand BackButtonCommand
        {
            get { return (ICommand)GetValue(BackButtonCommandProperty); }
            set { SetValue(BackButtonCommandProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="BackButtonCommand"/> dependency property
		/// </summary>
		public static readonly DependencyProperty BackButtonCommandProperty =
            DependencyProperty.Register("BackButtonCommand", typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(DependencyProperty.UnsetValue));

		/// <summary>
		/// Gets or sets the back button command parameter.
		/// </summary>
		public object BackButtonCommandParameter
        {
            get { return (object)GetValue(BackButtonCommandParameterProperty); }
            set { SetValue(BackButtonCommandParameterProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="BackButtonCommandParameter"/> dependency property
		/// </summary>
		public static readonly DependencyProperty BackButtonCommandParameterProperty =
            DependencyProperty.Register("BackButtonCommandParameter", typeof(object), typeof(CustomDialog), new PropertyMetadata(DependencyProperty.UnsetValue));
        #endregion
    }
}
