using SQLite;
using System;
using System.ComponentModel;
using System.Linq;

namespace KBS.App.TaxonFinder.Models
{
	public class RecordModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private double _longitude;
		private double _latitude;
		private double? _height;
		private double? _accuracy;
		private bool _isEditable;
		private bool _autoPosition;
		public string UserName;

		[PrimaryKey, AutoIncrement]
		public int LocalRecordId { get; set; }
		public bool IsEditable { get { return _isEditable; } set { _isEditable = value; OnPropertyChanged(nameof(IsEditable)); } }
		public Guid Identifier { get; set; }
		public string HabitatName { get; set; }
		public string HabitatDescription { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime RecordDate { get; set; }
		public int TaxonId { get; set; }
		public double Latitude { get { return Math.Round(_latitude, 7); ; } set { _latitude = value; OnPropertyChanged(nameof(Latitude)); } }
		public double Longitude { get { return Math.Round(_longitude, 7); } set { _longitude = value; OnPropertyChanged(nameof(Longitude)); } }
		public double? Height { get { return _height; } set { _height = value; OnPropertyChanged(nameof(Height)); } }
		public double? Accuracy { get { return _accuracy; } set { _accuracy = value; OnPropertyChanged(nameof(Accuracy)); } }
		public int MaleCount { get; set; }
		public int FemaleCount { get; set; }
		public string ReportedByName { get; set; }
		public int TotalCount { get; set; }
		public bool AutoPosition { get { return _autoPosition; } set { _autoPosition = value; OnPropertyChanged(nameof(AutoPosition)); } }
		public Data.PositionInfo.PositionOption Position { get; set; }

		public RecordModel()
		{
		}
		public string SearchString(int taxonId)
		{

			var tempTaxonInfo = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == taxonId);
			if (tempTaxonInfo == null)
				return "";
			return (string.IsNullOrEmpty(tempTaxonInfo.TaxonName) ? "" : tempTaxonInfo.TaxonName) + (string.IsNullOrEmpty(tempTaxonInfo.LocalName) ? "" : tempTaxonInfo.LocalName) + (string.IsNullOrEmpty(tempTaxonInfo.FamilyName) ? "" : tempTaxonInfo.FamilyName) + (string.IsNullOrEmpty(tempTaxonInfo.FamilyLocalName) ? "" : tempTaxonInfo.FamilyLocalName) + (string.IsNullOrEmpty(HabitatName) ? "" : HabitatName) + (string.IsNullOrEmpty(HabitatDescription) ? "" : HabitatDescription);

		}
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
