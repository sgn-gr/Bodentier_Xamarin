using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class RecordToPositionTextConverter : IMarkupExtension, IValueConverter
	{
		public object Convert(object value,
								Type targetType,
								object parameter,
								System.Globalization.CultureInfo culture)
		{
			var taxon = Database.GetRecordAsync((int)value).Result;
			var type = "";
			switch (taxon.Position)
			{
				case PositionInfo.PositionOption.Pin:
					type = "Position";
					break;
				case PositionInfo.PositionOption.Line:
					type = "Transekt";
					break;
				case PositionInfo.PositionOption.Area:
					type = "Fläche";
					break;
				default:
					break;
			}
			var description = taxon.HabitatName;

			return String.Format("{0} ({1})", description, type);
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

		private static RecordDatabase database;
		public static RecordDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3"));
				}
				return database;
			}
		}
	}
}