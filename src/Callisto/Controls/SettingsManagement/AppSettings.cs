﻿//
// Copyright (c) 2012 Tim Heuer
//
// Licensed under the Microsoft Public License (Ms-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Callisto.Controls.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Callisto.Controls.SettingsManagement
{
    /// <summary>
    /// Provides methods and properties allowing a Windows Store app to easily
    /// implement the settings contract.
    /// </summary>
    public sealed class AppSettings : INotifySettingChanged
    {        
        #region events
        
        /// <summary>
        /// Occurs when a setting value changes.
        /// </summary>
        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        #endregion
        
        #region fields
        private readonly Dictionary<object, ISettingsCommandInfo> _commands = new Dictionary<object, ISettingsCommandInfo>();
        private SolidColorBrush _headerBrush;
        private static volatile AppSettings _instance;
        private SettingsFlyout _settingsFlyout;
        private BitmapImage _smallLogoImageSource;
        private static readonly object SyncRoot = new object();
        private VisualElement _visualElement;     
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
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppSettings();
                        }
                    }
                }

                return _instance;
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
                return _visualElement;
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

            if (!_commands.ContainsKey(key))
            {
                _commands.Add(key, new SettingsCommandInfo<T>(headerText, width));
            }
        }

        public void AddCommand<T>(string headerText, SolidColorBrush headerBrush, SettingsFlyout.SettingsFlyoutWidth width = SettingsFlyout.SettingsFlyoutWidth.Narrow) where T : UserControl, new()
        {
            string key = headerText.Trim().Replace(" ", "");
            
            _headerBrush = headerBrush;

            if (!_commands.ContainsKey(key))
            {
                _commands.Add(key, new SettingsCommandInfo<T>(headerText, width));
            }
        }
        #endregion

        #region CommandsRequested
        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (_commands != null)
                foreach (var item in _commands)
                {
                    var command = new SettingsCommand(item.Key, item.Value.HeaderText, SettingsCommandInvokedHandler);
                    args.Request.ApplicationCommands.Add(command);
                }
        }

        #endregion

        #region Configure
        private async void Configure()
        {
            _visualElement = await AppManifestHelper.GetManifestVisualElementsAsync();
            _headerBrush = new SolidColorBrush(_visualElement.BackgroundColor);
            _smallLogoImageSource = new BitmapImage(_visualElement.SmallLogoUri);
            SettingsPane.GetForCurrentView().CommandsRequested += Current.CommandsRequested;
        }
        #endregion

        #region SettingsCommandInvokedHandler
        private void SettingsCommandInvokedHandler(IUICommand command)
        {
            ISettingsCommandInfo commandInfo = _commands[command.Id];

            _settingsFlyout = new SettingsFlyout();
            _settingsFlyout.HeaderBrush = _headerBrush;
            _settingsFlyout.SmallLogoImageSource = _smallLogoImageSource;
            _settingsFlyout.HeaderText = command.Label;
#pragma warning disable 612,618
            if (ContentBackgroundBrush != null)
#pragma warning restore 612,618
            {
#pragma warning disable 612,618
                _settingsFlyout.ContentBackgroundBrush = ContentBackgroundBrush;    
#pragma warning restore 612,618
            }

            _settingsFlyout.Content = commandInfo.Instance;
            _settingsFlyout.FlyoutWidth = commandInfo.Width;
            _settingsFlyout.IsOpen = true;
        }
        #endregion

        #region SignalSettingChanged
        /// <summary>
        /// Raises the <see cref="INotifySettingChanged.SettingChanged"/> event.
        /// </summary>
        /// <param name="settingName">(Optional) The name of the setting which changed.</param>
        public void SignalSettingChanged([CallerMemberName] string settingName = "")
        {
            var handler = SettingChanged;
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