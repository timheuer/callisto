using Callisto.Controls;
using LinqToVisualTree;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Callisto.TestApp.SamplePages
{
    public sealed partial class SettingsContent : UserControl
    {
        public SettingsContent()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Callisto.Controls.SettingsFlyout sf = this.Ancestors<Callisto.Controls.SettingsFlyout>().FirstOrDefault() as Callisto.Controls.SettingsFlyout;
            sf.IsOpen = false;
        }
    }
}
