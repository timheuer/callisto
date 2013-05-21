# Changelog

## 1.3.0 (21-MAY-2013)
* Fix #204: Fixed memory leak in CustomDialog (thanks kiwidev!)
* NEW: Added WrapPanel

## 1.2.9 (22-APR-2013)
* Fix #190: Setting IsOpen to false when the flyout is dismissed
* Fix #189: Fixing en-ZA localization and adding en fallback resources

## 1.2.8 (11-APR-2013)
* Fix #184: Quick fix to SettingsFlyout to prevent horizontal overbounce visual artifact

## 1.2.7 (28-MAR-2013)
* NEW: Added DropdownButton control
* Fix #47: Added BackClick event (thanks lprichar!)
* Fix #167: Added listener for Window.SizeChanged
* Fix #171: Enhancing flyout positioning logic
* Fix #156: Ensure back button on SettingsFlyout works for keyboard enter/space bar
* Fix #136: Unsealed CustomDialog
* Fix #140: Fix flyout parent check for menu
* NumericUpDown: Changed to repeat buttons to enable holding down the button on mouse click
* Ratings: fixed some random bugs
* Ratings: Added ReadOnlyFill (thanks ScottIsAFool!)

## 1.2.6 (04-DEC-2012)
* Fix #129: Added BackButtonCommand/Parameter to CustomDialog
* Fix #131: Added default colors for Background to CustomDialog

## 1.2.5 (28-OCT-2012)
* NEW: Added CustomDialog control
* Fixes #110: Implemented FromName method to enable use of named values in manifest for color in AppManifestHelper

## 1.2.4 (16-OCT-2012)
* NEW: Added NumericUpDown control
* NEW: ColorContrastConverter which does a YIQ calculation to determine white/black
* NEW: SettingsManagement API making it easier to do app-wide registration of SettingsFlyout elements.  Thanks Scott Dorman!  Community contributions FTW!
* NEW: Header in SettingsFlyout will automatically set contrast based on HeaderBrush color set.
* Fixes #104 with introduction of SettingsBackButtonStyle2
* Fixes #99 will null checks

## 1.2.3 (05-OCT-2012)
* HOTFIX: Fix #96 for converting without using InvariantCulture.  Sorry :-(

## 1.2.2 (04-OCT-2012)
* BREAKING CHANGE: Fixes #81 where the defaults to not honor the UI guidelines.  Reluctantly added new DPs for override if needed (but shouldn't be used)
* Fixes #74: Incorrect null check on WatermarkTextBox DP in property changed callback
* Fixes #75: Made Margin on Rating template bound but kept defaults for compat
* Fixes #83: Menu default focus issues.  Thanks lukasf!
* Fixes #82: If Flyout.HostPopup has a parent then some of the positioning is wrong.
* Fixes #79: Ensure IsOpen set to false on Flyout when the host Popup closes.  Thanks Stefan!
* TiltEffect: Stop tilt effect if pointer capture is lost
* Fixes #85: SettingsFlyout/Flyout for cases where input controls might be there and the IHM (aka software keyboard) might occlude the view.
* Fixes #86: SettingsFlyout 1px border to match UI design specs where border is 80% brightness of HeaderBrush (added ColorBrightnessConverter)
* NEW: Added AppManifestHelper to quickly parse out the VisualElements from the AppxManifest.xml

## 1.2.1 (03-SEP-2012)
* NEW: Added WatermarkTextBox control
* NEW: Added FlipViewIndicator control (hat tip to Diederik Krols -- thx for the help!)

## 1.1.0
* Fixes #69: Back button in snapped mode will attempt to show SettingsPane.  Put temp provisioin in to prevent.
* NEW: Adding DynamicTextBlock for CharacterEllipsis mode on a TextBlock

## 1.0.13
* Fixes #58: mostly fixing this bug maintaining the UI guidelines on margin/edge content, but hosting within ScrollViewer to enable edge-to-edge scroll behavior if content large enough.
* Fix for SettingsFlyout when OS is an RTL language.  On RTL, SettingsPane comes from the left edge and thus the flyout needs to respond that way.

## 1.0.12
* Fixes #67: removes dependency on BackButtonSnappedGlyph from StandardStyles.xaml so that it is self-contained.
* Fixes #24: ToggleMenuItem margins for toggle menu items only.  Thanks Nigel!
* Fixes #57: Checks that if the parent is a Flyout before closing.
* Compiled against RTM bits for .NET framework

## 1.0.11
* temporary release for early adopter customers
* all fixes are in 1.0.12

## 1.0.10
* Fixes #26 (again): implementing user suggestions of looking for invalid measures in ActualHeight/Width

## 1.0.9
* Fixes #52: WebViewExtension was broken as property name was wrong

## 1.0.8
* Rating/RatingItem: Fixed some PointerExit behavior

## 1.0.7
* Adding Rating->RatingItem template binding to Background property

## 1.0.6
* BREAKING CHANGE: Fixed HasAlgorithmProvider for OAuthUtils class.  Had to change to MacAlgorithmProvider input arg per Windows behavior change

## 1.0.5
* Fixed #42: Adding ToolTip to RatingItem
* BREAKING CHANGE: Changed the way Rating works so that it is no longer a 0-1 scale but rather relative to the ItemCount value (i.e., 2.5/5)

## 1.0.4
* Fixed #39: Getting HighContrast mode not to crash on RatingItem

## 1.0.3
* Fixed some bugs on Rating control needed for customers

## 1.0.2
* Added Rating control

## 1.0.1
* Fixed incorrect DP registration for Flyout

## 1.0.0
* Initial RC release