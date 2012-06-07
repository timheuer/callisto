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

namespace Callisto.Controls
{
    /// <summary>
    /// This type is used to determine the state of the item selected and the
    /// previous items.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum RatingSelectionMode
    {
        /// <summary>
        /// All items before the selected ones are selected.
        /// </summary>
        Continuous,

        /// <summary>
        /// Only the item selected is visually distinguished.
        /// </summary>
        Individual
    }
}