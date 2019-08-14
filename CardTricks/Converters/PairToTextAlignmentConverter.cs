using CardTricks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CardTricks.Converters
{
    public class PairToTextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Pair pair = value as Pair;
            if (pair != null)   return pair.Value;
            return TextAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TextAlignment alignment = (TextAlignment)value;
            Pair pair = new Pair(alignment.ToString(), alignment);
            return pair;

        }
    }
}
