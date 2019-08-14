using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CardTricks.Controls
{
    public class DoubleUpDown : Xceed.Wpf.Toolkit.DoubleUpDown
    {
        public double? ValueDependency
        {
            get { return (double?)GetValue(ValueDependencyProperty); }
            set { SetValue(ValueDependencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueDependencyProperty =
            DependencyProperty.Register("ValueDependency",
                                        typeof(double?),
                                        typeof(DoubleUpDown),
                                        new FrameworkPropertyMetadata(0.0, OnDependencyPropertyChanged));

        public DoubleUpDown()
        {
            this.ValueChanged += OnLocalPropertyChanged;
            this.TextAlignment = System.Windows.TextAlignment.Center;
        }

        public static void OnDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DoubleUpDown obj = sender as DoubleUpDown;
            obj.Value = e.NewValue as double?;
        }

        public void OnLocalPropertyChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ValueDependency = e.NewValue as double?;
            
        }
    }
}
