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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Callisto.Controls
{
	/// <summary>
	/// Event args for a value changing event
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
		/// <summary>
		/// Gets the old value.
		/// </summary>
        public T OldValue { get; private set; }
		/// <summary>
		/// Gets the new value.
		/// </summary>
        public T NewValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
