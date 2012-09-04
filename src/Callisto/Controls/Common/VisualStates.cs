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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls.Common
{
    /// <summary>
    /// Names and helpers for visual states in the controls.
    /// </summary>
    internal static class VisualStates
    {
        #region GroupCommon
        /// <summary>
        /// Common state group.
        /// </summary>
        public const string GroupCommon = "CommonStates";

        /// <summary>
        /// Normal state of the Common state group.
        /// </summary>
        public const string StateNormal = "Normal";

        /// <summary>
        /// Normal state of the Common state group.
        /// </summary>
        public const string StateReadOnly = "ReadOnly";

        /// <summary>
        /// MouseOver state of the Common state group.
        /// </summary>
        public const string StatePointerOver = "PointerOver";

        /// <summary>
        /// Pressed state of the Common state group.
        /// </summary>
        public const string StatePointerPressed = "PointerPressed";

        /// <summary>
        /// Exited state of the Common state group.
        /// </summary>
        public const string StatePointerExited = "PointerExited";

        /// <summary>
        /// Disabled state of the Common state group.
        /// </summary>
        public const string StateDisabled = "Disabled";
        #endregion GroupCommon

        #region GroupFocus
        /// <summary>
        /// Focus state group.
        /// </summary>
        public const string GroupFocus = "FocusStates";

        /// <summary>
        /// Unfocused state of the Focus state group.
        /// </summary>
        public const string StateUnfocused = "Unfocused";

        /// <summary>
        /// Focused state of the Focus state group.
        /// </summary>
        public const string StateFocused = "Focused";
        #endregion GroupFocus

        #region GroupActive
        /// <summary>
        /// Active state.
        /// </summary>
        public const string StateActive = "Active";

        /// <summary>
        /// Inactive state.
        /// </summary>
        public const string StateInactive = "Inactive";

        /// <summary>
        /// Active state group.
        /// </summary>
        public const string GroupActive = "ActiveStates";
        #endregion GroupActive

        #region GroupVisibility
        /// <summary>
        /// Visible state name for BusyIndicator.
        /// </summary>
        public const string StateVisible = "Visible";

        /// <summary>
        /// Hidden state name for BusyIndicator.
        /// </summary>
        public const string StateHidden = "Hidden";

        /// <summary>
        /// BusyDisplay group.
        /// </summary>
        public const string GroupVisibility = "VisibilityStates";
        #endregion

        #region GroupWatermark
        /// <summary>
        /// Non-watermarked state.
        /// </summary>
        public const string StateUnwatermarked = "Unwatermarked";

        /// <summary>
        /// Watermarked state.
        /// </summary>
        public const string StateWatermarked = "Watermarked";

        /// <summary>
        /// Watermark state group.
        /// </summary>
        public const string GroupWatermark = "WatermarkStates";
        #endregion GroupWatermark

        /// <summary>
        /// Use VisualStateManager to change the visual state of the control.
        /// </summary>
        /// <param name="control">
        /// Control whose visual state is being changed.
        /// </param>
        /// <param name="useTransitions">
        /// A value indicating whether to use transitions when updating the
        /// visual state, or to snap directly to the new visual state.
        /// </param>
        /// <param name="stateNames">
        /// Ordered list of state names and fallback states to transition into.
        /// Only the first state to be found will be used.
        /// </param>
        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        {
            Debug.Assert(control != null, "control should not be null!");
            Debug.Assert(stateNames != null, "stateNames should not be null!");
            Debug.Assert(stateNames.Length > 0, "stateNames should not be empty!");

            foreach (string name in stateNames)
            {
                if (VisualStateManager.GoToState(control, name, useTransitions))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the implementation root of the Control.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject.</param>
        /// <remarks>
        /// Implements Silverlight's corresponding internal property on Control.
        /// </remarks>
        /// <returns>Returns the implementation root or null.</returns>
        public static FrameworkElement GetImplementationRoot(DependencyObject dependencyObject)
        {
            Debug.Assert(dependencyObject != null, "DependencyObject should not be null.");
            return (1 == VisualTreeHelper.GetChildrenCount(dependencyObject)) ?
                VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement :
                null;
        }

        /// <summary>
        /// This method tries to get the named VisualStateGroup for the 
        /// dependency object. The provided object's ImplementationRoot will be 
        /// looked up in this call.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="groupName">The visual state group's name.</param>
        /// <returns>Returns null or the VisualStateGroup object.</returns>
        public static VisualStateGroup TryGetVisualStateGroup(DependencyObject dependencyObject, string groupName)
        {
            FrameworkElement root = GetImplementationRoot(dependencyObject);
            if (root == null)
            {
                return null;
            }

            return VisualStateManager.GetVisualStateGroups(root)
                .OfType<VisualStateGroup>()
                .Where(group => string.CompareOrdinal(groupName, group.Name) == 0)
                .FirstOrDefault();
        }
    }
}
