﻿//
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
	/// <summary>
	/// An items control that presents enumerable content similar to the live tiles on the
	/// start menu.
	/// </summary>
	[TemplatePart(Name = SCROLLER_PARTNAME, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = CURRENT_PARTNAME, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = NEXT_PARTNAME, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = TRANSLATE_PARTNAME, Type = typeof(TranslateTransform))]
	[TemplatePart(Name = STACK_PARTNAME, Type = typeof(StackPanel))]
	public sealed class LiveTile : Control
	{
		#region Member Variables
		// randomizer for randomizing when a tile swaps content
		private static Random _randomizer = new Random();
		// current index in the items displayed
		private int _currentIndex = 0;
		// timer for triggering when to flip the content
		private DispatcherTimer _timer;
		// FrameworkElement holding a reference to the current element being display
		private FrameworkElement _currentElement;
		// FrameworkElement holding a reference to the next element being display
		private FrameworkElement _nextElement;
		// Container Element that's being translated to animate from one item to the next
		private FrameworkElement _scroller;
		// Translate Transform used when animating the transition
		private TranslateTransform _translate;
		// StackPanel that contains the live tile elements
		private StackPanel _stackPanel;
		#endregion

		#region Constants

		const string SCROLLER_PARTNAME = "Scroller";
		const string CURRENT_PARTNAME = "Current";
		const string NEXT_PARTNAME = "Next";
		const string TRANSLATE_PARTNAME = "Translate";
		const string STACK_PARTNAME = "Stack";

		#endregion

		/// <summary>
		/// Creates a new instance of <see cref="LiveTile"/>
		/// </summary>
		public LiveTile()
		{
			this.DefaultStyleKey = typeof(LiveTile);

			this.Unloaded += EpisodeFlipControl_Unloaded;
			this.Loaded += EpisodeFlipControl_Loaded;
			this.SizeChanged += EpisodeFlipControl_SizeChanged;
		}

		#region Methods and Events
		
		/// <summary>
		/// Invoked whenever application code or internal processes (such as a rebuilding
		/// layout pass) call <see cref="ApplyTemplate"/>. In simplest terms, this means the method
		/// is called just before a UI element displays in an application. Override this
		/// method to influence the default post-template logic of a class.
		/// </summary>
		protected override void OnApplyTemplate()
		{
			_scroller = GetTemplateChild(SCROLLER_PARTNAME) as FrameworkElement;
			_currentElement = GetTemplateChild(CURRENT_PARTNAME) as FrameworkElement;
			_nextElement = GetTemplateChild(NEXT_PARTNAME) as FrameworkElement;
			_translate = GetTemplateChild(TRANSLATE_PARTNAME) as TranslateTransform;
			_stackPanel = GetTemplateChild(STACK_PARTNAME) as StackPanel;
			if (_stackPanel != null)
			{
				if (Direction == SlideDirection.Up)
					_stackPanel.Orientation = Orientation.Vertical;
				else
					_stackPanel.Orientation = Orientation.Horizontal;
			}
			if(ItemsSource != null)
				Start();
			base.OnApplyTemplate();
		}

		private void EpisodeFlipControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (_currentElement != null && _nextElement != null)
			{
				_currentElement.Width = _nextElement.Width = e.NewSize.Width;
				_currentElement.Height = _nextElement.Height = e.NewSize.Height;
			}
			//Set content area to twice the size in the slide direction
			if (_scroller != null)
			{
				if(Direction == SlideDirection.Up)
					_scroller.Height = e.NewSize.Height * 2;
				else
					_scroller.Width = e.NewSize.Width * 2;
			}
			//Set clip to control
			this.Clip = new Windows.UI.Xaml.Media.RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };
		}

		private void EpisodeFlipControl_Loaded(object sender, RoutedEventArgs e)
		{
			//Start timer after control has loaded
			if (_timer != null)
				_timer.Start();
		}

		private void EpisodeFlipControl_Unloaded(object sender, RoutedEventArgs e)
		{
			//Stop timer and reset animation when control unloads
			if (_timer != null)
				_timer.Stop();
			if(_translate != null)
				_translate.Y = 0;
		}

		/// <summary>
		/// Triggered when it's time to flip to the next live tile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_Tick(object sender, object e)
		{
			_currentIndex++;
			_timer.Interval = TimeSpan.FromSeconds(_randomizer.Next(5) + 5); //randomize next flip
			UpdateNextItem();
		}

		private void UpdateNextItem()
		{
			//Check if there's more than one item. if not, don't start animation
			bool hasTwoOrMoreItems = false;
			if (ItemsSource is IEnumerable)
			{
				var enumerator = (ItemsSource as IEnumerable).GetEnumerator();
				int count = 0;
				while(enumerator.MoveNext())
				{
					count++;
					if (count > 1)
					{
						hasTwoOrMoreItems = true;
						break;
					}
				}
			}
			if (!hasTwoOrMoreItems)
				return;
			var sb = new Windows.UI.Xaml.Media.Animation.Storyboard();
			if (_translate != null)
			{
				var anim = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
				anim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
				anim.From = 0;
				if(Direction == LiveTile.SlideDirection.Up)
					anim.To = -this.ActualHeight;
				else if (Direction == LiveTile.SlideDirection.Left)
					anim.To = -this.ActualWidth;

				anim.FillBehavior = Windows.UI.Xaml.Media.Animation.FillBehavior.HoldEnd;
				anim.EasingFunction = new Windows.UI.Xaml.Media.Animation.CubicEase() { EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut };
				Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(anim, _translate);
				if(Direction == LiveTile.SlideDirection.Up
					// || this.SlideDirection == SlideView.SlideDirection.Down
					)
					Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(anim, "Y");
				else
					Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(anim, "X");
				sb.Children.Add(anim);
			}
			sb.Completed += (a, b) =>
			{
				//Reset back and swap images, getting the next image ready
				sb.Stop();
				if (_translate != null)
				{
					_translate.X = _translate.Y = 0;
				}
				if(_currentElement != null)
					_currentElement.DataContext = GetCurrent();
				if(_nextElement != null)
					_nextElement.DataContext = GetNext();
			};
			sb.Begin();
		}

		private object GetCurrent()
		{
			return GetItemAt(_currentIndex);
		}

		private object GetNext()
		{
			return GetItemAt(_currentIndex + 1);
		}

		private object GetItemAt(int index)
		{
			if (ItemsSource != null)
			{
				if (ItemsSource is IList)
				{
					var list = ItemsSource as IList;
					if (list.Count > 0)
					{
						index = index % list.Count;
						return list[index];
					}
				}
				else if (ItemsSource is IEnumerable<object>)
				{
					var items = (ItemsSource as IEnumerable<object>);
					int count = items.Count();
					if (count > 0)
					{
						index = index % count;
						return items.ElementAt(index);
					}
				}
			}
			return null;
		}

		private void Start()
		{
			_currentIndex = 0;
			if (_currentElement != null)
				_currentElement.DataContext = GetCurrent();
			if (_nextElement != null)
				_nextElement.DataContext = GetNext();
			if (_timer == null)
			{
				_timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
				_timer.Tick += timer_Tick;
				_timer.Interval = TimeSpan.FromSeconds(_randomizer.Next(5) + 5);
			}
			_timer.Start();
		}

		#endregion

		#region Dependency Properties

		/// <summary>
		/// Gets or sets the ItemsSource
		/// </summary>
		public object ItemsSource
		{
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ItemsSource"/> property.
		/// </summary>
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(object), typeof(LiveTile), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

		private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (d as LiveTile);
			if (e.NewValue is IEnumerable)
			{
				if (ctrl._currentElement != null && ctrl._nextElement != null)
					ctrl.Start();
			}
			else if (ctrl._timer != null)
				ctrl._timer.Stop();
		}

		/// <summary>
		/// Gets or sets the item template
		/// </summary>
		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ItemTemplate"/> property.
		/// </summary>
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LiveTile), null);

		/// <summary>
		/// Gets or sets the direction the tile slides in.
		/// </summary>
		public SlideDirection Direction
		{
			get { return (SlideDirection)GetValue(SlideDirectionProperty); }
			set { SetValue(SlideDirectionProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="SlideDirection"/> property.
		/// </summary>
		public static readonly DependencyProperty SlideDirectionProperty =
			DependencyProperty.Register("SlideDirection", typeof(SlideDirection), typeof(LiveTile), new PropertyMetadata(SlideDirection.Up));

		#endregion

		/// <summary>
		/// Live Tile Slide Direction
		/// </summary>
		public enum SlideDirection
		{
			/// <summary>Up</summary>
			Up,
			//Down,
			/// <summary>Left</summary>
			Left,
			//Right
		}
	}
}