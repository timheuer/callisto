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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Callisto.Controls
{
    /// <summary>
    /// The main Flyout control to host any popup/flyout content.
    /// </summary>
    public sealed class Flyout : ContentControl, IDisposable
    {
        #region Member Variables
        // the inner Popup control
        private Popup _hostPopup;

        // the bounds of the current window
        private Rect _windowBounds;

        // the current Window 
        private UIElement _rootVisual;

        // the 'real' placement mode based on calculations
        private PlacementMode _realizedPlacement;

        private bool _ihmFocusMoved = false;
        private double _ihmOccludeHeight = 0.0;
        #endregion Member Variables

        #region Constants
        // This is the edge gutter when positioned against left/right/top/bottom or element edges 
        // should be 6px, but that doesn't take into account focus rects
        // TODO: Revisit gutter constant if needs to be less due to focus rects/touch targets
        const double GUTTER_BUFFER = 5.0;
        const double GUTTER_BUFFER_ADJUSTED = 4.0; // for element gutter due to focus rects
        #endregion Constants

        #region Public Properties
        // public accessor for the inner Popup
        public Popup HostPopup
        {
            get { return _hostPopup; }
        }
        #endregion Public Properties

        /// <summary>
        ///  Flyout constructor
        /// </summary>
        public Flyout()
        {
            this.DefaultStyleKey = typeof(Flyout);

            Window.Current.Activated += OnCurrentWindowActivated;
            Window.Current.SizeChanged += OnCurrentWindowSizeChanged;

            // set the default placement
            this.Placement = PlacementMode.Top;

            _windowBounds = Window.Current.Bounds;
            _rootVisual = Window.Current.Content;

            this.Loaded += OnLoaded;

            _hostPopup = new Popup {IsHitTestVisible = false, Opacity = 0};
            _hostPopup.Closed += OnHostPopupClosed;
            _hostPopup.Opened += OnHostPopupOpened;
            _hostPopup.IsLightDismissEnabled = true;
            _hostPopup.Child = this;
        }

        private void OnCurrentWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            IsOpen = false;
        }

        private void OnCurrentWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                IsOpen = false;
            }
        }

        #region Methods and Events
        private void OnHostPopupOpened(object sender, object e)
        {
            // there seems to be some issue where the mesaure is happening too fast when the size
            // of the flyout is 0,0.  This attempts to solve this. 
            // credit to avidgator/jvlppm for suggestions on github project
            if (_hostPopup.ActualHeight == 0 || _hostPopup.ActualWidth == 0)
            {
                SizeChangedEventHandler updatePosition = null;
                updatePosition = (s, eP) =>
                    {
                        if (eP.NewSize.Width != 0 && eP.NewSize.Height != 0)
                        {
                            OnHostPopupOpened(s, eP);
                            SizeChanged -= updatePosition;
                        }
                    };
                SizeChanged += updatePosition;
            }

            _hostPopup.HorizontalOffset = this.HorizontalOffset;
            _hostPopup.VerticalOffset = this.VerticalOffset;

            Measure(new Size(Double.PositiveInfinity, double.PositiveInfinity));

            PerformPlacement(this.HorizontalOffset, this.VerticalOffset);

            // handling the case where it isn't parented to the visual tree
            // inspect the visual root and adjust.
            if (_hostPopup.Parent == null)
            {
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().Showing += OnInputPaneShowing;
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().Hiding += OnInputPaneHiding;
            }

            var content = Content as Control;
            if (content != null)
                content.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void OnInputPaneHiding(Windows.UI.ViewManagement.InputPane sender, Windows.UI.ViewManagement.InputPaneVisibilityEventArgs args)
        {
            // if the ihm occluded something and we had to move, we need to adjust back
            if (_ihmFocusMoved)
            {
                _hostPopup.VerticalOffset += _ihmOccludeHeight; // ensure defaults back to normal
                _ihmFocusMoved = false;
            }
        }

        private void OnInputPaneShowing(Windows.UI.ViewManagement.InputPane sender, Windows.UI.ViewManagement.InputPaneVisibilityEventArgs args)
        {
            FrameworkElement focusedItem = FocusManager.GetFocusedElement() as FrameworkElement;

            if (focusedItem != null)
            {
                // if the focused item is within height - occludedrect height - buffer(50)
                // then it doesn't need to be changed
                GeneralTransform gt = focusedItem.TransformToVisual(Window.Current.Content);
                
                Rect focusedRect = gt.TransformBounds(new Rect(0.0, 0.0, focusedItem.ActualWidth, focusedItem.ActualHeight));

                if (focusedRect.Bottom > (_windowBounds.Height - args.OccludedRect.Top))
                {
                    _ihmFocusMoved = true;
                    _ihmOccludeHeight = focusedRect.Top < (int)args.OccludedRect.Top ? focusedRect.Top : args.OccludedRect.Top;
                    _hostPopup.VerticalOffset -= _ihmOccludeHeight;
                }
            }  
        }

        private static Rect GetBounds(params Point[] interestPoints)
        {
            double num2;
            double num4;
            double x = num2 = interestPoints[0].X;
            double y = num4 = interestPoints[0].Y;
            for (int i = 1; i < interestPoints.Length; i++)
            {
                double num6 = interestPoints[i].X;
                double num7 = interestPoints[i].Y;
                if (num6 < x)
                {
                    x = num6;
                }
                if (num6 > num2)
                {
                    num2 = num6;
                }
                if (num7 < y)
                {
                    y = num7;
                }
                if (num7 > num4)
                {
                    num4 = num7;
                }
            }
            return new Rect(x, y, (num2 - x) + 1.0, (num4 - y) + 1.0);
        }

        private Point PlacePopup(Rect window, Point[] target, Point[] flyout, PlacementMode placement)
        {
            Point[] pointArray;
            double y = 0.0;
            double x = 0.0;
            Rect bounds = GetBounds(target);
            Rect rect2 = GetBounds(flyout);
            double width = rect2.Width;
            double height = rect2.Height;
            if (placement == PlacementMode.Right)
            {
                double num5 = Math.Max(0.0, target[0].X);
                double num6 = window.Width - Math.Min(window.Width, target[1].X + 1.0);
                if ((num6 < width) && (num6 < num5))
                {
                    placement = PlacementMode.Left;
                }
            }
            else if (placement == PlacementMode.Left)
            {
                double num7 = window.Width - Math.Min(window.Width, target[1].X + 1.0);
                double num8 = Math.Max(0.0, target[0].X);
                if ((num8 < width) && (num8 < num7))
                {
                    placement = PlacementMode.Right;
                }
            }
            else if (placement == PlacementMode.Top)
            {
                double num9 = Math.Max(0.0, target[0].Y);
                double num10 = window.Height - Math.Min(window.Height, target[2].Y + 1.0);
                if ((num9 < height) && (num9 < num10))
                {
                    placement = PlacementMode.Bottom;
                }
            }
            else if (placement == PlacementMode.Bottom)
            {
                double num11 = Math.Max(0.0, target[0].Y);
                double num12 = window.Height - Math.Min(window.Height, target[2].Y + 1.0);
                if ((num12 < height) && (num12 < num11))
                {
                    placement = PlacementMode.Top;
                }
            }
            switch (placement)
            {
                case PlacementMode.Bottom:
                    pointArray = new Point[] { new Point(target[2].X, Math.Max((double)0.0, (double)(target[2].Y + 1.0))), new Point((target[3].X - width) + 1.0, Math.Max((double)0.0, (double)(target[2].Y + 1.0))), new Point(0.0, Math.Max((double)0.0, (double)(target[2].Y + 1.0))) };
                    break;

                case PlacementMode.Right:
                    pointArray = new Point[] { new Point(Math.Max((double)0.0, (double)(target[1].X + 1.0)), target[1].Y), new Point(Math.Max((double)0.0, (double)(target[3].X + 1.0)), (target[3].Y - height) + 1.0), new Point(Math.Max((double)0.0, (double)(target[1].X + 1.0)), 0.0) };
                    break;

                case PlacementMode.Left:
                    pointArray = new Point[] { new Point(Math.Min(window.Width, target[0].X) - width, target[1].Y), new Point(Math.Min(window.Width, target[2].X) - width, (target[3].Y - height) + 1.0), new Point(Math.Min(window.Width, target[0].X) - width, 0.0) };
                    break;

                case PlacementMode.Top:
                    pointArray = new Point[] { new Point(target[0].X, Math.Min(target[0].Y, window.Height) - height), new Point((target[1].X - width) + 1.0, Math.Min(target[0].Y, window.Height) - height), new Point(0.0, Math.Min(target[0].Y, window.Height) - height) };
                    break;

                default:
                    pointArray = new Point[] { new Point(0.0, 0.0) };
                    break;
            }
            double num13 = width * height;
            int index = 0;
            double num15 = 0.0;
            for (int i = 0; i < pointArray.Length; i++)
            {
                Rect rect3 = new Rect(pointArray[i].X, pointArray[i].Y, width, height);
                rect3.Intersect(window);
                double d = rect3.Width * rect3.Height;
                if (double.IsInfinity(d))
                {
                    index = pointArray.Length - 1;
                    break;
                }
                if (d > num15)
                {
                    index = i;
                    num15 = d;
                }
                if (d == num13)
                {
                    index = i;
                    break;
                }
            }
            // x = pointArray[index].X;
            x = pointArray[0].X; // TODO: taking this solves my horizontal nudging, but is a hack...keeping it though until a better solution
            y = pointArray[index].Y;
            if (index > 1)
            {
                if ((placement == PlacementMode.Left) || (placement == PlacementMode.Right))
                {
                    if (((y != target[0].Y) && (y != target[1].Y)) && (((y + height) != target[0].Y) && ((y + height) != target[1].Y)))
                    {
                        double num18 = bounds.Top + (bounds.Height / 2.0);
                        if ((num18 > 0.0) && ((num18 - 0.0) > (window.Height - num18)))
                        {
                            y = window.Height - height;
                        }
                        else
                        {
                            y = 0.0;
                        }
                    }
                }
                else if (((placement == PlacementMode.Top) || (placement == PlacementMode.Bottom)) && (((x != target[0].X) && (x != target[1].X)) && (((x + width) != target[0].X) && ((x + width) != target[1].X))))
                {
                    double num19 = bounds.Left + (bounds.Width / 2.0);
                    if ((num19 > 0.0) && ((num19 - 0.0) > (window.Width - num19)))
                    {
                        x = window.Width - width;
                    }
                    else
                    {
                        x = 0.0;
                    }
                }
            }

            _realizedPlacement = placement;

            return new Point(x, y);
        }

        private Point[] GetTransformedPoints(FrameworkElement element, bool isRTL, FrameworkElement relativeTo)
        {
            Point[] pointArray = new Point[4];
            if ((element != null) && (relativeTo != null))
            {
                GeneralTransform gt = relativeTo.TransformToVisual(_rootVisual);
                pointArray[0] = gt.TransformPoint(new Point(0.0, 0.0));
                pointArray[1] = gt.TransformPoint(new Point(element.ActualWidth, 0.0));
                pointArray[2] = gt.TransformPoint(new Point(0.0, element.ActualHeight));
                pointArray[3] = gt.TransformPoint(new Point(element.ActualWidth, element.ActualHeight));

                FrameworkElement _el = _rootVisual as FrameworkElement;
                bool flag = (_el != null) ? (_el.FlowDirection == FlowDirection.RightToLeft) : false;
                if (isRTL != flag)
                {
                    // TODO: Handle RTL - GetTransformedPoints
                    //for (int i = 0; i < pointArray.Length; i++)
                    //{
                    //    pointArray[i].X = _windowBounds.Width - pointArray[i].X;
                    //}
                }
            }
            return pointArray;
        }

        private void PerformPlacement(double horizontalOffset, double verticalOffset)
        {
            double x = 0.0;
            double y = 0.0;
            PlacementMode placement = this.Placement;
            FrameworkElement element = this.PlacementTarget as FrameworkElement;
            bool isRTL = (element != null) ? (element.FlowDirection == Windows.UI.Xaml.FlowDirection.RightToLeft) : false;

            if ((element != null) && !element.IsHitTestVisible)
            {
                return;
            }

            switch (placement)
            {
                case PlacementMode.Bottom:
                case PlacementMode.Left:
                case PlacementMode.Right:
                case PlacementMode.Top:
                    Point[] target = GetTransformedPoints(element, isRTL, element);
                    Point[] menu = GetTransformedPoints((FrameworkElement)_hostPopup.Child, isRTL, element);
                    if (menu[0].X > menu[1].X)
                    {
                        return;
                    }
                    Point p2 = PlacePopup(_windowBounds, target, menu, placement);
                    x = p2.X;
                    if (isRTL)
                    {
                        // TODO: Handle RTL - PerformPlacement
                        //x = _windowBounds.Width - x;
                        //this._hostPopup.VerticalOffset = y;
                        //this._hostPopup.HorizontalOffset = x;
                        //return;
                    }
                    y = p2.Y;
                    break;
                case PlacementMode.Mouse:
                    throw new NotImplementedException("Mouse PlacementMode is not implemented.");
            }

            if (x < 0.0) x = 0.0;

            if (y < 0.0) y = 0.0;

            
            var calcH = this.CalculateHorizontalCenterOffset(x, ((FrameworkElement)_hostPopup.Child).ActualWidth, element.ActualWidth);
            var calcY = this.CalculateVerticalCenterOffset(y, ((FrameworkElement)_hostPopup.Child).ActualHeight, element.ActualHeight);

            if (calcH < 0)
            {
                calcH = GUTTER_BUFFER;
            }
            else
            {
                // TODO: Correct right nudge positioning as it is incorrect
                if ((calcH > _windowBounds.Width) || (calcH + ((FrameworkElement)_hostPopup.Child).ActualWidth) > _windowBounds.Width)
                {
                    calcH = _windowBounds.Width - ((FrameworkElement)_hostPopup.Child).ActualWidth - GUTTER_BUFFER;
                }
            }

            UIElement parent = _hostPopup.Parent as UIElement;
            if (parent != null)
            {
                var transform = parent.TransformToVisual(Window.Current.Content);
                var offsetAdjustment = transform.TransformPoint(new Point(0, 0));
                calcH -= offsetAdjustment.X;
                calcY -= offsetAdjustment.Y;
            }

            _hostPopup.HorizontalOffset = calcH;
            _hostPopup.VerticalOffset = calcY;
            _hostPopup.IsHitTestVisible = true;
            _hostPopup.Opacity = 1;

            // for entrance animation
            // UX guidelines show a PopIn animation
            Storyboard inAnimation = new Storyboard();
            PopInThemeAnimation popin = new PopInThemeAnimation();

            // TODO: Switch statement begs of refactoring
            switch (this.Placement)
            {
                case PlacementMode.Bottom:
                    popin.FromVerticalOffset = -10;
                    popin.FromHorizontalOffset = 0;
                    break;
                case PlacementMode.Left:
                    popin.FromVerticalOffset = 0;
                    popin.FromHorizontalOffset = 10;
                    break;
                case PlacementMode.Right:
                    popin.FromVerticalOffset = 0;
                    popin.FromHorizontalOffset = -10;
                    break;
                case PlacementMode.Top:
                    popin.FromVerticalOffset = 10;
                    popin.FromHorizontalOffset = 0;
                    break;
            }
            Storyboard.SetTarget(popin, _hostPopup);
            inAnimation.Children.Add(popin);
            inAnimation.Begin();
        }

        #region Offset Calculations
        private double CalculateHorizontalCenterOffset(double initialOffset, double flyoutWidth, double elementWidth)
        {
            double newX = 0.0;

            if (_realizedPlacement == PlacementMode.Top || _realizedPlacement == PlacementMode.Bottom)
            {
                newX = this.HorizontalOffset + initialOffset - ((flyoutWidth / 2) - (elementWidth / 2));
            }
            else
            {
                newX = this.HorizontalOffset + initialOffset;
            }
            return newX;
        }

        private double CalculateVerticalCenterOffset(double initialOffset, double flyoutHeight, double elementHeight)
        {
            double newY = 0.0;

            if (_realizedPlacement == PlacementMode.Top || _realizedPlacement == PlacementMode.Bottom)
            {
                newY = this.VerticalOffset + initialOffset;
            }
            else
            {
                newY = this.VerticalOffset + initialOffset - (flyoutHeight / 2) + (elementHeight / 2);
            }
            return CalculateGutter(newY);
        }

        private double CalculateGutter(double p)
        {
            double trueOffset = p;

            // TODO: Need to fixt Left/Right gutter adjustments
            switch (_realizedPlacement)
            {
                case PlacementMode.Bottom:
                    trueOffset = p + GUTTER_BUFFER_ADJUSTED;
                    break;
                case PlacementMode.Left:
                    trueOffset = p - GUTTER_BUFFER_ADJUSTED;
                    break;
                case PlacementMode.Right:
                    trueOffset = p + GUTTER_BUFFER_ADJUSTED;
                    break;
                case PlacementMode.Top:
                    trueOffset = p - GUTTER_BUFFER_ADJUSTED;
                    break;
                default:
                    break;
            }

            return trueOffset;
        } 
        #endregion Offset Calculations

        private void OnHostPopupClosed(object sender, object e)
        {
            // important to remove this or else there will be a leak
            Window.Current.Activated -= OnCurrentWindowActivated;
            Window.Current.SizeChanged -= OnCurrentWindowSizeChanged;
            Windows.UI.ViewManagement.InputPane.GetForCurrentView().Showing -= OnInputPaneShowing;
            Windows.UI.ViewManagement.InputPane.GetForCurrentView().Hiding -= OnInputPaneHiding;

            if (Closed != null)
            {
                Closed(this, e);
            }

            IsOpen = false;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((Flyout)sender).IsHitTestVisible = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Runtime EventHandler")]
        public event EventHandler<object> Closed;
        #endregion Methods and Events

        #region Dependency Properties
        public Thickness HostMargin
        {
            get { return (Thickness)GetValue(HostMarginProperty); }
            set { SetValue(HostMarginProperty, value); }
        }

        public static readonly DependencyProperty HostMarginProperty =
            DependencyProperty.Register("HostMargin", typeof(Thickness), typeof(Flyout), new PropertyMetadata(0));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new PropertyMetadata(false, (obj, args) =>
            {
                if (args.NewValue != args.OldValue)
                {
                    Flyout f = (Flyout)obj;
                    f._hostPopup.IsOpen = (bool)args.NewValue;
                }
            }));

        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register("PlacementTarget", typeof(UIElement), typeof(Flyout), null);

        public Windows.UI.Xaml.Controls.Primitives.PlacementMode Placement
        {
            get { return (Windows.UI.Xaml.Controls.Primitives.PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(Windows.UI.Xaml.Controls.Primitives.PlacementMode), typeof(Flyout), null);

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Flyout), new PropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Flyout), new PropertyMetadata(0.0));
        #endregion Dependency Properties

        public void Dispose()
        {
            if (this._hostPopup != null)
            {
                this._hostPopup.Child = null;
                this._hostPopup.ChildTransitions = null;

                this.Content = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
