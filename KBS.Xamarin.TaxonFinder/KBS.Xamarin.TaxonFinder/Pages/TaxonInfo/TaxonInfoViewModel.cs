using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class TaxonInfoViewModel : INotifyPropertyChanged
    {
        #region Properties

        public int ProtectionHeight { get; set; }
        public int SelectedTaxonId
        {
            set
            {
                SelectedTaxon = ((App)App.Current).Taxa.First(i => i.TaxonId == value);
                //FillProtectionList();
                OnPropertyChanged(nameof(SelectedTaxon));
                OnPropertyChanged(nameof(Image1));
                OnPropertyChanged(nameof(Image2));
                OnPropertyChanged(nameof(Image3));
                OnPropertyChanged(nameof(Image4));
                OnPropertyChanged(nameof(Images));
            }
        }


        public TaxonImage Image1
        {
            get
            {
                if (SelectedTaxon != null && SelectedTaxon.Images.Count != 0)
                {
                    return SelectedTaxon.Images[0];
                }

                return new TaxonImage();
            }
        }
        public TaxonImage Image2
        {
            get
            {
                if (SelectedTaxon != null && SelectedTaxon.Images.Count > 1)
                {
                    return SelectedTaxon.Images[1];
                }

                return new TaxonImage();
            }
        }
        public TaxonImage Image3
        {
            get
            {
                if (SelectedTaxon != null && SelectedTaxon.Images.Count > 2)
                {
                    return SelectedTaxon.Images[2];
                }

                return new TaxonImage();
            }
        }
        public TaxonImage Image4
        {
            get
            {
                if (SelectedTaxon != null && SelectedTaxon.Images.Count > 3)
                {
                    return SelectedTaxon.Images[3];
                }

                return new TaxonImage();
            }
        }

        public bool AllowTaxonInfoView ()
        {
            return SelectedTaxon.TaxonomyStateName == "sp." ? true : false;
        }

        public Taxon SelectedTaxon { get; set; }
        public ObservableCollection<ProtectionInfo> ProtectionInfos { get; set; }

        public List<TaxonImage> Images
        {
            get
            {
                if (SelectedTaxon != null && SelectedTaxon.Images.Count != 0)
                {
                    return SelectedTaxon.Images.ToList();
                }

                return new List<TaxonImage>();
            }
        }


        #endregion

        #region Constructor

        public TaxonInfoViewModel()
        {
            SaveTaxonCommand = new Command(async () => await SaveTaxon());
            ImageTappedCommand = new Command<string>(async arg => await ImageTapped(arg));
            NavigateToWebCommand = new Command<Taxon>(async arg => await NavigateToWeb(arg));
            //ProtectionInfos = new ObservableCollection<ProtectionInfo>();
        }

        #endregion

        #region SaveTaxa Command

        public ICommand SaveTaxonCommand { get; set; }
        private async Task SaveTaxon()
        {
            await App.Current.MainPage.Navigation.PushAsync(new RecordEdit(SelectedTaxon.TaxonId));
        }

        #endregion

        #region NavigateToWeb Command

        public ICommand NavigateToWebCommand { get; set; }
        private async Task NavigateToWeb(Taxon taxon)
        {
            string urlEndpoint = taxon.TaxonName.ToLower().Replace(' ', '-');
            await Launcher.OpenAsync(new Uri($"http://bodentierhochvier.de/steckbrief/{urlEndpoint}"));
        }

        #endregion

        #region ImageTapped Command

        public ICommand ImageTappedCommand { get; set; }
        private async Task ImageTapped(string imageTitle)
        {
            if (imageTitle != null)
            {
                await App.Current.MainPage.Navigation.PushAsync(new TaxonMediaInfo(imageTitle));
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Fills the ProtectionList based on the current Taxon, sets the height of the ListView.
        /// </summary>
        private void FillProtectionList()
        {
            var taxonProtectionClassList = ((App)App.Current).TaxonProtectionClasses.FindAll(i => i.TaxonId == SelectedTaxon.TaxonId);
            if (!taxonProtectionClassList.Any(i => i.ClassId == 2))
                taxonProtectionClassList.Insert(0, new TaxonProtectionClass() { ClassId = 2, TaxonId = SelectedTaxon.TaxonId, ClassValue = "nicht besonders geschützt" });
            foreach (var protectionClass in taxonProtectionClassList)
            {
                ProtectionInfo protectionInfo = new ProtectionInfo();
                protectionInfo.ProtectionStatus = protectionClass.ClassValue;
                //protectionInfo.ProtectionName = ((App)App.Current).ProtectionClasses.First(i => i.ClassId == protectionClass.ClassId).ClassName;
                //ProtectionInfos.Add(protectionInfo);
            }
            ProtectionHeight = ProtectionInfos.Count() * 45;
            OnPropertyChanged(nameof(ProtectionHeight));
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
