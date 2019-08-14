﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CardTricks.Controls
{
    public class SizeChrome : Control
    {
        static SizeChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SizeChrome), new FrameworkPropertyMetadata(typeof(SizeChrome)));
        }
    }

    public class DoubleFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value / TemplateUserControl.DPI;
            return Math.Round(d,2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
