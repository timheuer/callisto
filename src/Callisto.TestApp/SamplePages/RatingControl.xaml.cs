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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Callisto.TestApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RatingControl : Page
    {
        public RatingControl()
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
        }

        private void rate_ValueChanged_1(object sender, ValueChangedEventArgs<double> e)
        {
            Rating r = sender as Rating;
            if (EventOutput != null)
            {
                EventOutput.Text = string.Format("You selected '{0}'", e.NewValue.ToString());
                EventOutput.Text += string.Format("\nOld Value: {0}, New Value: {1}", e.OldValue.ToString(), e.NewValue.ToString());
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            rate.Value = 4;
        }
    }
}
