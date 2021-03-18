using SQLite;
using System.ComponentModel;

namespace KBS.App.TaxonFinder.Models
{
    public class MediaFileModel : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int MediaId { get; set; }
        public int LocalRecordId { get; set; }
        public string Path { get; set; }
        public MediaType Media { get; set; }
        public enum MediaType
        {
            Image,
            Audio
        }

        public MediaFileModel()
        {

        }

        public MediaFileModel(string path)
        {
            Path = path;
        }

        public MediaFileModel(string path, int localRedcordId, MediaType media)
        {
            Path = path;
            LocalRecordId = localRedcordId;
            Media = media;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
