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

using Windows.UI.Xaml.Controls;

namespace Callisto.Controls
{
    public sealed class DropDownButton : Button
    {
        private TextBlock _arrowGlyph;
        private const string PART_ARROW_GLYPH = "PART_ArrowGlyph";

        // the desired type ramp for the arrow glyph
        private const string ARROW_GLYPH_XSMALL = "\uE015"; // <= 9pt
        private const string ARROW_GLYPH_SMALL  = "\uE09D"; // >= 10pt; < 16pt
        private const string ARROW_GLYPH_MEDIUM = "\uE099"; // >= 16pt; < 33pt
        private const string ARROW_GLYPH_LARGE  = "\uE228"; // > 33pt;

        public DropDownButton()
        {
            DefaultStyleKey = typeof(DropDownButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _arrowGlyph = GetTemplateChild(PART_ARROW_GLYPH) as TextBlock;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            EvaluateArrowGlyph();
            return base.MeasureOverride(availableSize);
        }

        private void EvaluateArrowGlyph()
        {
            if (_arrowGlyph == null) return;

            if (FontSize <= 12)
            {
                _arrowGlyph.Text = ARROW_GLYPH_XSMALL;
            }

            if (FontSize >= 13.333 && FontSize < 21.333)
            {
                _arrowGlyph.Text = ARROW_GLYPH_SMALL;
            }

            if (FontSize >= 21.333 && FontSize < 40)
            {
                _arrowGlyph.Text = ARROW_GLYPH_MEDIUM;
            }

            if (FontSize > 40)
            {
                _arrowGlyph.Text = ARROW_GLYPH_LARGE;
            }
        }
    }
}
