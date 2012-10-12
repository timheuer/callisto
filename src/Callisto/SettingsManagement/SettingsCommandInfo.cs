using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Callisto.SettingsManagement
{
    /// <summary>
    /// Represents the information for a <see cref="SettingsCommandInfo"/>.
    /// </summary>
    /// <typeparam name="T">A <see cref="UserControl"/> which represents the content for the settings flyout.</typeparam>
    class SettingsCommandInfo<T> : ISettingsCommandInfo where T : UserControl, new()
    {
        #region events
        #endregion

        #region fields
        #endregion

        #region constructors
        public SettingsCommandInfo(string headerText, SettingsFlyout.SettingsFlyoutWidth width)
        {
            this.HeaderText = headerText;
            this.Width = width;
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

        #region methods
        #endregion
    }
}
