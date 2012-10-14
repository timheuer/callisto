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

namespace Callisto.Controls.SettingsManagement
{
    /// <summary>
    /// Provides data for the <see cref="INotifySettingChanged.SettingChanged"/>
    /// event.</summary>
    public sealed class SettingChangedEventArgs : System.EventArgs
    {
        #region fields
        private readonly string _settingName;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Callisto.Controls.SettingsManagement.SettingChangedEventArgs"/>
        /// class.</summary>
        /// <param name="settingName">The name of the setting that changed.</param>
        public SettingChangedEventArgs(string settingName)
        {
            _settingName = settingName;
        }
        #endregion

        #region properties

        #region SettingName
        /// <summary>
        /// Gets the name of the setting that changed.
        /// </summary>
        /// <returns>The name of the setting that changed.</returns>
        public string SettingName
        {
            get
            {
                return _settingName;
            }
        }
        #endregion

        #endregion
    }
}
