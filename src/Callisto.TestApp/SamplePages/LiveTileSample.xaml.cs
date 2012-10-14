using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Callisto.TestApp.SamplePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LiveTileSample : Page
	{
		public class LiveTileData
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public Uri ImageUri { get; set; }
			public Uri ReadMoreUri { get; set; }
		}

		public LiveTileSample()
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
			List<LiveTileData> tileData = new List<LiveTileData>();
			tileData.Add(new LiveTileData()
			{
				Name = "Butterfly",
				Description = "A butterfly is a mainly day-flying insect of the order Lepidoptera, which includes the butterflies and moths. Like other holometabolous insects, the butterfly's life cycle consists of four parts: egg, larva, pupa and adult.",
				ImageUri = new Uri("ms-appx:/Images/butterfly.jpg"),
				ReadMoreUri = new Uri("http://en.wikipedia.org/wiki/Butterfly")
			});
            tileData.Add(new LiveTileData()
            {
                Name = "Grasshopper",
                Description = "The grasshopper is an insect of the suborder Caelifera in the order Orthoptera. To distinguish it from bush crickets or katydids, it is sometimes referred to as the short-horned grasshopper. Species that change colour and behaviour at high population densities are called locusts.",
                ImageUri = new Uri("ms-appx:/Images/grasshopper.jpg"),
                ReadMoreUri = new Uri("http://en.wikipedia.org/wiki/Grasshopper")
            });
			DataContext = tileData;
		}
		private async void OnReadMoreLink_Click(object sender, RoutedEventArgs e)
		{
			var link = (sender as FrameworkElement).Tag as Uri;
			if (link != null)
			{
				await Windows.System.Launcher.LaunchUriAsync(link);
			}
		}
	}
}
