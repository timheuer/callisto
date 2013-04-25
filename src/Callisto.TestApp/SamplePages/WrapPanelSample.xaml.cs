using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class WrapPanelSample : Page
    {
        public WrapPanelSample()
        {
            this.InitializeComponent();
        }

        Random rnd = new Random();

        private void AddItemsToWrapPanel(object sender, RoutedEventArgs e)
        {
            int numItems = Int32.Parse((string)((FrameworkElement)sender).Tag);

            while (numItems-- > 0)
            {
                Border b = new Border() { 
                Width = 100, Height = 100, 
                Background = new SolidColorBrush(Color.FromArgb(255, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256))), 
                BorderThickness = new Thickness(2), Margin=new Thickness(8) };

                wp.Children.Add(b);
            }
        }
    }
}
