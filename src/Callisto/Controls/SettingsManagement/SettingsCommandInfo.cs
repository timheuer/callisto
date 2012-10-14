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

using Windows.UI.Xaml.Controls;

namespace Callisto.Controls.SettingsManagement
{
    /// <summary>
    /// Represents the information for a <see cref="SettingsCommandInfo{T}"/>.
    /// </summary>
    /// <typeparam name="T">A <see cref="UserControl"/> which represents the content for the settings flyout.</typeparam>
    class SettingsCommandInfo<T> : ISettingsCommandInfo where T : UserControl, new()
    {
        #region constructors
        public SettingsCommandInfo(string headerText, SettingsFlyout.SettingsFlyoutWidth width)
        {
            HeaderText = headerText;
            Width = width;
        }
        #endregion

        #region properties

        #region HeaderText
        /// <summary>
        /// Gets the header of the settings command to display in the Settings charm.
        /// </summary>
        /// <value>The header of the settings command.</value>
        public string HeaderText
        {
            get;
            private set;
        }
        #endregion

        #region Instance
        /// <summary>
        /// Gets the instantiated <see cref="UserControl"/> instance which represents
        /// the content of the settings flyout.
        /// </summary>
        public UserControl Instance
        {
            get
            {
                return new T();
            }
        }
        #endregion

        #region Width
        /// <summary>
        /// Gets the width of the settings flyout.
        /// </summary>
        /// <value>The width of the settings flyout.</value>
        public SettingsFlyout.SettingsFlyoutWidth Width
        {
            get;
            private set;
        }
        #endregion
        #endregion
    }
}
