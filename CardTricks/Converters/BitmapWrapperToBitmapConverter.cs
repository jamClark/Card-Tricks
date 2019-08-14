using CardTricks.Models.Elements;
using CardTricks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CardTricks.Converters
{
    public class BitmapWrapperToBitmapConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapWrapper wrapper = value as BitmapWrapper;
            if (wrapper != null)
            {
                return wrapper.Image;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;// throw new NotImplementedException();
        }
    }
}
