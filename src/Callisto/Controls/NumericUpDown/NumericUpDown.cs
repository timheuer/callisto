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
using System.Diagnostics;
using System.Globalization;
using Callisto.Controls.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Callisto.Controls
{
    public sealed class NumericUpDown : TextBox
    {
        private RepeatButton _incrementButton;
        private RepeatButton _decrementButton;
        private string _text;
        private bool _canIncrement;
        private bool _canDecrement;

        private int _delay = 500;

        public int Delay
        {
            get { return _delay; }
            set 
            { 
                _delay = value;
                _incrementButton.Delay = _delay;
                _decrementButton.Delay = _delay;
            }
        }

        private int _interval = 100;

        public int Interval
        {
            get { return _interval; }
            set 
            { 
                _interval = value;
                _incrementButton.Interval = _interval;
                _decrementButton.Interval = _interval;
            }
        }

        public NumericUpDown()
        {
            this.DefaultStyleKey = typeof(NumericUpDown);
            LostFocus += OnTextLostFocus;
            GotFocus += OnTextGotFocus;
            TextChanged += OnTextChanged;
        }

        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            switch (e.Key)
            {
                case Windows.System.VirtualKey.Up:
                    DoIncrement();
                    e.Handled = true;
                    break;
                case Windows.System.VirtualKey.Down:
                    DoDecrement();
                    e.Handled = true;
                    break;
            }
        }

        private void OnTextGotFocus(object sender, RoutedEventArgs e)
        {
            if (Text != null)
            {
                if (SelectionLength == 0 && Text != null)
                {
                    Select(0, Text.Length);
                }
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ProcessUserInput();
        }

        private void OnTextLostFocus(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void ProcessUserInput()
        {
            if (Text != null)
            {
                if (string.Compare(_text, Text, StringComparison.CurrentCulture) != 0)
                {
                    // retain caretposition
                    int caretPosition = SelectionStart;

                    _text = Text;
                    ApplyValue(_text);

                    if (caretPosition < Text.Length)
                    {
                        // set back caretposition.
                        SelectionStart = caretPosition;
                    }
                }
            }
        }

        private void ApplyValue(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    double regularParsedValue = double.Parse(text, CultureInfo.CurrentCulture);
                    if (regularParsedValue >= Minimum && regularParsedValue <= Maximum)
                    {
                        Value = regularParsedValue;
                    }
                    SetTextBoxText();
                }
                catch
                {
                    SetTextBoxText();
                }
            }
            else
            {
                Value = Minimum;
                Text = string.Empty;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _incrementButton = GetTemplateChild("PART_IncrementButton") as RepeatButton;
            _decrementButton = GetTemplateChild("PART_DecrementButton") as RepeatButton;

            if (_incrementButton != null)
            {
                _incrementButton.SizeChanged += ResizePartButton;
                _incrementButton.Delay = _delay;
                _incrementButton.Interval = _interval;
                _incrementButton.Click += OnIncrementClicked;
            }
            if (_decrementButton != null)
            {
                _decrementButton.SizeChanged += ResizePartButton;
                _decrementButton.Delay = _delay;
                _decrementButton.Interval = _interval;
                _decrementButton.Click += OnDecrementClicked;
            }

            SetValidIncrementDirection();
        }

        private void OnDecrementClicked(object sender, RoutedEventArgs e)
        {
            DoDecrement();
        }

        private void DoDecrement()
        {
            if (_canDecrement)
            {
                Value = (double) ((decimal) Value - (decimal) Increment);
                _requestedVal = Value;
            }
        }

        private void OnIncrementClicked(object sender, RoutedEventArgs e)
        {
            DoIncrement();
        }

        private void DoIncrement()
        {
            if (_canIncrement)
            {
                Value = (double) ((decimal) Value + (decimal) Increment);
                _requestedVal = Value;
            }
        }

        private static void ResizePartButton(object s, SizeChangedEventArgs e)
        {
            RepeatButton b = s as RepeatButton;
            if (b != null)
            {
                double currentWidth = b.Width;

                if (currentWidth != e.NewSize.Height)
                {
                    b.Width = e.NewSize.Height;
                }
            }
        }

        private void SetValidIncrementDirection()
        {
            if (Value < Maximum)
            {
                VisualStateManager.GoToState(this, "IncrementEnabled", true);
                _canIncrement = true;
            }
            if (Value > Minimum)
            {
                VisualStateManager.GoToState(this, "DecrementEnabled", true);
                _canDecrement = true;
            }
            if (Math.Round(Value, DecimalPlaces) == Math.Round(Maximum, DecimalPlaces))
            {
                VisualStateManager.GoToState(this, "IncrementDisabled", true);
                _canIncrement = false;
            }
            if (Math.Round(Value, DecimalPlaces) == Math.Round(Minimum, DecimalPlaces))
            {
                VisualStateManager.GoToState(this, "DecrementDisabled", true);
                _canDecrement = false;
            }
        }

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericUpDown),
                                        new PropertyMetadata(0.0, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)d;
            EnsureValidDoubleValue(d, e);

            if (nud._levelsFromRootCall == 0)
            {
                nud._requestedMin = (double)e.NewValue;
                nud._initialMin = (double)e.OldValue;
                nud._initialMax = nud.Maximum;
                nud._initialVal = nud.Value;

                nud._levelsFromRootCall++;
                if (nud.Minimum != nud._requestedMin)
                {
                    nud.Minimum = nud._requestedMin;
                }
                nud._levelsFromRootCall--;
            }
            nud._levelsFromRootCall++;

            nud.CoerceMaximum();
            nud.CoerceValue();

            nud._levelsFromRootCall--;
            if (nud._levelsFromRootCall == 0)
            {
                nud.SetValidIncrementDirection();
            }
        }

        #endregion

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericUpDown), new PropertyMetadata(100.0, OnMaximumPropertyChanged));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnsureValidDoubleValue(d, e);
            NumericUpDown nud = d as NumericUpDown;

            if (nud._levelsFromRootCall == 0)
            {
                nud._requestedMax = (double)e.NewValue;
                nud._initialMax = (double)e.OldValue;
                nud._initialVal = nud.Value;
            }
            nud._levelsFromRootCall++;

            nud.CoerceMaximum();
            nud.CoerceValue();

            nud._levelsFromRootCall--;
            if (nud._levelsFromRootCall == 0)
            {
                nud.SetValidIncrementDirection();
            }
        } 
        #endregion

        #region Increment
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(NumericUpDown), new PropertyMetadata(1.0, OnIncrementPropertyChanged));

        private static void OnIncrementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)d;
            EnsureValidIncrementValue(d, e);

            if (nud._levelsFromRootCall == 0)
            {
                nud._requestedInc = (double)e.NewValue;
                nud._initialInc = (double)e.OldValue;

                nud._levelsFromRootCall++;
                if (nud.Increment != nud._requestedInc)
                {
                    nud.Increment = nud._requestedInc;
                }
                nud._levelsFromRootCall--;
            }

            if (nud._levelsFromRootCall == 0)
            {
                double increment = nud.Increment;
            }
        }
        #endregion

        #region DecimalPlaces
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof (int), typeof (NumericUpDown), new PropertyMetadata(0, OnDecimalPlacesPropertyChanged));

        private static void OnDecimalPlacesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnsureValidDecimalPlacesValue(d, e);

            NumericUpDown nud = d as NumericUpDown;
            nud.OnDecimalPlacesChanged((int)e.OldValue, (int)e.NewValue);
        }

        private void OnDecimalPlacesChanged(int oldValue, int newValue)
        {
            _formatString = string.Format(NumberFormatInfo.InvariantInfo, "F{0:D}", newValue);
            _levelsFromRootCall++;
            SetTextBoxText();
            _levelsFromRootCall--;
        }

        private string FormatValue()
        {
            return Value.ToString(_formatString, CultureInfo.CurrentCulture);
        }

        private string _formatString = "F0";
        #endregion

        internal void SetTextBoxText()
        {
            if (Text != null)
            {
                _text = FormatValue() ?? string.Empty;
                Text = _text;

                SelectionStart = _text.Length;
            }
        }


        #region Value
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0.0, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)d;
            nud.OnValueChanged((double)e.NewValue);
            nud.SetValidIncrementDirection();
        }

        private void OnValueChanged(double newValue)
        {
            SetTextBoxText();
        }

        #endregion

        #region Property Coersion and validation
        /// <summary>
        /// Levels from root call.
        /// </summary>
        private int _levelsFromRootCall;

        /// <summary>
        /// Initial Increment value.
        /// </summary>
        private double _initialInc = 1;

        /// <summary>
        /// Initial Minimum value.
        /// </summary>
        private double _initialMin;

        /// <summary>
        /// Initial Maximum value.
        /// </summary>
        private double _initialMax = 100;

        /// <summary>
        /// Initial Minimum value.
        /// </summary>
        private double _initialVal;

        /// <summary>
        /// Requested Increment value.
        /// </summary>
        private double _requestedInc = 1;

        /// <summary>
        /// Requested Minimum value.
        /// </summary>
        private double _requestedMin;

        /// <summary>
        /// Requested Maximum value.
        /// </summary>
        private double _requestedMax = 100;

        /// <summary>
        /// Requested Value value.
        /// </summary>
        private double _requestedVal;

        /// <summary>
        /// Ensure the Maximum is greater than or equal to the Minimum.
        /// </summary>
        private void CoerceMaximum()
        {
            double minimum = Minimum;
            double maximum = Maximum;

            if (_requestedMax != maximum)
            {
                if (_requestedMax >= minimum)
                {
                    SetValue(MaximumProperty, _requestedMax);
                }
                else if (maximum != minimum)
                {
                    SetValue(MaximumProperty, minimum);
                }
            }
            else if (maximum < minimum)
            {
                SetValue(MaximumProperty, minimum);
            }
        }

        /// <summary>
        /// Ensure the value falls between the Minimum and Maximum values.
        /// This function assumes that (Maximum >= Minimum).
        /// </summary>
        private void CoerceValue()
        {
            double minimum = Minimum;
            double maximum = Maximum;
            Debug.Assert(maximum >= minimum, "Maximum value should have been coerced already!");
            double value = Value;

            if (_requestedVal != value)
            {
                if (_requestedVal >= minimum && _requestedVal <= maximum)
                {
                    SetValue(ValueProperty, _requestedVal);
                }
                else if (_requestedVal < minimum && value != minimum)
                {
                    SetValue(ValueProperty, minimum);
                }
                else if (_requestedVal > maximum && value != maximum)
                {
                    SetValue(ValueProperty, maximum);
                }
            }
            else if (value < minimum)
            {
                SetValue(ValueProperty, minimum);
            }
            else if (value > maximum)
            {
                SetValue(ValueProperty, maximum);
            }
        }

        /// <summary>
        /// Check if an object value is a valid double value.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="number">The double value to be returned.</param>
        /// <returns>true if a valid double; false otherwise.</returns>
        private static bool IsValidDoubleValue(object value, out double number)
        {
            number = (double)value;
            return !double.IsNaN(number) && !double.IsInfinity(number)
                && number <= (double)decimal.MaxValue && number >= (double)decimal.MinValue;
        }

        /// <summary>
        /// Ensure the new value of a dependency property change is a valid double value, 
        /// or revert the change and throw an exception.
        /// </summary>
        /// <remarks>
        /// EnsureValidDoubleValue(DependencyObject d, DependencyPropertyChangedEventArgs e) is simply a wrapper for 
        /// EnsureValidDoubleValue(DependencyObject d, DependencyProperty property, object oldValue, object newValue).
        /// </remarks>
        /// <param name="d">The DependencyObject whose DependencyProperty is changed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void EnsureValidDoubleValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnsureValidDoubleValue(d, e.Property, e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Ensure the new value of a dependency property change is a valid double value, 
        /// or revert the change and throw an exception.
        /// </summary>
        /// <param name="d">The DependencyObject whose DependencyProperty is changed.</param>
        /// <param name="property">The DependencyProperty that changed.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void EnsureValidDoubleValue(DependencyObject d, DependencyProperty property, object oldValue, object newValue)
        {
            NumericUpDown nud = d as NumericUpDown;
            double number;
            if (!IsValidDoubleValue(newValue, out number))
            {
                // revert back to old value
                nud._levelsFromRootCall++;
                nud.SetValue(property, oldValue);
                nud._levelsFromRootCall--;

                // throw ArgumentException
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Not a valid Double value",
                    newValue);
                throw new ArgumentException(message, "newValue");
            }
        }

        /// <summary>
        /// Ensure the new value of Increment dependency property change is valid, 
        /// or revert the change and throw an exception.
        /// </summary>
        /// <param name="d">The DependencyObject whose DependencyProperty is changed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void EnsureValidIncrementValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)d;
            double number;
            if (!IsValidDoubleValue(e.NewValue, out number) || number <= 0)
            {
                nud._levelsFromRootCall++;
                nud.SetValue(e.Property, e.OldValue);
                nud._levelsFromRootCall--;

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Not a valie Double value",
                    e.NewValue);
                throw new ArgumentException(message, "e");
            }
        }

        /// <summary>
        /// Ensure the new value of DecimalPlaces dependency property change is valid, 
        /// or revert the change and throw an exception.
        /// </summary>
        /// <param name="d">The DependencyObject whose DecimalPlaces DependencyProperty is changed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void EnsureValidDecimalPlacesValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = d as NumericUpDown;
            int decimalPlaces = (int)e.NewValue;
            if (decimalPlaces < 0 || decimalPlaces > 15)
            {
                nud._levelsFromRootCall++;
                nud.DecimalPlaces = (int)e.OldValue;
                nud._levelsFromRootCall--;

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Not a valid Double value",
                    e.NewValue);
                throw new ArgumentException(message, "e");
            }
        }
        #endregion
    }
}
