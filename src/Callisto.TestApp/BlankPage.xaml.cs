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
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            //DateTimeWrapper dtw = new DateTimeWrapper(DateTime.Now);
            DateFormatted.Text = Windows.Globalization.DateTimeFormatting.DateTimeFormatter.ShortDate.Format(DateTimeOffset.Now); //dtw.HourNumber.ToString();
            DateCalculated.DataContext = DateTime.Now.AddHours(-2); // new DateTime(2012, 1, 12, 13, 13, 5);

            //MyTimePicker.Value = DateTime.Now;
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
    }
}
