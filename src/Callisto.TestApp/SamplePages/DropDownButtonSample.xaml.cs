using Callisto.Controls;
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
    public sealed partial class DropDownButtonSample : Page
    {
        public DropDownButtonSample()
        {
            this.InitializeComponent();
        }

        public void FilesButtonClicked(object sender, RoutedEventArgs args)
        {
            Flyout f = new Flyout();
            f.PlacementTarget = sender as UIElement;
            f.Placement = PlacementMode.Bottom;

            Menu menu = new Menu();

            MenuItem mi = new MenuItem();
            mi.Text = "Documents";

            MenuItem mi2 = new MenuItem();
            mi2.Text = "Pictures";

            MenuItem mi3 = new MenuItem();
            mi3.Text = "Music";

            MenuItem mi4 = new MenuItem();
            mi4.Text = "Network Directory";

            menu.Items.Add(mi);
            menu.Items.Add(mi2);
            menu.Items.Add(mi3);
            menu.Items.Add(mi4);

            //f.HostMargin = new Thickness(0); // on menu flyouts set HostMargin to 0
            f.HostMargin = new Thickness(-210, 0, 0, 0);
            f.Content = menu;
            f.IsOpen = true;
        }
    }
}
