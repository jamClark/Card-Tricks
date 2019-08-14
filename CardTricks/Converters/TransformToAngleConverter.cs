using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CardTricks.Windows
{
    public class TransformToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RotateTransform trans = value as RotateTransform;
            if (trans != null)
            {
                return trans.Angle;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new RotateTransform(System.Convert.ToDouble(value)) as RotateTransform;
        }
    }
}
