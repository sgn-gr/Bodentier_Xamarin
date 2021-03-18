using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class RecordAreaViewModel
	{
		#region Fields

		private static RecordDatabase database;

		#endregion

		#region Properties
		
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
		public bool PositionsExist
		{
			get { return RecordList.Any(); }
		}
		public ObservableCollection<RecordModel> RecordList
		{
			get { return new ObservableCollection<RecordModel>(Database.GetRecordsAsync().Result); }
		}
		#endregion

		#region Constructor

		public RecordAreaViewModel()
		{
		}

		#endregion
	}
}
