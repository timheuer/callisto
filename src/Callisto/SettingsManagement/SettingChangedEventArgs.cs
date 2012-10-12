using System;
using System.Linq;

namespace Callisto.SettingsManagement
{
    /// <summary>
    /// Provides data for the <see cref="INotifySettingChanged.SettingChanged"/>
    /// event.</summary>
    public sealed class SettingChangedEventArgs : System.EventArgs
    {
        #region events
        #endregion

        #region fields
        private readonly string settingName;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.SettingChangedEventArgs"/>
        /// class.</summary>
        /// <param name="settingName">The name of the setting that changed.</param>
        public SettingChangedEventArgs(string settingName)
        {
            this.settingName = settingName;
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
                return this.settingName;
            }
        }
        #endregion

        #endregion

        #region methods
        #endregion
    }
}
