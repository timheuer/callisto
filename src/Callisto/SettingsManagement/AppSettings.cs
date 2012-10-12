using Callisto.Controls;
using Callisto.Controls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Callisto.SettingsManagement
{
    /// <summary>
    /// Provides methods and properties allowing a Windows Store app to easily
    /// implement the settings contract.
    /// </summary>
    public sealed partial class AppSettings : INotifySettingChanged
    {        
        #region events
        
        /// <summary>
        /// Occurs when a setting value changes.
        /// </summary>
        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        #endregion
        
        #region fields
        private readonly Dictionary<object, ISettingsCommandInfo> commands = new Dictionary<object, ISettingsCommandInfo>();
        private SolidColorBrush headerBrush;
        private static volatile AppSettings instance;
        private SettingsFlyout settingsFlyout;
        private BitmapImage smallLogoImageSource;
        private static object syncRoot = new object();
        private VisualElement visualElement;     
        #endregion
        
        #region constructors
        
        private AppSettings()
        {
            Configure();
        }

        #endregion
        
        #region properties
        
        #region ContentBackgroundBrush
        
        /// <summary>
        /// Gets or sets the background brush for the content area of the settings flyout.
        /// </summary>
        /// <value>The <see cref="SolidColorBrush"/> for the background of the content area of the settings flyout.</value>
        [Obsolete("This is provided for backwards compatability until a proper light theme can be created so settings content will display corectly on a white background.", false)]
        public SolidColorBrush ContentBackgroundBrush { get; set; }
        
        #endregion
        
        #region Current
        
        /// <summary>
        /// Provides access to the app settings contract.
        /// </summary>
        /// <value>The app settings contract.</value>
        public static AppSettings Current
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new AppSettings();
                        }
                    }
                }

                return instance;
            }
        }
            
        #endregion

        #region VisualElements
        /// <summary>
        /// Gets a <see cref="VisualElement"/> instance which contains specific
        /// information from the application manifest.
        /// </summary>
        public VisualElement VisualElements
        {
            get
            {
                return this.visualElement;
            }
        }
        #endregion

        #endregion

        #region methods

        #region AddCommand
        /// <summary>
        /// Add a <see cref="UserControl"/> to the settings contract using the specified header.
        /// </summary>
        /// <typeparam name="T">A <see cref="UserControl"/> which represents the content for the settings flyout.</typeparam>
        /// <param name="headerText">The header to be displayed in the Settings charm.</param>
        /// <param name="width">(Optional) The width of the settings flyout. The default is <see cref="SettingsFlyout.SettingsFlyoutWidth.Narrow">SettingsFlyout.SettingsFlyoutWidth.Narrow</see></param>
        public void AddCommand<T>(string headerText, SettingsFlyout.SettingsFlyoutWidth width = SettingsFlyout.SettingsFlyoutWidth.Narrow) where T : UserControl, new()
        {
            string key = headerText.Trim().Replace(" ", "");

            if (!commands.ContainsKey(key))
            {
                this.commands.Add(key, new SettingsCommandInfo<T>(headerText, width));
            }
        }
        #endregion

        #region CommandsRequested
        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            foreach (var item in this.commands)
            {
                var command = new SettingsCommand(item.Key, item.Value.HeaderText, SettingsCommandInvokedHandler);
                args.Request.ApplicationCommands.Add(command);
            }
        }
        #endregion

        #region Configure
        private async void Configure()
        {
            visualElement = await AppManifestHelper.GetManifestVisualElementsAsync();
            headerBrush = new SolidColorBrush(visualElement.BackgroundColor);
            smallLogoImageSource = new BitmapImage(visualElement.SmallLogoUri);
            SettingsPane.GetForCurrentView().CommandsRequested += AppSettings.Current.CommandsRequested;
        }
        #endregion

        #region SettingsCommandInvokedHandler
        private void SettingsCommandInvokedHandler(IUICommand command)
        {
            ISettingsCommandInfo commandInfo = commands[command.Id];

            settingsFlyout = new SettingsFlyout();
            settingsFlyout.HeaderBrush = this.headerBrush;
            settingsFlyout.SmallLogoImageSource = this.smallLogoImageSource;
            settingsFlyout.HeaderText = command.Label;
            if (this.ContentBackgroundBrush != null)
            {
                settingsFlyout.ContentBackgroundBrush = this.ContentBackgroundBrush;    
            }

            settingsFlyout.Content = commandInfo.Instance;
            settingsFlyout.FlyoutWidth = commandInfo.Width;
            settingsFlyout.IsOpen = true;
        }
        #endregion

        #region SignalSettingChanged
        /// <summary>
        /// Raises the <see cref="INotifySettingChanged.SettingChanged"/> event.
        /// </summary>
        /// <param name="settingName">(Optional) The name of the setting which changed.</param>
        public void SignalSettingChanged([CallerMemberName] string settingName = "")
        {
            var handler = this.SettingChanged;
            if (handler != null)
            {
                var e = new SettingChangedEventArgs(settingName);
                handler(this, e);
            }
        }
        #endregion

        #endregion
    }
}