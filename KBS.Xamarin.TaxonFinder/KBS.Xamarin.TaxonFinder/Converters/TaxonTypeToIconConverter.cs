using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class TaxonTypeToIconConverter : IMarkupExtension, IValueConverter
	{
		public object Convert(object value,
								Type targetType,
								object parameter,
								System.Globalization.CultureInfo culture)
		{
			int taxonTypeId = ((int?)value ?? (int?)20005).Value;

			switch (taxonTypeId)
			{
				case 20000:
					return CustomRenderers.IconLabel.FontIcon.Mars;
				case 20001:
					return CustomRenderers.IconLabel.FontIcon.Venus;
				case 20003:
					return CustomRenderers.IconLabel.FontIcon.Mars;
				case 20004:
					return CustomRenderers.IconLabel.FontIcon.Venus;
				case 20005:
					return CustomRenderers.IconLabel.FontIcon.VenusMars;
			}

			return CustomRenderers.IconLabel.FontIcon.Unknown;
		}

		public object ConvertBack(object value,
									Type targetTypes,
									object parameter,
									System.Globalization.CultureInfo culture)
		{
			return null;
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}