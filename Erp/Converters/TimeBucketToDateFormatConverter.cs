using Erp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Erp.Converters
{
    public class TimeBucketToDateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BasicEnums.Timebucket timeBucket)
            {
                switch (timeBucket)
                {
                    case BasicEnums.Timebucket.Monthly:
                        return "MMM/yyyy";
                    case BasicEnums.Timebucket.Weekly:
                        return "WEEK%U/%MMM/yyyy";
                    case BasicEnums.Timebucket.Daily:
                        return "dd/MM/yyyy";
                    default:
                        return "MMM/yyyy"; // Default format for unknown values
                }
            }
            return "MMM/yyyy"; // Default format if value is not TimeBucket
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
