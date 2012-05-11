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

// BASE CODE PROVIDED UNDER THIS LICENSE
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
#if INCLUDE_EXPERIMENTAL
using System;

namespace Callisto.Controls
{
    /// <summary>
    /// Provides data for the DatePicker and TimePicker's ValueChanged event.
    /// </summary>
    public class DateTimeValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeValueChangedEventArgs class.
        /// </summary>
        /// <param name="oldDateTime">Old DateTime value.</param>
        /// <param name="newDateTime">New DateTime value.</param>
        public DateTimeValueChangedEventArgs(DateTime? oldDateTime, DateTime? newDateTime)
        {
            OldDateTime = oldDateTime;
            NewDateTime = newDateTime;
        }

        /// <summary>
        /// Gets or sets the old DateTime value.
        /// </summary>
        public DateTime? OldDateTime { get; private set; }

        /// <summary>
        /// Gets or sets the new DateTime value.
        /// </summary>
        public DateTime? NewDateTime { get; private set; }
    }
}
#endif