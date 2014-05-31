# Callisto

[![Build status](http://ci.appveyor.com/api/projects/status/cibx2vk3h5fr53t7)](http://ci.appveyor.com/project/TimHeuer/callisto)

## What Is It?

Callisto is a library for use in Windows Store XAML applications (aka Metro style apps).  The XAML framework in Windows.UI.Xaml is great, but has some functionality that isn't provided in-the-box in a few controls and APIs.  Callisto provides added functionality on top of the XAML UI framework for Windows.

You can also read more under [Frequently Asked Questions](https://github.com/timheuer/callisto/wiki/FAQ).  Documentation for each control is always an 'in progress' thing ([contributions welcome](contributing.md)) but is on the [Wiki page here](https://github.com/timheuer/callisto/wiki/).

## What's In It?
So far Callisto includes:

* `Rating` - a ratings control for Metro UI
* `LiveTile` - an in-app tile experience to give you animated or 'live' tiles
* `Tilt` - an effect to provide the tilt experience when clicked on edges/corners
* OAuth helpers - a set of helpers to create OAuth 1.0 signatures/headers for those sites that hate OAuth 2.0 :-)
* `BooleanToVisibilityConverter` - a converter to well, convert boolean to visibility, very common use
* `LengthToBooleanConverter` - a converter to examine a string length to convert to boolean (simple validation helper)
* `RelativeTimeConverter` - a converter to show time as relative string, i.e., 'about an hour ago'
* Extensions - some extension method helpers (i.e., bind html content to a WebView)

What is in the plan (could always use the help):

* `DatePicker` and `TimePicker` - providing a globalized time/date picker control in Metro style
* `Ratings` - providing a rating control for Metro style
* `Pivot` - a UI interface for providing "tabbed" interface in a Metro style way

Note the following have been deprecated in the most recent release as Windows 8.1 introduces proper controls for these:

* `Flyout` - a primitive that includes positioning and 'light dismiss' logic
* `Menu` - primarily to be used from AppBar, contains the base for providing, well, a Menu
* `MenuItem` - an item for a menu, including separators and contains the command point for the menu item
* `SettingsFlyout` - an item to create a custom settings pane UI

## How To Install It?
There are two ways you can install the control.  

### Visual Studio Extension SDK
Install as an Extension SDK via the Visual Studio Gallery - available within VS as well as [downloading from the gallery directly](http://visualstudiogallery.msdn.microsoft.com/0526563b-7a48-4b17-a087-a35cea701052)

### NuGet
You can get the control via [NuGet](http://www.nuget.org) if you have the extension installed for Visual Studio or via the PowerShell package manager.  This control is published via NuGet at [Callisto](https://nuget.org/packages/Callisto).

## How To Use It?
Controls are individually documented with examples for usage on the [Wiki Documentation](https://github.com/timheuer/callisto/wiki).

## I found an Issue
Great, please [log a bug](https://github.com/timheuer/Callisto/issues/new) so that it can be tracked. 

## Credits and Acknowledgements
* [Tim Heuer](http://timheuer.com/blog/) ([@timheuer](http://twitter.com/timheuer)) - for the initial conception of the toolkit.
* [Morten Nielsen](http://www.sharpgis.net/) ([@dotMorten](http://twitter.com/dotMorten)) - core contributor
* Unni Ravindranathan ([@unnir](http://twitter.com/unnir)) - For all the great guidance and always pushing for better designer integration!
* Moon logo from [The Noun Project](http://thenounproject.com) which is a great source for Metro style iconography

### Open Source Project Credits
Some of the code represented in the toolkit is forks/ports of other Open Source libraries.  Where used, their license is reprsented in the code files.

* [Silverlight Toolkit](http://silverlight.codeplex.com) - for providing the base of many ported concepts/code.  Licensed under Ms-PL
* [RestSharp](http://restsharp.org) - the OAuth helper file is a fork from the RestSharp project which uses this internally for some authenticators. Licensed under Apache 2.0
