using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Callisto.SettingsManagement
{
    public class SettingsUserControl : UserControl
    {
        public SettingsFlyout.SettingsFlyoutWidth SettingsWidth
        {
            get
            {
                return (SettingsFlyout.SettingsFlyoutWidth)GetValue(SettingsWidthProperty);
            }
            set
            {
                SetValue(SettingsWidthProperty, value);
            }
        }

        public static readonly DependencyProperty SettingsWidthProperty =
            DependencyProperty.Register("SettingsWidth", typeof(SettingsFlyout.SettingsFlyoutWidth), typeof(SettingsUserControl),
                                        new PropertyMetadata(SettingsFlyout.SettingsFlyoutWidth.Narrow, OnSettingsWidthPropertyChanged));

        private static void OnSettingsWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsUserControl s = d as SettingsUserControl;
            if (s != null)
            {
                s.Width = (double)e.NewValue;
            }
        }
    }
}
