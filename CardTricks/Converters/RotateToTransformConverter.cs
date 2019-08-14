using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CardTricks.Windows
{
    public class RotateToTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RotateTransform rot = value as RotateTransform;
            if(rot != null)
            {
                return rot as Transform;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Transform trans = value as RotateTransform;
            if (trans != null)
            {
                return trans as RotateTransform;
            }

            return null;
        }
    }
}
