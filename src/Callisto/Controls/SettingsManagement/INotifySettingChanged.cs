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
 
using System;

namespace Callisto.Controls.SettingsManagement
{
    /// <summary>
    /// Notifies clients that a setting value has changed.
    /// </summary>
    public interface INotifySettingChanged
    {
        /// <summary>
        /// Occurs when a setting value changes.
        /// </summary>
        event EventHandler<SettingChangedEventArgs> SettingChanged;
    }

}
