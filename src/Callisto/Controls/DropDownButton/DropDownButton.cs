using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Callisto.Controls
{
    public sealed class DropDownButton : Button
    {
        private TextBlock _arrowGlyph;
        private const string PART_ARROW_GLYPH = "ArrowGlyph";
        private const double FONT_SIZE_THRESHOLD = 36;
        private const string ARROW_GLYPH_SMALL = "\uE099";

        public DropDownButton()
        {
            this.DefaultStyleKey = typeof(DropDownButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _arrowGlyph = GetTemplateChild(PART_ARROW_GLYPH) as TextBlock;

            // TODO: if the FontSize property changes we need to run this logic again
            if (_arrowGlyph != null)
            {
                if (this.FontSize < FONT_SIZE_THRESHOLD)
                {
                    _arrowGlyph.Text = ARROW_GLYPH_SMALL;
                }
            }
        }
    }
}
