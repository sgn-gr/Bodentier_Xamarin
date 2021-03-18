using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class VisibilityCatToColourConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,System.Globalization.CultureInfo culture)
        {
            if(value != null)
            {
                if ((int)value == 2)
                {
                    return Color.FromHex("#e8c31c");
                }
                else if ((int)value == 3)
                {
                    return Color.DarkGreen;
                }
            }
            return Color.DarkGray;
        }

        public object ConvertBack(object value,
                                    Type targetTypes,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}