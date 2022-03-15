using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable

namespace BranchAdjustor
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return bool.Parse(value.ToString()) ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return bool.Parse(value.ToString()) ? false : true;
        }
    }
}
