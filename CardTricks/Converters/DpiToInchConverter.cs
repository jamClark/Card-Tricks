using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CardTricks.Windows
{
    [ValueConversion(typeof(object), typeof(string))]
    public class DpiToInchConverter : IValueConverter
    {
        readonly double DPI = 96.0;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Double val = ((double)value / DPI);
            return val.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double size = 0.0;
            string s = value as string;
            if (s != null)
            {
                if (double.TryParse(s, out size))
                {
                    return size * DPI;
                }
                //this is not the correct value, but if something goes
                //wrong at least it won't throw and exception
                return size;
            }
            if (value == null) return 0.0;
            double val = (double)value * DPI;
            return val;
        }
    }
}
