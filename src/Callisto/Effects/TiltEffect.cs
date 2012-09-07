//
// Copyright (c) 2012 Morten Nielsen
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
// This is a work derived from Microsoft Silverlight Toolkit for 
// Windows Phone, also released under Ms-PL. See: silverlight.codeplex.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Callisto.Effects
{
	/// <summary>
	/// This code provides attached properties for adding a 'tilt' effect to all 
	/// controls within a container.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed", Justification = "Cannot be static and derive from DependencyObject.")]
	public partial class Tilt : DependencyObject
	{
		/// <summary>
		/// Cache of previous cache modes. Not using weak references for now.
		/// </summary>
		private static Dictionary<DependencyObject, CacheMode> _originalCacheMode = new Dictionary<DependencyObject, CacheMode>();

		/// <summary>
		/// Maximum amount of tilt, in radians.
		/// </summary>
		private const double MaxAngle = 0.3;

		/// <summary>
		/// Maximum amount of depression, in pixels
		/// </summary>
		private const double MaxDepression = 25;

		/// <summary>
		/// Delay between releasing an element and the tilt release animation 
		/// playing.
		/// </summary>
		private static readonly TimeSpan TiltReturnAnimationDelay = TimeSpan.FromMilliseconds(200);

		/// <summary>
		/// Duration of tilt release animation.
		/// </summary>
		private static readonly TimeSpan TiltReturnAnimationDuration = TimeSpan.FromMilliseconds(100);

		/// <summary>
		/// The control that is currently being tilted.
		/// </summary>
		private static FrameworkElement currentTiltElement;

		/// <summary>
		/// The single instance of a storyboard used for all tilts.
		/// </summary>
		private static Storyboard tiltReturnStoryboard;

		/// <summary>
		/// The single instance of an X rotation used for all tilts.
		/// </summary>
		private static DoubleAnimation tiltReturnXAnimation;

		/// <summary>
		/// The single instance of a Y rotation used for all tilts.
		/// </summary>
		private static DoubleAnimation tiltReturnYAnimation;

		/// <summary>
		/// The single instance of a Z depression used for all tilts.
		/// </summary>
		private static DoubleAnimation tiltReturnZAnimation;

		/// <summary>
		/// The center of the tilt element.
		/// </summary>
		private static Point currentTiltElementCenter;

		/// <summary>
		/// Whether the animation just completed was for a 'pause' or not.
		/// </summary>
		private static bool wasPauseAnimation = false;

		#region Constructor and Static Constructor
		/// <summary>
		/// This is not a constructable class, but it cannot be static because 
		/// it derives from DependencyObject.
		/// </summary>
		private Tilt()
		{
		}

		#endregion

		#region Dependency properties

		/// <summary>
		/// Whether the tilt effect is enabled on a container (and all its 
		/// children).
		/// </summary>
		public static readonly DependencyProperty IsTiltEnabledProperty = DependencyProperty.RegisterAttached(
		  "IsTiltEnabled",
		  typeof(bool),
		  typeof(Tilt),
		  new PropertyMetadata(false, OnIsTiltEnabledChanged)
		  );

		/// <summary>
		/// Gets the IsTiltEnabled dependency property from an object.
		/// </summary>
		/// <param name="source">The object to get the property from.</param>
		/// <returns>The property's value.</returns>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
		public static bool GetIsTiltEnabled(DependencyObject source)
		{
			return (bool)source.GetValue(IsTiltEnabledProperty);
		}

		/// <summary>
		/// Sets the IsTiltEnabled dependency property on an object.
		/// </summary>
		/// <param name="source">The object to set the property on.</param>
		/// <param name="value">The value to set.</param>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
		public static void SetIsTiltEnabled(DependencyObject source, bool value)
		{
			source.SetValue(IsTiltEnabledProperty, value);
		}

		/// <summary>
		/// Suppresses the tilt effect on a single control that would otherwise 
		/// be tilted.
		/// </summary>
		public static readonly DependencyProperty SuppressTiltProperty = DependencyProperty.RegisterAttached(
		  "SuppressTilt",
		  typeof(bool),
		  typeof(Tilt),
		  null
		  );

		/// <summary>
		/// Gets the SuppressTilt dependency property from an object.
		/// </summary>
		/// <param name="source">The object to get the property from.</param>
		/// <returns>The property's value.</returns>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
		public static bool GetSuppressTilt(DependencyObject source)
		{
			return (bool)source.GetValue(SuppressTiltProperty);
		}

		/// <summary>
		/// Sets the SuppressTilt dependency property from an object.
		/// </summary>
		/// <param name="source">The object to get the property from.</param>
		/// <param name="value">The property's value.</param>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
		public static void SetSuppressTilt(DependencyObject source, bool value)
		{
			source.SetValue(SuppressTiltProperty, value);
		}

		/// <summary>
		/// Property change handler for the IsTiltEnabled dependency property.
		/// </summary>
		/// <param name="target">The element that the property is atteched to.</param>
		/// <param name="args">Event arguments.</param>
		/// <remarks>
		/// Adds or removes event handlers from the element that has been 
		/// (un)registered for tilting.
		/// </remarks>
		private static void OnIsTiltEnabledChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
		{
			FrameworkElement fe = target as FrameworkElement;
			if (fe != null)
			{
				// Add / remove the event handler if necessary
				if ((bool)args.NewValue == true)
				{
					fe.PointerPressed += TiltEffect_PointerPressed;
				}
				else
				{
					fe.PointerPressed -= TiltEffect_PointerPressed;
				}
			}
		}


		#endregion

		#region Top-level manipulation event handlers

		/// <summary>
		/// Event handler for ManipulationStarted.
		/// </summary>
		/// <param name="sender">sender of the event - this will be the tilt 
		/// container (eg, entire page).</param>
		/// <param name="e">Event arguments.</param>
		private static void TiltEffect_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			TryStartTiltEffect(sender as FrameworkElement, e);
		}

		/// <summary>
		/// Event handler for ManipulationDelta
		/// </summary>
		/// <param name="sender">sender of the event - this will be the tilting 
		/// object (eg a button).</param>
		/// <param name="e">Event arguments.</param>
		private static void TiltEffect_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			ContinueTiltEffect(sender as FrameworkElement, e);
		}

		/// <summary>
		/// Event handler for ManipulationCompleted.
		/// </summary>
		/// <param name="sender">sender of the event - this will be the tilting 
		/// object (eg a button).</param>
		/// <param name="e">Event arguments.</param>
		private static void TiltEffect_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			EndTiltEffect(currentTiltElement);
		}

		/// <summary>
		/// Event handler for PointerCaptureLost
		/// </summary>
		/// <param name="sender">sender of the event - this will be the tilting 
		/// object (eg a button).</param>
		/// <param name="e">Event arguments.</param>
		private static void TiltEffect_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
		{
			EndTiltEffect(currentTiltElement);
		}

		#endregion

		#region Core tilt logic

		/// <summary>
		/// Checks if the manipulation should cause a tilt, and if so starts the 
		/// tilt effect.
		/// </summary>
		/// <param name="source">The source of the manipulation (the tilt 
		/// container, eg entire page).</param>
		/// <param name="e">The args from the ManipulationStarted event.</param>
		private static void TryStartTiltEffect(FrameworkElement source, PointerRoutedEventArgs e)
		{
			FrameworkElement element = source; // VisualTreeHelper.GetChild(ancestor, 0) as FrameworkElement;
			FrameworkElement container = source;// e.Container as FrameworkElement;

			if (element == null || container == null)
				return;

			// Touch point relative to the element being tilted.
			Point tiltTouchPoint = e.GetCurrentPoint(element).Position; // container.TransformToVisual(element).TransformPoint(e.GetCurrentPoint(element));

			// Center of the element being tilted.
			Point elementCenter = new Point(element.ActualWidth / 2, element.ActualHeight / 2);

			// Camera adjustment.
			Point centerToCenterDelta = GetCenterToCenterDelta(element, source);

			BeginTiltEffect(element, tiltTouchPoint, elementCenter, centerToCenterDelta, e.Pointer);
			return;
		}

		/// <summary>
		/// Computes the delta between the centre of an element and its 
		/// container.
		/// </summary>
		/// <param name="element">The element to compare.</param>
		/// <param name="container">The element to compare against.</param>
		/// <returns>A point that represents the delta between the two centers.</returns>
		private static Point GetCenterToCenterDelta(FrameworkElement element, FrameworkElement container)
		{
			Point elementCenter = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
			Point containerCenter = new Point(container.ActualWidth / 2, container.ActualHeight / 2);
			Point transformedElementCenter = element.TransformToVisual(container).TransformPoint(elementCenter);
			return new Point(containerCenter.X - transformedElementCenter.X, containerCenter.Y - transformedElementCenter.Y);
		}

		/// <summary>
		/// Begins the tilt effect by preparing the control and doing the 
		/// initial animation.
		/// </summary>
		/// <param name="element">The element to tilt.</param>
		/// <param name="touchPoint">The touch point, in element coordinates.</param>
		/// <param name="centerPoint">The center point of the element in element
		/// coordinates.</param>
		/// <param name="centerDelta">The delta between the 
		/// <paramref name="element"/>'s center and the container's center.</param>
		private static void BeginTiltEffect(FrameworkElement element, Point touchPoint, Point centerPoint, Point centerDelta, Pointer p)
		{
			if (tiltReturnStoryboard != null)
			{
				StopTiltReturnStoryboardAndCleanup();
			}

			if (PrepareControlForTilt(element, centerDelta, p) == false)
			{
				return;
			}

			currentTiltElement = element;
			currentTiltElementCenter = centerPoint;
			PrepareTiltReturnStoryboard(element);

			ApplyTiltEffect(currentTiltElement, touchPoint, currentTiltElementCenter);
		}

		/// <summary>
		/// Prepares a control to be tilted by setting up a plane projection and
		/// some event handlers.
		/// </summary>
		/// <param name="element">The control that is to be tilted.</param>
		/// <param name="centerDelta">Delta between the element's center and the
		/// tilt container's.</param>
		/// <returns>true if successful; false otherwise.</returns>
		/// <remarks>
		/// This method is conservative; it will fail any attempt to tilt a 
		/// control that already has a projection on it.
		/// </remarks>
		private static bool PrepareControlForTilt(FrameworkElement element, Point centerDelta, Pointer p)
		{
			// Prevents interference with any existing transforms
			if (element.Projection != null || (element.RenderTransform != null && element.RenderTransform.GetType() != typeof(MatrixTransform)))
			{
				return false;
			}

			_originalCacheMode[element] = element.CacheMode;
			element.CacheMode = new BitmapCache();

			TranslateTransform transform = new TranslateTransform();
			transform.X = centerDelta.X;
			transform.Y = centerDelta.Y;
			element.RenderTransform = transform;

			PlaneProjection projection = new PlaneProjection();
			projection.GlobalOffsetX = -1 * centerDelta.X;
			projection.GlobalOffsetY = -1 * centerDelta.Y;
			element.Projection = projection;

			element.PointerMoved += TiltEffect_PointerMoved;
			element.PointerReleased += TiltEffect_PointerReleased;
			element.PointerCaptureLost += TiltEffect_PointerCaptureLost;
			element.CapturePointer(p);
			return true;
		}

		/// <summary>
		/// Removes modifications made by PrepareControlForTilt.
		/// </summary>
		/// <param name="element">THe control to be un-prepared.</param>
		/// <remarks>
		/// This method is basic; it does not do anything to detect if the 
		/// control being un-prepared was previously prepared.
		/// </remarks>
		private static void RevertPrepareControlForTilt(FrameworkElement element)
		{
			element.PointerMoved -= TiltEffect_PointerMoved;
			element.PointerReleased -= TiltEffect_PointerReleased;
			element.PointerCaptureLost -= TiltEffect_PointerCaptureLost;
			element.Projection = null;
			element.RenderTransform = null;

			CacheMode original;
			if (_originalCacheMode.TryGetValue(element, out original))
			{
				element.CacheMode = original;
				_originalCacheMode.Remove(element);
			}
			else
			{
				element.CacheMode = null;
			}
		}

		/// <summary>
		/// Creates the tilt return storyboard (if not already created) and 
		/// targets it to the projection.
		/// </summary>
		/// <param name="element">The framework element to prepare for
		/// projection.</param>
		private static void PrepareTiltReturnStoryboard(FrameworkElement element)
		{
			if (tiltReturnStoryboard == null)
			{
				tiltReturnStoryboard = new Storyboard();
				tiltReturnStoryboard.Completed += TiltReturnStoryboard_Completed;
				tiltReturnXAnimation = new DoubleAnimation();
				Storyboard.SetTargetProperty(tiltReturnXAnimation, "RotationX");
				tiltReturnXAnimation.BeginTime = TiltReturnAnimationDelay;
				tiltReturnXAnimation.To = 0;
				tiltReturnXAnimation.Duration = TiltReturnAnimationDuration;

				tiltReturnYAnimation = new DoubleAnimation();
				Storyboard.SetTargetProperty(tiltReturnYAnimation, "RotationY");
				tiltReturnYAnimation.BeginTime = TiltReturnAnimationDelay;
				tiltReturnYAnimation.To = 0;
				tiltReturnYAnimation.Duration = TiltReturnAnimationDuration;

				tiltReturnZAnimation = new DoubleAnimation();
				Storyboard.SetTargetProperty(tiltReturnZAnimation, "GlobalOffsetZ");
				tiltReturnZAnimation.BeginTime = TiltReturnAnimationDelay;
				tiltReturnZAnimation.To = 0;
				tiltReturnZAnimation.Duration = TiltReturnAnimationDuration;

				tiltReturnStoryboard.Children.Add(tiltReturnXAnimation);
				tiltReturnStoryboard.Children.Add(tiltReturnYAnimation);
				tiltReturnStoryboard.Children.Add(tiltReturnZAnimation);
			}

			Storyboard.SetTarget(tiltReturnXAnimation, element.Projection);
			Storyboard.SetTarget(tiltReturnYAnimation, element.Projection);
			Storyboard.SetTarget(tiltReturnZAnimation, element.Projection);
		}

		/// <summary>
		/// Continues a tilt effect that is currently applied to an element, 
		/// presumably because the user moved their finger.
		/// </summary>
		/// <param name="element">The element being tilted.</param>
		/// <param name="e">The manipulation event args.</param>
		private static void ContinueTiltEffect(FrameworkElement element, PointerRoutedEventArgs e)
		{
			FrameworkElement container = element;
			if (container == null || element == null)
			{
				return;
			}

			Point tiltTouchPoint =  e.GetCurrentPoint(element).Position;

			// If touch moved outside bounds of element, then pause the tilt 
			// (but don't cancel it)
			if (new Rect(0, 0, currentTiltElement.ActualWidth, currentTiltElement.ActualHeight).Contains(tiltTouchPoint) != true)
			{
				PauseTiltEffect();
			}
			else
			{
				// Apply the updated tilt effect
				ApplyTiltEffect(currentTiltElement, tiltTouchPoint, currentTiltElementCenter);
			}
		}

		/// <summary>
		/// Ends the tilt effect by playing the animation.
		/// </summary>
		/// <param name="element">The element being tilted.</param>
		private static void EndTiltEffect(FrameworkElement element)
		{
			if (element != null)
			{
				element.PointerReleased -= TiltEffect_PointerPressed;
				element.PointerMoved -= TiltEffect_PointerMoved;
			}

			if (tiltReturnStoryboard != null)
			{
				wasPauseAnimation = false;
				if (tiltReturnStoryboard.GetCurrentState() != ClockState.Active)
				{
					tiltReturnStoryboard.Begin();
				}
			}
			else
			{
				StopTiltReturnStoryboardAndCleanup();
			}
		}

		/// <summary>
		/// Handler for the storyboard complete event.
		/// </summary>
		/// <param name="sender">sender of the event.</param>
		/// <param name="e">event args.</param>
		private static void TiltReturnStoryboard_Completed(object sender, object e)
		{
			if (wasPauseAnimation)
			{
				ResetTiltEffect(currentTiltElement);
			}
			else
			{
				StopTiltReturnStoryboardAndCleanup();
			}
		}

		/// <summary>
		/// Resets the tilt effect on the control, making it appear 'normal'
		/// again.
		/// </summary>
		/// <param name="element">The element to reset the tilt on.</param>
		/// <remarks>
		/// This method doesn't turn off the tilt effect or cancel any current
		/// manipulation; it just temporarily cancels the effect.
		/// </remarks>
		private static void ResetTiltEffect(FrameworkElement element)
		{
			PlaneProjection projection = element.Projection as PlaneProjection;
			projection.RotationY = 0;
			projection.RotationX = 0;
			projection.GlobalOffsetZ = 0;
		}

		/// <summary>
		/// Stops the tilt effect and release resources applied to the currently
		/// tilted control.
		/// </summary>
		private static void StopTiltReturnStoryboardAndCleanup()
		{
			if (tiltReturnStoryboard != null)
			{
				tiltReturnStoryboard.Stop();
			}

			RevertPrepareControlForTilt(currentTiltElement);
		}

		/// <summary>
		/// Pauses the tilt effect so that the control returns to the 'at rest'
		/// position, but doesn't stop the tilt effect (handlers are still 
		/// attached).
		/// </summary>
		private static void PauseTiltEffect()
		{
			if ((tiltReturnStoryboard != null) && !wasPauseAnimation)
			{
				tiltReturnStoryboard.Stop();
				wasPauseAnimation = true;
				tiltReturnStoryboard.Begin();
			}
		}

		/// <summary>
		/// Resets the storyboard to not running.
		/// </summary>
		private static void ResetTiltReturnStoryboard()
		{
			tiltReturnStoryboard.Stop();
			wasPauseAnimation = false;
		}

		/// <summary>
		/// Applies the tilt effect to the control.
		/// </summary>
		/// <param name="element">the control to tilt.</param>
		/// <param name="touchPoint">The touch point, in the container's 
		/// coordinates.</param>
		/// <param name="centerPoint">The center point of the container.</param>
		private static void ApplyTiltEffect(FrameworkElement element, Point touchPoint, Point centerPoint)
		{
			// Stop any active animation
			ResetTiltReturnStoryboard();

			// Get relative point of the touch in percentage of container size
			Point normalizedPoint = new Point(
				Math.Min(Math.Max(touchPoint.X / (centerPoint.X * 2), 0), 1),
				Math.Min(Math.Max(touchPoint.Y / (centerPoint.Y * 2), 0), 1));

			if (double.IsNaN(normalizedPoint.X) || double.IsNaN(normalizedPoint.Y))
			{
				return;
			}

			// Shell values
			double xMagnitude = Math.Abs(normalizedPoint.X - 0.5);
			double yMagnitude = Math.Abs(normalizedPoint.Y - 0.5);
			double xDirection = -Math.Sign(normalizedPoint.X - 0.5);
			double yDirection = Math.Sign(normalizedPoint.Y - 0.5);
			double angleMagnitude = xMagnitude + yMagnitude;
			double xAngleContribution = xMagnitude + yMagnitude > 0 ? xMagnitude / (xMagnitude + yMagnitude) : 0;

			double angle = angleMagnitude * MaxAngle * 180 / Math.PI;
			double depression = (1 - angleMagnitude) * MaxDepression;

			// RotationX and RotationY are the angles of rotations about the x- 
			// or y-*axis*; to achieve a rotation in the x- or y-*direction*, we 
			// need to swap the two. That is, a rotation to the left about the 
			// y-axis is a rotation to the left in the x-direction, and a 
			// rotation up about the x-axis is a rotation up in the y-direction.
			PlaneProjection projection = element.Projection as PlaneProjection;
			projection.RotationY = angle * xAngleContribution * xDirection;
			projection.RotationX = angle * (1 - xAngleContribution) * yDirection;
			projection.GlobalOffsetZ = -depression;
		}

		#endregion
	}

	/// <summary>
	/// Couple of simple helpers for walking the visual tree.
	/// </summary>
	static class TreeHelpers
	{
		/// <summary>
		/// Gets the ancestors of the element, up to the root.
		/// </summary>
		/// <param name="node">The element to start from.</param>
		/// <returns>An enumerator of the ancestors.</returns>
		public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
		{
			FrameworkElement parent = node.GetVisualParent();
			while (parent != null)
			{
				yield return parent;
				parent = parent.GetVisualParent();
			}
		}

		/// <summary>
		/// Gets the visual parent of the element.
		/// </summary>
		/// <param name="node">The element to check.</param>
		/// <returns>The visual parent.</returns>
		public static FrameworkElement GetVisualParent(this FrameworkElement node)
		{
			return VisualTreeHelper.GetParent(node) as FrameworkElement;
		}
	}
}

