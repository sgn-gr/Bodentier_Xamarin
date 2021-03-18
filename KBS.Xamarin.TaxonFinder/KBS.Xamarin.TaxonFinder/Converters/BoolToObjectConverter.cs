using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class BoolToObjectConverter<T> : IMarkupExtension ,IValueConverter
	{
		public T FalseObject { set; get; }

		public T TrueObject { set; get; }

		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			return (bool)value ? this.TrueObject : this.FalseObject;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			return ((T)value).Equals(this.TrueObject);
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}