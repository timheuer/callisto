using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
    public sealed partial class FlipViewIndicatorSample : Page
    {
        public FlipViewIndicatorSample()
        {
            this.InitializeComponent();

            Loaded += FlipViewIndicatorSample_Loaded;
        }

        void FlipViewIndicatorSample_Loaded(object sender, RoutedEventArgs e)
        {
            var cvm = this.DataContext as ColorViewModel;

            Flipper2.ItemsSource = cvm.ColorsCollection;
        }
    }

    public class ColorViewModel
    {
        public ObservableCollection<SolidColorBrush> ColorsCollection
        {
            get
            {
                return new ObservableCollection<SolidColorBrush>()
                {
                    new SolidColorBrush(Colors.Red),
                    new SolidColorBrush(Colors.Blue),
                    new SolidColorBrush(Colors.Purple),
                    new SolidColorBrush(Colors.Orange)
                };
            }
        }
    }
}
