# Changelog

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