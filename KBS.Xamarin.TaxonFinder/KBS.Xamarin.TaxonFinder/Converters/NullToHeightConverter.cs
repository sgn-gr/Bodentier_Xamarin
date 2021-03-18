using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class NullToHeightConverter : IMarkupExtension, IValueConverter
	{
		public object Convert(object value,
								Type targetType,
								object parameter,
								CultureInfo culture)
		{
			var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
			var screenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
			return value != null ? screenWidth/2.85 : 0;
		}

		public object ConvertBack(object value,
									Type targetType,
									object parameter,
									CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}