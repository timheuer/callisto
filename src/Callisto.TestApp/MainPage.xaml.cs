using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class DebugWriteCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Debug.WriteLine(string.Format("DebugWriteCommand Fired: {0}", parameter.ToString()));
        }
    }

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
            Loaded += MainPage_Loaded;
		}

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoginDialog.BackButtonCommand = new DebugWriteCommand();
            LoginDialog.BackButtonCommandParameter = "Hello world";
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
                Samples.Add(new SamplePage() { Name = "DynamicTextBlock", Page = typeof(SamplePages.DynamicTextBlock) });
                Samples.Add(new SamplePage() { Name = "WebViewExtension", Page = typeof(SamplePages.WebViewExtension) });
                Samples.Add(new SamplePage() { Name = "FlipViewIndicator", Page = typeof(SamplePages.FlipViewIndicatorSample) });
                Samples.Add(new SamplePage() { Name = "WatermarkTextBox", Page = typeof(SamplePages.WatermarkTextBoxSample) });
                Samples.Add(new SamplePage() { Name = "NumericUpDown", Page = typeof(SamplePages.NumericUpDownSample) });
			    Samples.Add(new SamplePage() { Name = "CustomDialog", Page = typeof(SamplePages.CustomDialogSample) });
                Samples.Add(new SamplePage() { Name = "DropDownButton", Page = typeof(SamplePages.DropDownButtonSample) });
                Samples.Add(new SamplePage() { Name = "WrapPanel", Page = typeof(SamplePages.WrapPanelSample) });
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

        private void DialogCancelClicked(object sender, RoutedEventArgs e)
        {
            LoginDialog.IsOpen = false;
        }

        private void LoginDialog_BackButtonClicked_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Dialog back button clicked");
            LoginDialog.IsOpen = false;
        }
	}
}
