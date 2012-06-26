using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XamlControlsUITestApp
{
    sealed partial class App : Application
    {
       public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(Callisto.TestApp.MainPage));

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }
    }
}
