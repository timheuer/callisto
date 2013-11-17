using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Callisto.Controls.Common;

namespace Callisto.Controls
{
    public class MapScale : ContentControl
    {
        private Grid _scaleGrid;
        private TextBlock _scaleText;

        public MapScale()
        {
            DefaultStyleKey = typeof (MapScale);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scaleGrid = GetTemplateChild("ScaleGrid") as Grid;
            _scaleText = GetTemplateChild("ScaleText") as TextBlock;
        }

        internal void SetScale(double width, string text)
        {
            if (_scaleGrid != null)
            {
                _scaleGrid.Width = width;
                _scaleText.Text = text;
            }
        }
    }
}
