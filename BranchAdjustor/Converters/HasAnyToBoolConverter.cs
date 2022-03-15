using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#nullable disable

namespace BranchAdjustor
{
    [ValueConversion(typeof(ObservableCollection<AdjustBranchResult>), typeof(bool))]
    public class HasAnyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var disputeRecs = (ObservableCollection<AdjustBranchResult>)value;

            return disputeRecs.Any();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
