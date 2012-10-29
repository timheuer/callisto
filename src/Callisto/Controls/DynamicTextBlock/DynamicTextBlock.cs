//-----------------------------------------------------------------------
// <copyright file="DynamicTextBlock.cs" company="Pixel Lab">
//    Copyright (c) 2009 Pixel Lab
//
//    Permission is hereby granted, free of charge, to any person
//    obtaining a copy of this software and associated documentation
//    files (the "Software"), to deal in the Software without
//    restriction, including without limitation the rights to use,
//    copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the
//    Software is furnished to do so, subject to the following
//    conditions:
//
//    The above copyright notice and this permission notice shall be
//    included in all copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
//
//    Read more about the MIT License for software at it's wikipedia
//    page here: http://en.wikipedia.org/wiki/MIT_License
// 
//    If you happen to use this code in a project, please email me
//    robby@nerdplusart.com and let me know how and where you use 
//    it. That is, after all, the fun part of sharing code: knowing
//    that it gets used.
//   
//    Find out more about PixelLab at http://pixellab.cc or read my
//    blog at http://nerdplusart.com.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{


    /// <summary>
    /// A simple text control that truncates the text to ellipses when there
    /// is insufficient room to display all of the text.
    /// </summary>
    public class DynamicTextBlock : ContentControl
    {
        #region Text (DependencyProperty)

        /// <summary>
        /// Gets or sets the Text DependencyProperty. This is the text that will be displayed.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DynamicTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicTextBlock)d).OnTextChanged(e);
        }

        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }

        #endregion

        #region TextWrapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the TextWrapping property. This corresponds to TextBlock.TextWrapping.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(DynamicTextBlock),
            new PropertyMetadata(TextWrapping.NoWrap, new PropertyChangedCallback(OnTextWrappingChanged)));

        private static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicTextBlock)d).OnTextWrappingChanged(e);
        }

        protected virtual void OnTextWrappingChanged(DependencyPropertyChangedEventArgs e)
        {
            this.textBlock.TextWrapping = (TextWrapping)e.NewValue;
            this.InvalidateMeasure();
        }

        #endregion

        #region LineHeight (DependencyProperty)

        /// <summary>
        /// Gets or sets the LineHeight property. This property corresponds to TextBlock.LineHeight;
        /// </summary>
        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(double), typeof(DynamicTextBlock),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnLineHeightChanged)));

        private static void OnLineHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicTextBlock)d).OnLineHeightChanged(e);
        }

        protected virtual void OnLineHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlock.LineHeight = LineHeight;
            this.InvalidateMeasure();
        }

        #endregion

        #region LineStackingStrategy (DependencyProperty)

        /// <summary>
        /// Gets or sets the LineStackingStrategy DependencyProperty. This corresponds to TextBlock.LineStackingStrategy.
        /// </summary>
        public LineStackingStrategy LineStackingStrategy
        {
            get { return (LineStackingStrategy)GetValue(LineStackingStrategyProperty); }
            set { SetValue(LineStackingStrategyProperty, value); }
        }
        public static readonly DependencyProperty LineStackingStrategyProperty =
            DependencyProperty.Register("LineStackingStrategy", typeof(LineStackingStrategy), typeof(DynamicTextBlock),
            new PropertyMetadata(LineStackingStrategy.BlockLineHeight, new PropertyChangedCallback(OnLineStackingStrategyChanged)));

        private static void OnLineStackingStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicTextBlock)d).OnLineStackingStrategyChanged(e);
        }

        protected virtual void OnLineStackingStrategyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.textBlock.LineStackingStrategy = (LineStackingStrategy)e.NewValue;
            this.InvalidateMeasure();
        }

        #endregion

        /// <summary>
        /// A TextBlock that gets set as the control's content and is ultiately the control 
        /// that displays our text
        /// </summary>
        private TextBlock textBlock;

        /// <summary>
        /// Initializes a new instance of the DynamicTextBlock class
        /// </summary>
        public DynamicTextBlock()
        {
            // create our textBlock and initialize
            this.textBlock = new TextBlock();
            this.Content = this.textBlock;
        }

        /// <summary>
        /// Handles the measure part of the measure and arrange layout process. During this process
        /// we measure the textBlock that we've created as content with increasingly smaller amounts
        /// of text until we find text that fits.
        /// </summary>
        /// <param name="availableSize">the available size</param>
        /// <returns>the base implementation of Measure</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // just to make the code easier to read
            bool wrapping = this.TextWrapping == TextWrapping.Wrap;

            Size unboundSize = wrapping ? new Size(availableSize.Width, double.PositiveInfinity) : new Size(double.PositiveInfinity, availableSize.Height);
            string reducedText = this.Text;

            // set the text and measure it to see if it fits without alteration
            if (string.IsNullOrEmpty(reducedText)) reducedText = string.Empty;
            this.textBlock.Text = reducedText;
            Size textSize = base.MeasureOverride(unboundSize);

            while (wrapping ? textSize.Height > availableSize.Height : textSize.Width > availableSize.Width)
            {
                int prevLength = reducedText.Length;
                
                if (reducedText.Length > 0)
                {
                    reducedText = this.ReduceText(reducedText);    
                }
                
                if (reducedText.Length == prevLength)
                {
                    break;
                }

                this.textBlock.Text = reducedText + "...";
                textSize = base.MeasureOverride(unboundSize);
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Reduces the length of the text. Derived classes can override this to use different techniques 
        /// for reducing the text length.
        /// </summary>
        /// <param name="text">the original text</param>
        /// <returns>the reduced length text</returns>
        protected virtual string ReduceText(string text)
        {
            return text.Substring(0, text.Length - 1);
        }
    }
}