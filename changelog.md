# Changelog

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