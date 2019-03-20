using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Store.Helpers
{
    public class ConverterWidthToShorter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double widthlenght = 0;
            if (value != null)
            {
                widthlenght = (double)value;
                return widthlenght - 5;
            }
            return widthlenght;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
