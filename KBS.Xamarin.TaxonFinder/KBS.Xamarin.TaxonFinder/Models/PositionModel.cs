using SQLite;
using System.ComponentModel;
using static KBS.App.TaxonFinder.Data.PositionInfo;

namespace KBS.App.TaxonFinder.Models
{
    public class PositionModel : INotifyPropertyChanged
	{
		[PrimaryKey, AutoIncrement]
		public int PositionId { get; set; }
		public PositionOption Type { get; set; }
		public int LocalRecordId { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public PositionModel()
		{

		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}