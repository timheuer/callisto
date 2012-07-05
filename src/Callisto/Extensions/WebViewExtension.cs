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
// Derived from http://blogs.msdn.com/b/delay/archive/2011/04/14/quot-those-who-cannot-remember-the-past-are-condemned-to-repeat-it-quot-webbrowserextensions-stringsource-attached-dependency-property-makes-silverlight-windows-phone-wpf-s-webbrowser-control-more-xaml-and-binding-friendly.aspx

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto
{
    /// <summary>
    /// Useful extensions for the WebBrowser class.
    /// </summary>
    public static class WebViewExtension
    {
        /// <summary>
        /// Gets the value of the StringSource attached dependency property.
        /// </summary>
        /// <param name="browser">WebBrowser instance for which to get the value.</param>
        /// <returns>Value of the property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Method applies only to WebView instances.")]
        public static string GetHtmlSource(WebView view)
        {
            if (null == view)
            {
                throw new ArgumentNullException("view");
            }
            return (string)view.GetValue(HtmlSourceProperty);
        }

        /// <summary>
        /// Sets the value of the StringSource attached dependency property.
        /// </summary>
        /// <param name="browser">WebBrowser instance for which to set the value.</param>
        /// <param name="value">Value to set.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Method applies only to WebView instances.")]
        public static void SetHtmlSource(WebView view, string value)
        {
            if (null == view)
            {
                throw new ArgumentNullException("view");
            }
            view.SetValue(HtmlSourceProperty, value);
        }

        /// <summary>
        /// Defines the StringSource attached dependency property for the WebBrowser class.
        /// </summary>
        public static readonly DependencyProperty HtmlSourceProperty = DependencyProperty.RegisterAttached(
            "HtmlSource", typeof(string), typeof(WebViewExtension), new PropertyMetadata(null, OnHtmlSourcePropertyChanged));

        /// <summary>
        /// Handles changes to the StringSource attached dependency property.
        /// </summary>
        /// <param name="o">Instance for which the property changed.</param>
        /// <param name="e">Property change information.</param>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WebView", Justification = "Class name.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HtmlSource", Justification = "Property name.")]
        private static void OnHtmlSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var view = o as WebView;
            if (null == view)
            {
                throw new NotSupportedException("The HtmlSource attached dependency property is only valid for WebView instances.");
            }

            // Determine new HTML content
            var newString = e.NewValue as string;
            var newHtml = string.IsNullOrEmpty(newString) ? "<html></html>" : newString;

            view.NavigateToString(newHtml);
        }
    }
}