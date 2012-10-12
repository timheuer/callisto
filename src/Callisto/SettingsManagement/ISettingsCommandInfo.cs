using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Callisto.Controls;

namespace Callisto.SettingsManagement
{
    /// <summary>
    /// Represents the information for a <see cref="SettingsCommandInfo"/>.
    /// </summary>
    interface ISettingsCommandInfo
    {
        #region events
        #endregion

        #region properties

        #region HeaderText
        /// <summary>
        /// Gets the header of the settings command to display in the Settings charm.
        /// </summary>
        /// <value>The header of the settings command.</value>
        string HeaderText
        {
            get;
        }
        #endregion

        #region Instance
        /// <summary>
        /// Gets the instantiated <see cref="UserControl"/> instance which represents
        /// the content of the settings flyout.
        /// </summary>
        UserControl Instance
        {
            get;
        }
        #endregion

        #region Width
        /// <summary>
        /// Gets the width of the settings flyout.
        /// </summary>
        /// <value>The width of the settings flyout.</value>
        SettingsFlyout.SettingsFlyoutWidth Width
        {
            get;
        }
        #endregion

        #endregion

        #region methods
        #endregion
    }
}
