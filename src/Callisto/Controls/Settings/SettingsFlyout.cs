using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    public sealed class SettingsFlyout : ContentControl
    {
        #region Member Variables
        private Popup _hostPopup;
        private Rect _windowBounds;
        private Button _backButton;
        #endregion Member Variables

        #region Overrides
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _backButton = GetTemplateChild("SettingsBackButton") as Button;

            _backButton.Tapped += (x, y) =>
            {
                if (_hostPopup != null)
                {
                    _hostPopup.IsOpen = false;
                }
                SettingsPane.Show();
            };
        }
        #endregion Overrides

        #region Constructor
        public SettingsFlyout()
        {
            this.DefaultStyleKey = typeof(SettingsFlyout);

            Window.Current.Activated += OnCurrentWindowActivated;

            _windowBounds = Window.Current.Bounds;

            this.Loaded += OnLoaded;

            _hostPopup = new Popup();
            _hostPopup.Closed += OnHostPopupClosed;
            _hostPopup.IsLightDismissEnabled = true;
            _hostPopup.Height = _windowBounds.Height;
            _hostPopup.Child = this;
            _hostPopup.SetValue(Canvas.TopProperty, 0);

            this.Height = _windowBounds.Height;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _hostPopup.Width = (this.FlyoutWidth == SettingsFlyoutWidth.Wide) ? 646 : 346;
            this.Width = _hostPopup.Width;
            _hostPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - (double)this.FlyoutWidth);
        }

        void OnHostPopupClosed(object sender, object e)
        {
            _hostPopup.Child = null;
            Window.Current.Activated -= OnCurrentWindowActivated;
            this.Content = null;
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

        #region Enums
        public enum SettingsFlyoutWidth
        {
            Narrow = 346,
            Wide = 646
        }
        #endregion Enums
    }
}
