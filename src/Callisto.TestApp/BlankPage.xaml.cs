using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Callisto.Controls;
using UIElementLeakTester;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
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

namespace XamlControlsUITestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage : Page
    {
        public BlankPage()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += BlankPage_CommandsRequested;
        }

        void BlankPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand("sample", "Sample Custom Setting", (x) =>
            {
                SettingsFlyout settings = new SettingsFlyout();
                settings.FlyoutWidth = (Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth)Enum.Parse(typeof(Callisto.Controls.SettingsFlyout.SettingsFlyoutWidth), settingswidth.SelectionBoxItem.ToString());
                settings.HeaderBrush = new SolidColorBrush(Colors.Orange);
                settings.HeaderText = "Foo Bar Setting";

                BitmapImage bmp = new BitmapImage(new Uri(this.BaseUri, "Assets/SmallLogo.png"));

                settings.SmallLogoImageSource = bmp;

                ToggleSwitch ts = new ToggleSwitch();
                ts.Header = "Download updates automatically";

                settings.Content = ts;

                settings.IsOpen = true;

                ObjectTracker.Track(settings);
            });

            args.Request.ApplicationCommands.Add(cmd);

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            positioning.SelectedIndex = 0;
            settingswidth.SelectedIndex = 0;

            //DateTimeWrapper dtw = new DateTimeWrapper(DateTime.Now);
            DateFormatted.Text = Windows.Globalization.DateTimeFormatting.DateTimeFormatter.ShortDate.Format(DateTimeOffset.Now); //dtw.HourNumber.ToString();
            DateCalculated.DataContext = DateTime.Now.AddHours(-2); // new DateTime(2012, 1, 12, 13, 13, 5);

            //MyTimePicker.Value = DateTime.Now;
        }

        private void LogEvent(string msg)
        {
            LogOutput.Text += System.Environment.NewLine + msg;
            Debug.WriteLine(msg);
        }

        private void ShowFlyoutMenu(object sender, RoutedEventArgs e)
        {
            Flyout f = new Flyout();
            //f.FlowDirection = Windows.UI.Xaml.FlowDirection.RightToLeft;
            f.PlacementTarget = sender as UIElement;
            f.Placement = PlacementMode.Top;
            f.Closed += (x, y) =>
            {
                LogEvent("Event: Closed");
            };

            Menu menu = new Menu();
            //menu.Placement = PlacementMode.Top;
            //menu.PlacementTarget = sender as UIElement;
            //menu.Closed += (x, y) =>
            //    {
            //        LogEvent("Event: Closed");
            //    };

            MenuItem mi = new MenuItem();
            mi.Tag = "Easy";
            mi.Tapped += ItemClicked;
            mi.Text = "Easy Game";

            MenuItem mi2 = new MenuItem();
            mi2.Text = "Medium Game";
            mi2.Tag = "Medium";
            mi2.Tapped += ItemClicked;

            MenuItem mi3 = new MenuItem();
            mi3.Text = "Hard Game";
            mi3.Command = new CommandTest();
            mi3.CommandParameter = "test param from command";

            menu.Items.Add(mi);
            menu.Items.Add(mi2);
            menu.Items.Add(new MenuItemSeparator());
            menu.Items.Add(new MenuItem() { Text = "Foobar something really long", Tag = "Long menu option" });
            menu.Items.Add(new MenuItemSeparator());
            menu.Items.Add(mi3);


            //GeneralTransform gt = ((UIElement)VisualTreeHelper.GetParent(((UIElement)sender))).TransformToVisual(LayoutRoot);
            //Point topLeft = gt.TransformPoint(new Point(0, 0));

            //var vert = topLeft.Y - menu.Height;
            //if (vert < 0)
            //{
            //    vert = ((UIElement)VisualTreeHelper.GetParent(((UIElement)sender))).RenderSize.Height;
            //}
            //var hor = topLeft.X + (((UIElement)sender).RenderSize.Width / 2) - (menu.Width / 2);

            //menu.HorizontalOffset = 0;// hor;
            //menu.VerticalOffset = -10;// vert;
            f.Content = menu;
            f.IsOpen = true;

            ObjectTracker.Track(f);

            UpdateLayout();
        }

        private void ItemClicked(object sender, TappedRoutedEventArgs args)
        {
            MenuItem mi = sender as MenuItem;
            LogEvent(string.Format("Event: Tapped; Argument: {0}", mi.Tag.ToString()));
        }

        private void ShowFlyoutMenu2(object sender, RoutedEventArgs e)
        {
            Flyout f = new Flyout();
            
            Border b = new Border();
            b.Width = 300;
            b.Height = 125;

            TextBlock tb = new TextBlock();
            tb.FontSize = 24.667;
            tb.Text = "Hello";

            b.Child = tb;

            f.Content = b;

            f.Placement = (PlacementMode)Enum.Parse(typeof(PlacementMode), positioning.SelectionBoxItem.ToString());
            f.PlacementTarget = sender as UIElement;

            f.IsOpen = true;


            ObjectTracker.Track(f);

            //middlebutton.Width = 500;
            //middlebutton.Height = 200;

        }

        private void ShowFlyoutMenu3(object sender, RoutedEventArgs e)
        {
            Flyout f = new Flyout();

            f.Margin = new Thickness(20, 12, 20, 20);
            f.VerticalOffset = -12;
            f.HorizontalOffset = -124;
            f.Content = new SampleInput();
            f.Placement = PlacementMode.Top;
            f.PlacementTarget = sender as UIElement;

            LayoutRoot.Children.Add(f.HostPopup);

            f.Closed += (b, c) =>
                {
                    LayoutRoot.Children.Remove(f.HostPopup);
                };

            f.IsOpen = true;

            ObjectTracker.Track(f);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ObjectTracker.GarbageCollect(LogOutput);
        }

        private void ForceGC(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();
        }
    }

    public class CommandTest : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Debug.WriteLine(string.Format("Event: Command.Execute; Argument: {0}", parameter.ToString()));
        }
    }
}
