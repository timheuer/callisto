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

using Callisto.Controls.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    /// <summary>
    /// A control that has a rating.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePointerPressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateReadOnly, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RatingItem))]
    public class Rating : ItemsControl, IUpdateVisualState
    {
        #region protected double DisplayValue
        /// <summary>
        /// Gets or sets the actual value of the Rating control.
        /// </summary>
        protected double DisplayValue
        {
            get { return (double)GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayValue dependency property.
        /// </summary>
        protected static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(
                "DisplayValue",
                typeof(double),
                typeof(Rating),
                new PropertyMetadata(0.0, OnDisplayValueChanged));

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">Rating that changed its DisplayValue.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnDisplayValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            Rating source = (Rating)dependencyObject;
            source.OnDisplayValueChanged();
        }

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary>
        private void OnDisplayValueChanged()
        {
            UpdateDisplayValues();
        }

        #endregion protected double DisplayValue

        /// <summary>
        /// Gets or sets the rating item hovered over.
        /// </summary>
        private RatingItem HoveredRatingItem { get; set; }

        /// <summary>
        /// Gets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        internal InteractionHelper Interaction { get; private set; }

        /// <summary>
        /// Gets or sets the items control helper class.
        /// </summary>
        private ItemsControlHelper ItemsControlHelper { get; set; }

        #region public int ItemCount
        /// <summary>
        /// Gets or sets the number of rating items.
        /// </summary>
        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register(
                "ItemCount",
                typeof(int),
                typeof(Rating),
                new PropertyMetadata(0, OnItemCountChanged));

        /// <summary>
        /// ItemCountProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its ItemCount.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = d as Rating;
            int value = (int)e.NewValue;
            source.OnItemCountChanged(value);
        }

        /// <summary>
        /// This method is invoked when the items count property is changed.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnItemCountChanged(int newValue)
        {
            if (newValue < 0)
            {
                throw new ArgumentException("Value must be larger than or equal to 0.");
            }

            int amountToAdd = newValue - this.Items.Count;
            if (amountToAdd > 0)
            {
                for (int cnt = 0; cnt < amountToAdd; cnt++)
                {
                    this.Items.Add(new RatingItem());
                }
            }
            else if (amountToAdd < 0)
            {
                for (int cnt = 0; cnt < Math.Abs(amountToAdd); cnt++)
                {
                    this.Items.RemoveAt(this.Items.Count - 1);
                }
            }
        }
        #endregion public int ItemCount

        #region public Style ItemContainerStyle
        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its ItemContainerStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            Style newValue = (Style)e.NewValue;
            source.OnItemContainerStyleChanged(newValue);
        }

        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnItemContainerStyleChanged(Style newValue)
        {
            ItemsControlHelper.UpdateItemContainerStyle(newValue);
        }
        #endregion public Style ItemContainerStyle

        #region public RatingSelectionMode SelectionMode
        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        public RatingSelectionMode SelectionMode
        {
            get { return (RatingSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(RatingSelectionMode),
                typeof(Rating),
                new PropertyMetadata(RatingSelectionMode.Continuous, OnSelectionModeChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its SelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            RatingSelectionMode oldValue = (RatingSelectionMode)e.OldValue;
            RatingSelectionMode newValue = (RatingSelectionMode)e.NewValue;
            source.OnSelectionModeChanged(oldValue, newValue);
        }

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnSelectionModeChanged(RatingSelectionMode oldValue, RatingSelectionMode newValue)
        {
            UpdateDisplayValues();
        }
        #endregion public RatingSelectionMode SelectionMode

        #region public double Value
        /// <summary>
        /// Gets or sets the rating value.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Value is the logical name for this property.")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double WeightedValue { get; private set; }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(Rating),
                new PropertyMetadata(0.0, OnValueChanged));

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its Value.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            source.OnValueChanged(oldValue, newValue);
        }

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            WeightedValue = newValue / ItemCount;

            UpdateValues();

            ValueChangedEventHandler<double> handler = ValueChanged;
            if (handler != null)
            {
                handler(this, new ValueChangedEventArgs<double>(oldValue, newValue));
            }
        }

        /// <summary>
        /// Updates the control when the items change.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnItemsChanged(object e)
        {
            //EventHandler<object> layoutUpdated = null;
            //layoutUpdated =
            //    delegate
            //    {
            //        this.LayoutUpdated -= layoutUpdated;
            //        UpdateValues();
            //        UpdateDisplayValues();
            //    };
            //this.LayoutUpdated += layoutUpdated;

            this.ItemCount = this.Items.Count;

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// This event is raised when the value of the rating is changed.
        /// </summary>
        public event ValueChangedEventHandler<double> ValueChanged;

        #endregion public double Value

        #region PointerPressedFill
        public SolidColorBrush PointerPressedFill
        {
            get { return (SolidColorBrush)GetValue(PointerPressedFillProperty); }
            set { SetValue(PointerPressedFillProperty, value); }
        }

        public static readonly DependencyProperty PointerPressedFillProperty =
            DependencyProperty.Register("PointerPressedFill", typeof(SolidColorBrush), typeof(Rating), null);
        #endregion PoitnerPressedFill

        #region PointerOverFill
        public SolidColorBrush PointerOverFill
        {
            get { return (SolidColorBrush)GetValue(PointerOverFillProperty); }
            set { SetValue(PointerOverFillProperty, value); }
        }

        public static readonly DependencyProperty PointerOverFillProperty =
            DependencyProperty.Register("PointerOverFill", typeof(SolidColorBrush), typeof(Rating), null);
        #endregion PointerOverFill

        #region ReadonlyFill

        public static readonly DependencyProperty ReadOnlyFillProperty =
            DependencyProperty.Register("ReadOnlyFill", typeof(SolidColorBrush), typeof(Rating), null);

        public SolidColorBrush ReadOnlyFill
        {
            get { return (SolidColorBrush)GetValue(ReadOnlyFillProperty); }
            set { SetValue(ReadOnlyFillProperty, value); }
        }
        #endregion ReadonlyFill
        
        /// <summary>
        /// Initializes a new instance of the Rating control.
        /// </summary>
        public Rating()
        {
            this.DefaultStyleKey = typeof(Rating);

            this.Interaction = new InteractionHelper(this);

            this.ItemsControlHelper = new ItemsControlHelper(this);

            this.LayoutUpdated += ((snd, arg) =>
                {
                    UpdateValues();
                    UpdateDisplayValues();
                });

        }

        /// <summary>
        /// Applies control template to the items control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            ItemsControlHelper.OnApplyTemplate();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// This method is invoked when the pointer enters the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (Interaction.AllowPointerEnter(e))
            {
                Interaction.UpdateVisualStateBase(true);
            }
            base.OnPointerEntered(e);
        }

        /// <summary>
        /// This method is invoked when the pointer leaves the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (Interaction.AllowPointerExit(e))
            {
                Interaction.UpdateVisualStateBase(true);
            }
            base.OnPointerExited(e);
        }

        /// <summary>
        /// Provides handling for the Rating's PointerPressed event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (Interaction.AllowPointerPressed(e))
            {
                Interaction.OnPointerPressedBase();
            }
            base.OnPointerPressed(e);
        }

        /// <summary>
        /// Provides handling for the Rating's PointerReleased event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (Interaction.AllowPointerPressed(e))
            {
                Interaction.OnPointerReleasedBase();
            }
            base.OnPointerReleased(e);
        }

        /// <summary>
        /// Updates the values of the rating items.
        /// </summary>
        private void UpdateValues()
        {
            IList<RatingItem> ratingItems = GetRatingItems().ToList();

            RatingItem oldSelectedItem = this.GetSelectedRatingItem();

            IEnumerable<Tuple<RatingItem, double>> itemAndWeights =
                EnumerableFunctions
                    .Zip(
                        ratingItems,
                        ratingItems
                            .Select(ratingItem => 1.0)
                            .GetWeightedValues(WeightedValue),
                        (item, percent) => Tuple.Create(item, percent));

            foreach (Tuple<RatingItem, double> itemAndWeight in itemAndWeights)
            {
                itemAndWeight.Item1.Value =  itemAndWeight.Item2;
            }

            RatingItem newSelectedItem = this.GetSelectedRatingItem();

            if (HoveredRatingItem == null)
            {
                DisplayValue = WeightedValue;
            }
        }

        /// <summary>
        /// Updates the value and actual value of the rating items.
        /// </summary>
        private void UpdateDisplayValues()
        {
            IList<RatingItem> ratingItems = GetRatingItems().ToList();

            IEnumerable<Tuple<RatingItem, double>> itemAndWeights =
                EnumerableFunctions
                    .Zip(
                        ratingItems,
                        ratingItems
                            .Select(ratingItem => 1.0)
                            .GetWeightedValues(WeightedValue),
                        (item, percent) => Tuple.Create(item, percent));

            RatingItem selectedItem = null;
            Tuple<RatingItem, double> selectedItemAndWeight = itemAndWeights.LastOrDefault(i => i.Item2 > 0.0);
            if (selectedItemAndWeight != null)
            {
                selectedItem = selectedItemAndWeight.Item1;
            }
            else
            {
                selectedItem = GetSelectedRatingItem();
            }

            foreach (Tuple<RatingItem, double> itemAndWeight in itemAndWeights)
            {
                if (SelectionMode == RatingSelectionMode.Individual && itemAndWeight.Item1 != selectedItem)
                {
                    itemAndWeight.Item1.DisplayValue = 0.0;
                }
                else
                {
                    itemAndWeight.Item1.DisplayValue = itemAndWeight.Item2;
                }
            }
        }

        /// <summary>
        /// Updates the hover states of the rating items.
        /// </summary>
        private void UpdateHoverStates()
        {
            if (HoveredRatingItem != null && IsEnabled)
            {
                IList<RatingItem> ratingItems = GetRatingItems().ToList();
                int indexOfItem = ratingItems.IndexOf(HoveredRatingItem);

                double total = ratingItems.Count();
                double filled = indexOfItem + 1;

                this.DisplayValue = filled / total;

                for (int cnt = 0; cnt < ratingItems.Count; cnt++)
                {
                    RatingItem ratingItem = ratingItems[cnt];
                    if (cnt <= indexOfItem && this.SelectionMode == RatingSelectionMode.Continuous)
                    {
                        VisualStates.GoToState(ratingItem, true, VisualStates.StatePointerOver);
                    }
                    else
                    {
                        IUpdateVisualState updateVisualState = (IUpdateVisualState)ratingItem;
                        updateVisualState.UpdateVisualState(true);
                    }
                }
            }
            else
            {
                this.DisplayValue = this.Value;

                foreach (IUpdateVisualState updateVisualState in GetRatingItems().OfType<IUpdateVisualState>())
                {
                    updateVisualState.UpdateVisualState(true);
                }
            }
        }

        /// <summary>
        /// This method returns a container for the item.
        /// </summary>
        /// <returns>A container for the item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RatingItem();
        }

        /// <summary>
        /// Gets a value indicating whether the item is its own container.
        /// </summary>
        /// <param name="item">The item which may be a container.</param>
        /// <returns>A value indicating whether the item is its own container.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RatingItem;
        }

        /// <summary>
        /// This method prepares a container to host an item.
        /// </summary>
        /// <param name="element">The container.</param>
        /// <param name="item">The item hosted in the container.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            RatingItem ratingItem = (RatingItem)element;

            var index = ItemContainerGenerator.IndexFromContainer(element);

            if (index > -1)
            {
                ToolTipService.SetToolTip(ratingItem, ((index+1).ToString()));
            }

            ratingItem.SetBinding(Control.ForegroundProperty, new Binding() {Path = new PropertyPath("Foreground"), Source = this});
            ratingItem.SetBinding(RatingItem.PointerOverFillProperty, new Binding() { Path = new PropertyPath("PointerOverFill"), Source = this });
            ratingItem.SetBinding(RatingItem.PointerPressedFillProperty, new Binding() { Path = new PropertyPath("PointerPressedFill"), Source = this });
            ratingItem.SetBinding(RatingItem.FontSizeProperty, new Binding() { Path = new PropertyPath("FontSize"), Source = this });
            ratingItem.SetBinding(RatingItem.TagProperty, new Binding() { Path = new PropertyPath("Tag"), Source = this });
            ratingItem.SetBinding(RatingItem.BackgroundProperty, new Binding() { Path = new PropertyPath("Background"), Source = this });
            ratingItem.SetBinding(RatingItem.ReadOnlyFillProperty, new Binding() { Path = new PropertyPath("ReadOnlyFill"), Source = this });

            ratingItem.IsEnabled = this.IsEnabled;
            if (ratingItem.Style == null)
            {
                ratingItem.Style = this.ItemContainerStyle;
            }
            ratingItem.Click += RatingItemTapped;
            ratingItem.PointerEntered += RatingItemPointerEnter;
            ratingItem.PointerExited += RatingItemPointerExited;

            ratingItem.ParentRating = this;
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// This method clears a container used to host an item.
        /// </summary>
        /// <param name="element">The container that hosts the item.</param>
        /// <param name="item">The item hosted in the container.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            RatingItem ratingItem = (RatingItem)element;
            ratingItem.Click -= RatingItemTapped;
            ratingItem.PointerEntered -= RatingItemPointerEnter;
            ratingItem.PointerExited -= RatingItemPointerExited;
            ratingItem.ParentRating = null;

            if (ratingItem == HoveredRatingItem)
            {
                HoveredRatingItem = null;
                UpdateDisplayValues();
                UpdateHoverStates();
            }

            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// This method is invoked when a rating item's mouse enter event is
        /// invoked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RatingItemPointerEnter(object sender, PointerRoutedEventArgs e)
        {
            HoveredRatingItem = (RatingItem)sender;
            UpdateHoverStates();
        }

        /// <summary>
        /// This method is invoked when a rating item's mouse leave event is
        /// invoked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RatingItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            HoveredRatingItem = null;
            UpdateDisplayValues();
            UpdateHoverStates();
        }

        /// <summary>
        /// Returns a sequence of rating items.
        /// </summary>
        /// <returns>A sequence of rating items.</returns>
        internal IEnumerable<RatingItem> GetRatingItems()
        {
            return
                Enumerable
                    .Range(0, this.Items.Count)
                    .Select(index => (RatingItem)ItemContainerGenerator.ContainerFromIndex(index))
                    .Where(ratingItem => ratingItem != null);
        }

        /// <summary>
        /// Selects a rating item.
        /// </summary>
        /// <param name="selectedRatingItem">The selected rating item.</param>
        internal void SelectRatingItem(RatingItem selectedRatingItem)
        {
            if (this.IsEnabled)
            {
                IList<RatingItem> ratingItems = GetRatingItems().ToList();
                IEnumerable<double> weights = ratingItems.Select(ratingItem => 1.0);
                double total = ratingItems.Count();
                double percent;
                if (total != 0)
                {
                    percent = weights.Take(ratingItems.IndexOf(selectedRatingItem) + 1).Sum() / total;
                    this.Value = percent;
                }
            }
        }

        /// <summary>
        /// This method is raised when a rating item value is selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RatingItemTapped(object sender, RoutedEventArgs e)
        {
            if (this.IsEnabled)
            {
                RatingItem item = (RatingItem)sender;
                OnRatingItemValueSelected(item, 1.0);
            }
        }

        /// <summary>
        /// Returns the selected rating item.
        /// </summary>
        /// <returns>The selected rating item.</returns>
        private RatingItem GetSelectedRatingItem()
        {
            return this.GetRatingItems().LastOrDefault(ratingItem => ratingItem.Value > 0.0);
        }

        /// <summary>
        /// This method is invoked when the rating item value is changed.
        /// </summary>
        /// <param name="ratingItem">The rating item that has changed.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnRatingItemValueSelected(RatingItem ratingItem, double newValue)
        {
            List<RatingItem> ratingItems = GetRatingItems().ToList();
            double total = ratingItems.Count();
            this.Value = ratingItems.IndexOf(ratingItem) + 1;
        }

        /// <summary>
        /// Provides handling for the
        /// <see cref="E:System.Windows.UIElement.KeyDown" /> event when a key
        /// is pressed while the control has focus.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains
        /// the event data.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="e " />is null.
        /// </exception>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity metric is inflated by the switch statements")]
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (!Interaction.AllowKeyDown(e))
            {
                return;
            }

            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
            VirtualKey invariantKey = InteractionHelper.GetLogicalKey(FlowDirection, e.Key);

            switch (invariantKey)
            {
                case VirtualKey.Left:
                    {
                        RatingItem ratingItem = FocusManager.GetFocusedElement() as RatingItem;

                        if (ratingItem != null)
                        {
                            ratingItem = GetRatingItemAtOffsetFrom(ratingItem, -1);
                        }
                        else
                        {
                            ratingItem = GetRatingItems().FirstOrDefault();
                        }
                        if (ratingItem != null)
                        {
                            if (ratingItem.Focus(FocusState.Keyboard))
                            {
                                e.Handled = true;
                            }
                        }
                    }
                    break;
                case VirtualKey.Right:
                    {
                        RatingItem ratingItem = FocusManager.GetFocusedElement() as RatingItem;

                        if (ratingItem != null)
                        {
                            ratingItem = GetRatingItemAtOffsetFrom(ratingItem, 1);
                        }
                        else
                        {
                            ratingItem = GetRatingItems().FirstOrDefault();
                        }
                        if (ratingItem != null)
                        {
                            if (ratingItem.Focus(FocusState.Keyboard))
                            {
                                e.Handled = true;
                            }
                        }
                    }
                    break;
                case VirtualKey.Add:
                    {
                        if (this.IsEnabled)
                        {
                            RatingItem ratingItem = GetSelectedRatingItem();
                            if (ratingItem != null)
                            {
                                ratingItem = GetRatingItemAtOffsetFrom(ratingItem, 1);
                            }
                            else
                            {
                                ratingItem = GetRatingItems().FirstOrDefault();
                            }
                            if (ratingItem != null)
                            {
                                ratingItem.SelectValue();
                                e.Handled = true;
                            }
                        }
                    }
                    break;
                case VirtualKey.Subtract:
                    {
                        if (this.IsEnabled)
                        {
                            RatingItem ratingItem = GetSelectedRatingItem();
                            if (ratingItem != null)
                            {
                                ratingItem = GetRatingItemAtOffsetFrom(ratingItem, -1);
                            }
                            if (ratingItem != null)
                            {
                                ratingItem.SelectValue();
                                e.Handled = true;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets a rating item at a certain index offset from another 
        /// rating item.
        /// </summary>
        /// <param name="ratingItem">The rating item.</param>
        /// <param name="offset">The rating item at an offset from the 
        /// index of the rating item.</param>
        /// <returns>The rating item at the offset.</returns>
        private RatingItem GetRatingItemAtOffsetFrom(RatingItem ratingItem, int offset)
        {
            IList<RatingItem> ratingItems = GetRatingItems().ToList();
            int index = ratingItems.IndexOf(ratingItem);
            if (index == -1)
            {
                return null;
            }
            index += offset;
            if (index >= 0 && index < ratingItems.Count)
            {
                ratingItem = ratingItems[index];
            }
            else
            {
                ratingItem = null;
            }
            return ratingItem;
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to use transitions.</param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            Interaction.UpdateVisualStateBase(useTransitions);
        }
    }
}
