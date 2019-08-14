using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace CardTricks.Windows
{
    [ValueConversion(typeof(object), typeof(double))]
    public class InchToDpiConverter : IValueConverter
    {
        readonly double DPI = 96.0;
            
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double size = 0.0;
            string s = value as string;
            if(s != null)
            {
                if(double.TryParse(s, out size)) return size;
            }
            

            double val = (double)value * DPI;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
