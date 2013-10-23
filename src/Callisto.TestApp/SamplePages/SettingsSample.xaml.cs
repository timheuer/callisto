using System;
using Callisto.Controls;
using UIElementLeakTester;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XamlControlsUITestApp;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsSample : Page
	{
		public SettingsSample()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			settingswidth.SelectedIndex = 0;
			SettingsPane.GetForCurrentView().CommandsRequested += BlankPage_CommandsRequested;
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			SettingsPane.GetForCurrentView().CommandsRequested -= BlankPage_CommandsRequested;
			base.OnNavigatingFrom(e);
		}

		private void BlankPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
		{
            // Note this is kept here for posterity but shouldn't be used
            // see the next sample of using the Windows.UI.Xaml.Controls.SettingsFlyout
            // in Windows 8.1 for the replacement if you were using this code example before.
			SettingsCommand cmd = new SettingsCommand("sample", "Sample Custom Setting", (x) =>
			{
                // create a new instance of the flyout
				Callisto.Controls.SettingsFlyout settings = new Callisto.Controls.SettingsFlyout();
                // set the desired width.  If you leave this out, you will get Narrow (346px)
				settings.FlyoutWidth = (Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth)Enum.Parse(typeof(Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth), settingswidth.SelectionBoxItem.ToString());
                
                // optionally change header and content background colors away from defaults (recommended)
                // if using Callisto's AppManifestHelper you can grab the element from some member var you held it in
                // settings.HeaderBrush = new SolidColorBrush(App.VisualElements.BackgroundColor);
                settings.HeaderBrush = new SolidColorBrush(Colors.Orange);
                settings.HeaderText = string.Format("{0} Custom Settings", App.VisualElements.DisplayName);

                // provide some logo (preferrably the smallogo the app uses)
                BitmapImage bmp = new BitmapImage(App.VisualElements.SmallLogoUri);
                settings.SmallLogoImageSource = bmp;

                // set the content for the flyout
                settings.Content = new SettingsContent();

                // open it
				settings.IsOpen = true;

                // this is only for the test app and not needed
                // you would not use this code in your real app
				ObjectTracker.Track(settings);
			});

            // note this is the new and preferred way in Windows 8.1 using 
            // Windows.UI.Xaml.Controls.SettingsFlyout
            // This sample is kept here to show an example of this
            SettingsCommand cmd2 = new SettingsCommand("sample2", "Sample 2", (x) =>
            {
                Windows.UI.Xaml.Controls.SettingsFlyout settings = new Windows.UI.Xaml.Controls.SettingsFlyout();
                settings.Width = 500;
                settings.HeaderBackground = new SolidColorBrush(Colors.Orange);
                settings.HeaderForeground = new SolidColorBrush(Colors.Black);
                settings.Title = string.Format("{0} Custom 2", App.VisualElements.DisplayName);
                settings.IconSource = new BitmapImage(Windows.ApplicationModel.Package.Current.Logo);
                settings.Content = new SettingsContent();
                settings.Show();
                ObjectTracker.Track(settings);
            });

			args.Request.ApplicationCommands.Add(cmd);
            args.Request.ApplicationCommands.Add(cmd2);
		}

		private void ShowSettings(object sender, RoutedEventArgs e)
		{
			SettingsPane.Show();
		}
	}
}
