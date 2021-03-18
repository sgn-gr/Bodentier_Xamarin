using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
using Plugin.SimpleAudioPlayer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class TaxonMediaInfoViewModel : INotifyPropertyChanged
    {
        #region Fields

        private TaxonImage _selectedMedia;

        #endregion

        #region Properties
        public int SelectedMediaId
        {
            set
            {
                SelectedMedia = ((App)App.Current).TaxonImages.FirstOrDefault(i => i.ImageId == value.ToString());
                OnPropertyChanged(nameof(SelectedMedia));
            }
        }

        public Guid SelectedMediaGuid
        {
            set
            {
                SelectedMedia = ((App)App.Current).TaxonImages.FirstOrDefault(i => i.ImageId == value.ToString());
                OnPropertyChanged(nameof(SelectedMediaGuid));
            }
        }

        public String SelectedMediaTitle
        {
            set
            {
                SelectedMedia = ((App)App.Current).TaxonImages.FirstOrDefault(i => i.ImageId == value.ToString());
                OnPropertyChanged(nameof(SelectedMediaTitle));
            }
        }


        public TaxonImage SelectedMedia
        {
            get
            {
                return _selectedMedia == null ? new TaxonImage() : _selectedMedia;
            }

            set
            {
                _selectedMedia = value;
                OnPropertyChanged(nameof(SelectedMedia));
            }
        }
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
