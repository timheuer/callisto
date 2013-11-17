using Callisto.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.TestApp.SamplePages
{
    public sealed partial class MapSample : Page
    {
        public MapSample()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => TileLayerProvider.SelectedIndex = 0;
        }

        private void TileLayerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var tileLayers = (TileLayerCollection)Resources["TileLayers"];
            map.TileLayer = tileLayers[(string)((ComboBoxItem)comboBox.SelectedItem).Content];
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var tileLayers = (TileLayerCollection)Resources["TileLayers"];
            var tileLayer = tileLayers["Seamarks"];

            if ((bool)checkBox.IsChecked)
            {
                map.TileLayers.Add(tileLayer);
            }
            else
            {
                map.TileLayers.Remove(tileLayer);
            }
        }
    }
}
