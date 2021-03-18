using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static KBS.App.TaxonFinder.CustomRenderers.IconLabel;

namespace KBS.App.TaxonFinder.Converters
{
    public class BoolToSelectionImageConverter : IMarkupExtension, IValueConverter
	{
		public object Convert(object value,
								Type targetType,
								object parameter,
								System.Globalization.CultureInfo culture)
		{
			if ((bool)value)
				return FontIcon.CheckedCheckbox; //ImageSource.FromFile("checkbox.png");
			else
				return FontIcon.UncheckedCheckbox;  //ImageSource.FromFile("checkbox_unchecked.png");
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