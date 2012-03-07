//
// Copyright (c) 2012 Jeff Wilcox
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
﻿//
// Copyright (c) 2012 Tim Heuer
//
// Licensed under the Microsoft Public License (MS-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/ms-pl.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

// NOTICE: This is a derivitive of Jeff Wilcox's WP-ThemeManager work 
// (https://github.com/jeffwilcox/wp-thememanager) for Windows Phone and is
// modified here for Metro style app development and redistributed under Ms-PL
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Shell;

//
// This nifty class will help override the runtime resources used in the app's
// instance to a specific theme (Light or Dark), the inversion of the user's
// OS setting, etc.
//
// A static OverrideOptions property can be set *before* calling the theme
// application methods to set choices for altering SystemTray instances, 
// ApplicationBars, etc.
//
// For the best effect, any code inside the application newing up 
// ApplicationBar instances should be replaced by a call to the ThemeManager
// and its CreateApplicationBar instance, or the extension method for 
// theming should be used.
//
// This set of wonderful hacks unfortunately cannot alter the MessageBox that
// the OS displays.
//
// Although this code could be adapted for use with custom app branding, at 
// this time the implementation is not designed for that.
//

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a light/dark theme.
    /// </summary>
    public enum Theme
    {
        Light,
        Dark,
    }

    /// <summary>
    /// Represents an accent color available to users on the Windows Phone. 
    /// I've also included a few popular themes found on specific phones or in
    /// specific markets.
    /// </summary>
    public enum AccentColor
    {
        Blue,
        Brown,
        Green,
        Pink,
        Purple,
        Red,
        Teal,
        Lime,
        Magenta,
        Mango,

        // Additional accent colors of interest.
        NokiaBlue,
        Gray,
    }

    /// <summary>
    /// Options for overriding the themes in the application instance with
    /// regards to system components that do not use the XAML static resource
    /// values.
    /// </summary>
    public enum ThemeManagerOverrideOptions
    {
        /// <summary>
        /// No special override options. SystemTray and ApplicationBar 
        /// instances are unaffected and will not be properly themed.
        /// </summary>
        None,

        /// <summary>
        /// Colors system trays appropriately.
        /// </summary>
        SystemTrayColors,

        /// <summary>
        /// Colors system trays appropriately and also any set ApplicationBar
        /// instances. Will not theme ApplicationBar instances that are 
        /// created after the page's Navigated event or that are created and
        /// not set immediately.
        /// </summary>
        SystemTrayAndApplicationBars,
    }

    /// <summary>
    /// A specialized Windows Phone theme management class that can be used to
    /// override the user's selected Light/Dark operating system for the 
    /// running app instance. It must be overridden inside the App::App() 
    /// constructor (or early in initialization) and cannot be changed at 
    /// runtime or called more than once.
    /// </summary>
    public static class ThemeManager
    {
        static ThemeManager()
        {
            // The default system tray overriding value. This will have a 
            // slight performance impact on all navigations, etc., hence the
            // ability to turn off. It must be set to False before calling any
            // theme override methods, however - there's no property change
            // notifications on the value.
            OverrideOptions = ThemeManagerOverrideOptions.SystemTrayAndApplicationBars;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hook up to navigation
        /// events to update system tray background and foreground colors with
        /// the overriden values.
        /// </summary>
        public static ThemeManagerOverrideOptions OverrideOptions { get; set; }

        private static Color _chrome;
        private static Color _background;
        private static Color _foreground;

        private static bool _applied;

        /// <summary>
        /// An extension method for ApplicationBars, will apply the overridden
        /// theme's coloring to the app bar if needed.
        /// </summary>
        /// <param name="bar">The application bar instance.</param>
        public static void MatchOverriddenTheme(this IApplicationBar bar)
        {
            if (bar != null && _applied) // Only if actually overrid the theme.
            {
                bar.BackgroundColor = _chrome;

                // This probably isn't the exact resource to reference but it 
                // looks correct.
                bar.ForegroundColor = _foreground;
            }
        }

        /// <summary>
        /// Creates a new instance of ApplicationBar and applies any 
        /// overridden theme coloring to the instance.
        /// </summary>
        /// <returns>Returns a new ApplicationBar instance with its foreground
        /// and background colors set.</returns>
        public static ApplicationBar CreateApplicationBar()
        {
            var ab = new ApplicationBar();
            ab.MatchOverriddenTheme();
            return ab;
        }

        /// <summary>
        /// Overrides the theme resources used at runtime for this instance.
        /// Will not make any changes to the style resources if the requested
        /// theme is the same as the runtime theme at startup.
        /// </summary>
        /// <param name="theme">The preferred Theme to override to.</param>
        public static void OverrideTheme(Theme theme)
        {
            if (_applied)
            {
                throw new InvalidOperationException("The theme can only be overridden once per application initialization.");
            }

            if (IsThemeAlready(theme))
            {
                Debug.WriteLine("The user's theme is already set to the " + theme + " theme. No resources are being overwritten.");
            }
            else
            {
                Debug.WriteLine("Overriding resources to match the " + theme + " theme.");

                _applied = true;

                new RuntimeThemeResources().Apply(theme);
            }
        }

        private static bool IsThemeAlready(Theme theme)
        {
            double current = (double)Application.Current.Resources["PhoneDarkThemeOpacity"];
            double requested = theme == Theme.Dark ? 1.0 : 0.0;

            // Double comparisons can be scary but these are straightforward.
            return current == requested;
        }

        /// <summary>
        /// Overrides the theme resources used at runtime to be the inverse of
        /// the user's current operating system theme setting.
        /// </summary>
        public static void InvertTheme()
        {
            OverrideTheme(IsThemeAlready(Theme.Dark) ? Theme.Light : Theme.Dark);
        }

        /// <summary>
        /// Overrides the theme resources used at runtime to be the Light 
        /// theme.
        /// </summary>
        public static void ToLightTheme()
        {
            OverrideTheme(Theme.Light);
        }

        /// <summary>
        /// Overrides the theme resources used at runtime to be the Dark theme.
        /// </summary>
        public static void ToDarkTheme()
        {
            OverrideTheme(Theme.Dark);
        }

        /// <summary>
        /// Overrides the accent color and brush used at runtime to a new one.
        /// </summary>
        /// <param name="color">A uint representing the color to set the brush
        /// and color to.</param>
        public static void SetAccentColor(uint color)
        {
            SetAccentColor(RuntimeThemeResources.DualColorValue.ToColor(color));
        }

        /// <summary>
        /// Overrides the accent color and brush used at runtime to a new one.
        /// </summary>
        /// <param name="color">A Color to set the accent brush/color to.</param>
        public static void SetAccentColor(Color color)
        {
            RuntimeThemeResources.DualColorValue.SetColorAndBrush("PhoneAccent", color);
        }

        /// <summary>
        /// Overrides the accent color and brush used at runtime to a new one.
        /// </summary>
        /// <param name="accentColor">Represents the new popular accent color
        /// to use.</param>
        public static void SetAccentColor(AccentColor accentColor)
        {
            SetAccentColor(AccentColorEnumToColorValue(accentColor));
        }

        private static uint AccentColorEnumToColorValue(AccentColor accent)
        {
            switch (accent)
            {
                case AccentColor.Brown:
                    return 0xFFA05000;

                case AccentColor.Green:
                    return 0xFF339933;

                case AccentColor.Lime:
                    return 0xFFA2C139;

                case AccentColor.Magenta:
                    return 0xFFD80073;

                case AccentColor.Mango:
                    return 0xFFF09609;

                case AccentColor.Pink:
                    return 0xFFE671B8;

                case AccentColor.Purple:
                    return 0xFFA200FF;

                case AccentColor.Red:
                    return 0xFFE51400;

                case AccentColor.Teal:
                    return 0xFF00ABA9;

                // Gray is a custom accent color that my Lumia 800 came with.
                case AccentColor.Gray:
                    return 0xFF4B4B4B;

                // Many Lumias come with this custom color.
                case AccentColor.NokiaBlue:
                    return 0xFF1080DD;

                case AccentColor.Blue:
                default:
                    return 0xFF1BA1E2;
            }
        }

        /// <summary>
        /// A specialized private class for handling dark/light theme resource
        /// overrides.
        /// </summary>
        private class RuntimeThemeResources
        {
            private List<ThemeValue> _values;

            public RuntimeThemeResources()
            {
                // These initial constants come from the Windows Phone design
                // resources: C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v7.1\Design\
                _values = new List<ThemeValue>
                {
                    // The prefix value actually is the text after Phone.
                    // For colors and brushes (it's assumed both exist), no
                    // suffix is in the name.

                    // Assumption hard-coded: beware the _values[0] needs to be
                    // the Background color as it is used to override the frame
                    // and its Background value.
                    // Assumption hard-coded: _values[1] is the Foreground.
                    // Assumption hard-coded: _values[2] is Chrome.
                    new ThemeValue("Background", new DualColorValue(0xFF000000, 0xFFFFFFFF)),
                    new ThemeValue("Foreground", new DualColorValue(0xFFFFFFFF, 0xDE000000)),
                    new ThemeValue("Chrome", new DualColorValue(0xFF1F1F1F, 0xFFDDDDDD)),

                    new ThemeValue("ContrastForeground", new DualColorValue(0xFF000000, 0xFFFFFFFF)),
                    new ThemeValue("ContrastBackground", new DualColorValue(0xFFFFFFFF, 0xDE000000)),
                    new ThemeValue("Disabled", new DualColorValue(0x66FFFFFF, 0x4D000000)),
                    new ThemeValue("Subtle", new DualColorValue(0x99FFFFFF, 0x66000000)),

                    new ThemeValue("TextCaret", new DualColorValue(0xFF000000, 0xDE000000)),
                    new ThemeValue("TextBox", new DualColorValue(0xBFFFFFFF, 0x26000000)),
                    new ThemeValue("TextBoxForeground", new DualColorValue(0xFF000000, 0xDE000000)),
                    new ThemeValue("TextBoxEditBackground", new DualColorValue(0xFFFFFFFF, 0x00000000)),
                    new ThemeValue("TextBoxEditBorder", new DualColorValue(0xFFFFFFFF, 0xDE000000)),
                    new ThemeValue("TextBoxReadOnly", new DualColorValue(0x77000000, 0x2E000000)),

                    new ThemeValue("RadioCheckBox", new DualColorValue(0xBFFFFFFF, 0x26000000)),
                    new ThemeValue("RadioCheckBoxDisabled", new DualColorValue(0x66FFFFFF, 0x00000000)),
                    new ThemeValue("RadioCheckBoxCheck", new DualColorValue(0xFF000000, 0xDE000000)),
                    new ThemeValue("RadioCheckBoxCheckDisabled", new DualColorValue(0x66000000, 0x4D000000)),
                    new ThemeValue("RadioCheckBoxPressed", new DualColorValue(0xFFFFFFFF, 0x00000000)),
                    new ThemeValue("RadioCheckBoxPressedBorder", new DualColorValue(0xFFFFFFFF, 0xDE000000)),

                    new ThemeValue("Semitransparent", new DualColorValue(0xAA000000, 0xAAFFFFFF)),

                    new ThemeValue("Inactive", new DualColorValue(0x33FFFFFF, 0x33000000)),
                    new ThemeValue("InverseInactive", new DualColorValue(0xFFCCCCCC, 0xFFE5E5E5)),
                    new ThemeValue("InverseBackground", new DualColorValue(0xFFFFFFFF, 0xFFDDDDDD)),

                    new ThemeValue("Border", new DualColorValue(0xBFFFFFFF, 0x99000000)),

                    new ThemeValue("DarkThemeVisibility", new DualValue<Visibility>(Visibility.Visible, Visibility.Collapsed)),
                    new ThemeValue("LightThemeVisibility", new DualValue<Visibility>(Visibility.Collapsed, Visibility.Visible)),

                    new ThemeValue("DarkThemeOpacity", new DualValue<double>(1.0, 0.0)),
                    new ThemeValue("LightThemeOpacity", new DualValue<double>(0.0, 1.0)),
                };

                // Could customize this class to offer debug/testing-specific
                // values or branding themes here.
            }

            internal class DualColorValue : IDualValue
            {
                private uint _dark;
                private uint _light;

                public DualColorValue(uint dark, uint light)
                {
                    _dark = dark;
                    _light = light;
                }

                internal static void SetColorAndBrush(string prefix, Color color)
                {
                    var currentColor = (Color)Application.Current.Resources[prefix + "Color"];
                    currentColor.A = color.A;
                    currentColor.B = color.B;
                    currentColor.G = color.G;
                    currentColor.R = color.R;

                    var brush = (SolidColorBrush)Application.Current.Resources[prefix + "Brush"];
                    brush.Color = currentColor;
                }

                public object Value(Theme theme)
                {
                    return ToColor(theme == Theme.Dark ? _dark : _light);
                }

                internal static Color ToColor(uint argb)
                {
                    return Color.FromArgb((byte)((argb & -16777216) >> 24), (byte)((argb & 0xff0000) >> 16), (byte)((argb & 0xff00) >> 8), (byte)(argb & 0xff));
                }

                public void Apply(Theme theme, string prefix)
                {
                    string name = "Phone" + prefix;
                    var value = (Color)(Value(theme));
                    SetColorAndBrush(name, value);
                }
            }

            private class DualValue<T> : IDualValue
            {
                private T _dark;
                private T _light;

                public DualValue(T dark, T light)
                {
                    _dark = dark;
                    _light = light;
                }

                public object Value(Theme theme)
                {
                    return theme == Theme.Dark ? _dark : _light;
                }

                public void Apply(Theme theme, string prefix)
                {
                    string name = "Phone" + prefix;
                    Application.Current.Resources.Remove(name);
                    Application.Current.Resources.Add(name, Value(theme));
                }
            }

            private interface IDualValue
            {
                object Value(Theme theme);
                void Apply(Theme theme, string prefix);
            }

            public void Apply(Theme theme)
            {
                foreach (var value in _values)
                {
                    value.Apply(theme);
                }

                Debug.Assert(_values[0]._prefix == "Background", "It's magic but the first value in the list should be the Background for performance reasons.");
                var background = _values[0]._value;

                Debug.Assert(_values[1]._prefix == "Foreground", "It's magic but the second value in the list should be the Foreground for performance reasons.");
                var foreground = _values[1]._value;

                Debug.Assert(_values[2]._prefix == "Chrome", "It's magic but the third value in the list should be Chrome for performance reasons.");
                var chrome = _values[2]._value;

                // Using navigation events, hook up to alter the system tray
                // values as well as the page's set application bar.
                AttachRootFrameNavigationEvents((Color)background.Value(theme), (Color)foreground.Value(theme), (Color)chrome.Value(theme));

                // We pass the memory savings on to you!
                _values = null;
            }

            private void AttachOnRootFrameReady(PhoneApplicationFrame frame, Color background, Color foreground, Color chrome)
            {
                frame.Navigated += (x, xe) =>
                {
                    var page = xe.Content as PhoneApplicationPage;
                    if (page != null)
                    {
                        SetSystemComponentColors(page, background, foreground, chrome);
                    }
                };

                var firstPage = frame.Content as PhoneApplicationPage;
                SetSystemComponentColors(firstPage, background, foreground, chrome);
            }

            private void SetSystemComponentColors(PhoneApplicationPage page, Color background, Color foreground, Color chrome)
            {
                if (page != null)
                {
                    SystemTray.SetBackgroundColor(page, background);
                    SystemTray.SetForegroundColor(page, foreground);

                    if (OverrideOptions == ThemeManagerOverrideOptions.SystemTrayAndApplicationBars)
                    {
                        var appBar = page.ApplicationBar as IApplicationBar;
                        if (appBar != null)
                        {
                            appBar.MatchOverriddenTheme();
                        }
                    }
                }
            }

            private void AttachRootFrameNavigationEvents(Color background, Color foreground, Color chrome)
            {
                ThemeManager._chrome = chrome;
                ThemeManager._background = background;
                ThemeManager._foreground = foreground;

                // Loop until the root frame is set for real. To really 
                // improve startup performance, instead the system could take 
                // in a reference to the Frame object and we could require the
                // caller of theme functions to instead use that.
                var frame = Application.Current.RootVisual;
                if (frame != null)
                {
                    if (!(frame is Canvas))
                    {
                        // We have the real frame now. Unfortunately this may
                        // increase the fill count heavily in the app by += 1.
                        var asControl = frame as Control;
                        if (asControl != null)
                        {
                            asControl.Background = new SolidColorBrush(background);
                        }

                        // Hook up to the navigation events for the tray.
                        if (OverrideOptions == ThemeManagerOverrideOptions.SystemTrayAndApplicationBars || 
                            OverrideOptions == ThemeManagerOverrideOptions.SystemTrayColors)
                        {
                            PhoneApplicationFrame paf = frame as PhoneApplicationFrame;
                            if (paf != null)
                            {
                                AttachOnRootFrameReady(paf, background, foreground, chrome);
                            }
                        }

                        return;
                    }
                }

                Deployment.Current.Dispatcher.BeginInvoke(() => AttachRootFrameNavigationEvents(background, foreground, chrome));
            }

            private class ThemeValue
            {
                public ThemeValue(string prefix, IDualValue val)
                {
                    _prefix = prefix;
                    _value = val;
                }

                public string _prefix;
                public IDualValue _value;

                public void Apply(Theme theme)
                {
                    _value.Apply(theme, _prefix);
                }
            }
        }
    }
}
