using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class BoolToSelectionFontConverter : IMarkupExtension, IValueConverter
	{
		public object Convert(object value,
								Type targetType,
								object parameter,
								System.Globalization.CultureInfo culture)
		{
			if ((bool)value)
				return Font.SystemFontOfSize(NamedSize.Large, FontAttributes.Bold);
			else
				return Font.SystemFontOfSize(NamedSize.Large, FontAttributes.None);
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