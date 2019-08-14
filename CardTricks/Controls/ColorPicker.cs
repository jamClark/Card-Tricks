using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CardTricks.Controls
{
    public class ColorPicker : Xceed.Wpf.Toolkit.ColorPicker
    {

        public Color SelectedColorAdv
        {
            get { return (Color)GetValue(SelectedColorAdvProperty); }
            set { SetValue(SelectedColorAdvProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorAdvProperty =
            DependencyProperty.Register("SelectedColorAdv",
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new FrameworkPropertyMetadata(Colors.Black, OnColorPropertyChanged) );

        public ColorPicker() : base()
        {
            this.UsingAlphaChannel = true;
            this.DisplayColorAndName = true;
            this.SelectedColorChanged += OnSelectedColorChanged;
        }

        private static void OnColorPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker picker = source as ColorPicker;
            picker.SelectedColor = (Color)e.NewValue;
        }

        private void OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SelectedColorAdv = e.NewValue;
        }
    }
}
