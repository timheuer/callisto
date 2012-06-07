using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using UIElementLeakTester;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public sealed class SamplePage
		{
			public string Name { get; set; }
			public Type Page { get; set; }
		}
		private List<SamplePage> Samples;

		public MainPage()
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
			if (Samples == null)
			{
				Samples = new List<SamplePage>();
				Samples.Add(new SamplePage() { Name = "Flyout", Page = typeof(SamplePages.FlyoutSample) });
				Samples.Add(new SamplePage() { Name = "Settings", Page = typeof(SamplePages.SettingsSample) });
				Samples.Add(new SamplePage() { Name = "Tilt Effect", Page = typeof(SamplePages.TiltSample) });
				Samples.Add(new SamplePage() { Name = "LiveTile", Page = typeof(SamplePages.LiveTileSample) });
                Samples.Add(new SamplePage() { Name = "Rating", Page = typeof(SamplePages.RatingControl) });
				this.DataContext = Samples;
			}
		}

		private void SampleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var pagetype = ((sender as ListBox).SelectedItem as SamplePage).Page;
			MainFrame.Navigate(pagetype);
		}

        private void Image_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            ObjectTracker.GarbageCollect();
        }
	}
}
