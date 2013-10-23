using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Callisto.Controls;
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
using XamlControlsUITestApp;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class FlyoutSample : Page
	{
        bool chk = false;

		public FlyoutSample()
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
			positioning.SelectedIndex = 0;
		}

		private void LogEvent(string msg)
		{
			LogOutput.Text += System.Environment.NewLine + msg;
			Debug.WriteLine(msg);
		}

		private void ItemClicked(object sender, TappedRoutedEventArgs args)
		{
			MenuItem mi = sender as MenuItem;
			LogEvent(string.Format("Event: Tapped; Argument: {0}", mi.Tag.ToString()));
		}

		private void ShowFlyoutMenu(object sender, RoutedEventArgs e)
		{
			Callisto.Controls.Flyout f = new Callisto.Controls.Flyout();
			f.PlacementTarget = sender as UIElement;
			f.Placement = PlacementMode.Top;
			f.Closed += (x, y) =>
			{
				LogEvent("Event: Closed");
			};

			Menu menu = new Menu();

			MenuItem mi = new MenuItem();
			mi.Tag = "Easy";
			mi.Tapped += ItemClicked;
			mi.Text = "Easy Game";
            mi.MenuTextMargin = new Thickness(28, 10, 28, 12);

			MenuItem mi2 = new MenuItem();
			mi2.Text = "Medium Game";
			mi2.Tag = "Medium";
			mi2.Tapped += ItemClicked;
            mi2.MenuTextMargin = new Thickness(28, 10, 28, 12);

			MenuItem mi3 = new MenuItem();
			mi3.Text = "Hard Game";
			mi3.Command = new CommandTest();
			mi3.CommandParameter = "test param from command";
            mi3.MenuTextMargin = new Thickness(28, 10, 28, 12);

            ToggleMenuItem tmi = new ToggleMenuItem();
            tmi.Text = "Enable Logging";
            tmi.IsChecked = chk;
            tmi.Tapped += (a, b) =>
                {
                    chk = !chk;
                };

			menu.Items.Add(mi);
			menu.Items.Add(mi2);
			menu.Items.Add(new MenuItemSeparator());
			menu.Items.Add(new MenuItem() { Text = "Foobar something really long", Tag = "Long menu option", MenuTextMargin = new Thickness(28,10,28,12) });
            menu.Items.Add(tmi);
			menu.Items.Add(new MenuItemSeparator());
			menu.Items.Add(mi3);
            f.HostMargin = new Thickness(0); // on menu flyouts set HostMargin to 0
			f.Content = menu;
			f.IsOpen = true;

			ObjectTracker.Track(f);

			UpdateLayout();
		}

		private void ShowFlyoutMenu2(object sender, RoutedEventArgs e)
		{
			Callisto.Controls.Flyout f = new Callisto.Controls.Flyout();

			Border b = new Border();
			b.Width = 300;
			b.Height = 125;

			TextBlock tb = new TextBlock();
            tb.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            tb.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            tb.TextWrapping = TextWrapping.Wrap;
			tb.FontSize = 24.667;
			tb.Text = "This is a basic ContentControl so put anything you want in here.";

			b.Child = tb;

			f.Content = b;

			f.Placement = (PlacementMode)Enum.Parse(typeof(PlacementMode), positioning.SelectionBoxItem.ToString());
			f.PlacementTarget = sender as UIElement;
            
			f.IsOpen = true;

			ObjectTracker.Track(f);
		}


		private void ShowFlyoutMenu3(object sender, RoutedEventArgs e)
		{
			Callisto.Controls.Flyout f = new Callisto.Controls.Flyout();

			f.Margin = new Thickness(20, 12, 20, 12);
			f.Content = new SampleInput();
			f.Placement = PlacementMode.Top;
			f.PlacementTarget = sender as UIElement;

            var parentGrid = ((FrameworkElement)this.Parent).FindName("FrameLayoutRoot") as Grid;

            parentGrid.Children.Add(f.HostPopup);

            f.Closed += (b, c) =>
            {
                parentGrid.Children.Remove(f.HostPopup);
            };

			f.IsOpen = true;

			ObjectTracker.Track(f);
		}



		public class CommandTest : ICommand
		{
			public bool CanExecute(object parameter)
			{
				return true;
			}

#pragma warning disable 67 //CanExecute never changes, but event is required by ICommand.
			public event EventHandler CanExecuteChanged;
#pragma warning restore 67

			public void Execute(object parameter)
			{
				Debug.WriteLine(string.Format("Event: Command.Execute; Argument: {0}", parameter.ToString()));
			}
		}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ObjectTracker.GarbageCollect();
        }

        private void ShowFlyoutToggleMenu(object sender, RoutedEventArgs e)
        {
            Callisto.Controls.Flyout f = new Callisto.Controls.Flyout();
            f.PlacementTarget = sender as UIElement;
            f.Placement = PlacementMode.Top;
            f.Closed += (x, y) =>
            {
                LogEvent("Event: Closed");
            };

            Menu menu = new Menu();


            ToggleMenuItem tmi1 = new ToggleMenuItem();
            tmi1.Text = "Enable Logging";
            tmi1.IsChecked = chk;
            tmi1.Tapped += (a, b) =>
            {
                chk = !chk;
            };

            ToggleMenuItem tmi2 = new ToggleMenuItem();
            tmi2.Text = "Disable Logging";
            tmi2.IsChecked = !chk;
            tmi2.Tapped += (a, b) =>
            {
                chk = !chk;
            };

            menu.Items.Add(tmi1);
            menu.Items.Add(tmi2);
            f.HostMargin = new Thickness(0); // on menu flyouts set HostMargin to 0
            f.Content = menu;
            f.IsOpen = true;

            ObjectTracker.Track(f);

            UpdateLayout();
        }
	}
}
