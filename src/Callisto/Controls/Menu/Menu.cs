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
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace Callisto.Controls
{
	/// <summary>
	/// OBSOLETE. Menu is a special type of Flyout which serves the purpose for Menu commands.
	/// Most of the time this will be from an AppBar or a header area. The intent is text 
	/// commands only and not meant always to be styled.
	/// </summary>
	/// <remarks>
	/// This control is deprecated in favor of using the <see cref="Windows.UI.Xaml.Controls.MenuFlyout"/> controls in Windows 8.1.
	/// </remarks>
	[Obsolete("Windows 8.1 now provides this functionality in the XAML framework itself as MenuFlyout.")]
    [ContentProperty(Name="Items")]
    public sealed class Menu : Control
    {
        private ObservableCollection<MenuItemBase> _items;
        private ItemsControl _itemContainerList;

		/// <summary>
		/// Gets the menu items.
		/// </summary>
		public ObservableCollection<MenuItemBase> Items
        {
            get { return _items; }
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            switch (args.Key)
            {
                case Windows.System.VirtualKey.Escape:
                    if (this.Parent.GetType() == typeof(Flyout)) { ((Flyout)this.Parent).IsOpen = false; }
                    break;
                case Windows.System.VirtualKey.Up:
                    ChangeFocusedItem(false);
                    break;
                case Windows.System.VirtualKey.Down:
                    ChangeFocusedItem(true);
                    break;
                case Windows.System.VirtualKey.Home:
                case Windows.System.VirtualKey.PageUp:
                    PageFocusedItem(true);
                    break;
                case Windows.System.VirtualKey.End:
                case Windows.System.VirtualKey.PageDown:
                    PageFocusedItem(false);
                    break;
                case Windows.System.VirtualKey.Enter:
                case Windows.System.VirtualKey.Space:
                    OnTapped(new TappedRoutedEventArgs());
                    break;
                default:
                    break;
            }
        }

        private void PageFocusedItem(bool top)
        {
            int itemNumber = top ? 0 : _items.Count-1;

            _items[itemNumber].Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void ChangeFocusedItem(bool ascendIndex)
        {
            var focusedElement = FocusManager.GetFocusedElement();
            int startIndex;

            if (focusedElement != null && ((focusedElement == _itemContainerList || focusedElement == this) && _items.Count > 0))
            {
                // focused item is the menu or the item container list, so try to set focus to an initial item
                startIndex = GetNextItemIndex(-1, ascendIndex);
            }
            else if (focusedElement != null && (focusedElement is MenuItem && _items.Contains((MenuItem)focusedElement)))
            {
                // focused item is already one of our menu items, so try to set focus to next
                startIndex = GetNextItemIndex(_items.IndexOf((MenuItem)focusedElement), ascendIndex);
            }
            else
            {
                // focus is outside of the menu
                return;
            }

            int index = startIndex;

            MenuItemBase nextItem = _items[index];

            // focus next item, if it's a menu item
            if (nextItem is MenuItem)
            {
                nextItem.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
            else
            {
                // next element wasn't a MenuItem, so loop through once trying to find the next item
                index = GetNextItemIndex(index, ascendIndex);
                nextItem = _items[index];
                while (nextItem != null && (!(nextItem is MenuItem) && index != startIndex))
                {
                    index = GetNextItemIndex(index, ascendIndex);
                    nextItem = _items[index];
                }
                if (nextItem != null && nextItem is MenuItem)
                {
                    nextItem.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            }
        }

        private int GetNextItemIndex(int startIndex, bool ascending)
        {
            int index = startIndex + (ascending ? 1 : -1);

            if (index >= _items.Count)
            {
                // if after end of collection, go to start
                index = 0;
            }
            else if (index < 0)
            {
                // if before start of collection, go to end
                index = _items.Count - 1;
            }

            return index;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Menu"/> class.
		/// </summary>
        public Menu()
        {
            this.DefaultStyleKey = typeof(Menu);

            // handle other key events
            this.AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), true);

            this.Loaded += OnLoaded;

            _items = new ObservableCollection<MenuItemBase>();
            _items.CollectionChanged += OnItemsCollectionChanged;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((Menu)sender).IsHitTestVisible = true;
        }

		/// <summary>
		/// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
		/// call ApplyTemplate. In simplest terms, this means the method is called just before a UI 
		/// element displays in your app. Override this method to influence the default post-template
		/// logic of a class.
		/// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemContainerList = GetTemplateChild("ItemList") as ItemsControl;
            _itemContainerList.ItemsSource = _items;   
        }

        void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // since we can't make a custom routed event, manually attach and detach event handlers for selection event
            if (e.NewItems != null)
            {
                foreach (object o in e.NewItems)
                {
                    MenuItem newItem = o as MenuItem;
                    if (newItem != null)
                        newItem.Tapped += OnMenuItemSelected;
                }
            }

            if (e.OldItems != null)
            {
                foreach (object o in e.OldItems)
                {
                    MenuItem oldItem = o as MenuItem;
                    if (oldItem != null)
                        oldItem.Tapped -= OnMenuItemSelected;
                }
            }
        }

        void OnMenuItemSelected(object sender, TappedRoutedEventArgs args)
        {
            MenuItem item = sender as MenuItem;

            if (item == null)
                return;

            // NOTE: Menu is intended to be used with a Parent Flyout.
            // this below is added as a precaution to prevent some issues but Menu without 
            // Flyout isn't fully supported
            if (this.Parent is Flyout)
            {
                ((Flyout)this.Parent).IsOpen = false;
            }

            if (item.Command != null)
                item.Command.Execute(item.CommandParameter);
        }
    }
}
