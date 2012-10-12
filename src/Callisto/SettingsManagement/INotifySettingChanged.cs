using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callisto.SettingsManagement
{
    /// <summary>
    /// Notifies clients that a setting value has changed.
    /// </summary>
    public interface INotifySettingChanged
    {
        #region events
        /// <summary>
        /// Occurs when a setting value changes.
        /// </summary>
        event EventHandler<SettingChangedEventArgs> SettingChanged;
        #endregion

        #region properties
        #endregion

        #region methods
        #endregion
    }

}
