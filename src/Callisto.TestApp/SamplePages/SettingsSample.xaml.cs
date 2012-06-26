using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Callisto.Controls;
using UIElementLeakTester;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
			SettingsCommand cmd = new SettingsCommand("sample", "Sample Custom Setting", (x) =>
			{
				SettingsFlyout settings = new SettingsFlyout();
				settings.FlyoutWidth = (Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth)Enum.Parse(typeof(Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth), settingswidth.SelectionBoxItem.ToString());
                //settings.HeaderBrush = new SolidColorBrush(Colors.Orange);
                //settings.Background = new SolidColorBrush(Colors.Red);
				settings.HeaderText = "Foo Bar Custom Settings";

                BitmapImage bmp = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"));

                settings.SmallLogoImageSource = bmp;

                StackPanel sp = new StackPanel();

				ToggleSwitch ts = new ToggleSwitch();
				ts.Header = "Download updates automatically";

                Button b = new Button();
                b.Content = "Test";

                sp.Children.Add(ts);
                sp.Children.Add(b);

				settings.Content = sp;

				settings.IsOpen = true;

				ObjectTracker.Track(settings);
			});

			args.Request.ApplicationCommands.Add(cmd);
		}

		private void ShowSettings(object sender, RoutedEventArgs e)
		{
			SettingsPane.Show();
		}
	}
}
