using Callisto.Controls;
using Callisto.Controls.SettingsManagement;
using Callisto.TestApp.SamplePages;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XamlControlsUITestApp
{
    sealed partial class App : Application
    {
        public static Callisto.Controls.Common.VisualElement VisualElements;

        public App()
        {
            this.InitializeComponent();
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            AppSettings.Current.AddCommand<SettingsContent>("App Registered", SettingsFlyout.SettingsFlyoutWidth.Wide);

            VisualElements = await Callisto.Controls.Common.AppManifestHelper.GetManifestVisualElementsAsync();

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(Callisto.TestApp.MainPage));

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }
    }
}
