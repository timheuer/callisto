using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Callisto.Controls;
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

namespace XamlControlsUITestApp
{
    public sealed partial class SampleInput : UserControl
    {
        public SampleInput()
        {
            this.InitializeComponent();

            defaultbutton.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void defaultbutton_Click_1(object sender, RoutedEventArgs e)
        {
            Callisto.Controls.Flyout f = this.Parent as Callisto.Controls.Flyout;
            f.IsOpen = false;
        }
    }
}
