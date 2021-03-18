using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{

    public class RecordListViewModel : INotifyPropertyChanged
    {
        #region Fields

        private static RecordDatabase _database;
        private readonly IMobileApi _mobileApi;
        private bool _isBusy;
        private string _searchText = "";
        private string _syncButtonText;
        private string _result;
        private DateTime _fromDate;
        private DateTime _toDate;
        private ObservableCollection<RecordModel> _selectedRecordList;

        #endregion

        #region Properties

        public static RecordDatabase Database
        {
            get
            {
                return _database ?? (_database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3")));
            }
        }
        public bool Unauthorized { get; set; }
        public bool NewRecord { get; set; }
        public bool ShowSyncButton { get; set; }
        public bool EnableSearch { get; set; }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                UpdateRecordList();
            }
        }
        public string SyncButtonText
        {
            get { return _syncButtonText; }
            set
            {
                _syncButtonText = value;
                OnPropertyChanged(nameof(SyncButtonText));
            }
        }
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                UpdateRecordList();
            }
        }
        public DateTime ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                UpdateRecordList();
            }
        }
        public List<RecordModel> Records { get; set; }
        public ObservableCollection<RecordModel> SelectedRecordList
        {
            get { return _selectedRecordList; }
            set
            {
                _selectedRecordList = value;
                OnPropertyChanged(nameof(SelectedRecordList));
            }
        }

        #endregion

        #region Constructor

        public RecordListViewModel()
        {
            GetSelectedRecord();
            SaveTaxaCommand = new Command(async () => await SaveTaxa());
            CopyCommand = new Command<int>(async arg => await Copy(arg));
            _mobileApi = DependencyService.Get<IMobileApi>();
            SyncButtonText = "Funde synchronisieren";
            OnPropertyChanged(nameof(SyncButtonText));
            ShowSyncButton = true;
            OnPropertyChanged(nameof(ShowSyncButton));
            NavigateToWebCommand = new Command<Taxon>(async arg => await NavigateToWeb(arg));
        }

        #endregion

        #region NavigateToWeb Command

        public ICommand NavigateToWebCommand { get; set; }
        private async Task NavigateToWeb(Taxon taxon)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new Uri($"https://bodentierhochvier.de/erfassen/funde"));
        }

        #endregion


        #region SaveTaxa Command

        public ICommand SaveTaxaCommand { get; set; }

        private async Task SaveTaxa()
        {
            try
            {
                // "Neue Fundmeldung anlegen"
                if (NewRecord)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new TaxonList(NewRecord));
                    throw new Exception("");
                }
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    throw new Exception("Zur Synchronisation Internetverbindung herstellen.");
                }
                //go to login > open login
                if (Unauthorized)
                {
                    Unauthorized = false;
                    await App.Current.MainPage.Navigation.PushAsync(new RegisterDevice());
                    SyncButtonText = "Funde synchronisieren";
                    throw new Exception("");
                }
                //unauthorized > hint: go to login
                else if (Database.GetRegister() == null)
                {
                    Unauthorized = true;
                    SyncButtonText = "Anmeldung öffnen";
                    throw new UnauthorizedAccessException("Zur Synchronisation von Fundmeldungen anmelden.");
                }

                //synchronize if no problems
                RecordEditViewModel revm = new RecordEditViewModel();
                var count = 0;
                string result;
                IsBusy = true;
                foreach (RecordModel taxaItem in SelectedRecordList.Where(i => i.IsEditable))
                {
                    try
                    {
                        revm.SelectedRecordId = taxaItem.LocalRecordId;
                        AdviceJsonItem[] advItem = { revm.SaveTaxa() };
                        result = await _mobileApi.SaveAdvicesByDevice(advItem);
                        ResultObj result_obj = JsonConvert.DeserializeObject<ResultObj>(result);
                        if (result_obj.succeeded == true)
                        {
                            Database.SetSynced(advItem[0].AdviceId);
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                var backSynchronizedCount = await SynchronizeBack();
                count += backSynchronizedCount;
                if (count == 0)
                {
                    result = "Fundmeldungen sind aktuell";
                }
                else
                {
                    result = "Fundmeldungen synchronisiert";
                }
                IsBusy = false;

                SelectedRecordList.Clear();
                Records = await Database.GetRecordsAsync();
                SelectedRecordList = new ObservableCollection<RecordModel>(Records);
                Result = result;
                ShowSyncButton = false;
                OnPropertyChanged(nameof(ShowSyncButton));
            }
            catch (Exception e)
            {
                IsBusy = false;
                Result = e.Message;
            }
        }

        private async Task<int> SynchronizeBack()
        {
            var count = 0;
            try
            {
                var deviceId = DependencyService.Get<IDeviceId>().GetDeviceId();
                var deviceHash = (await Database.GetRegister()).DeviceHash;
                var auth = new AuthorizationJson(deviceId, deviceHash);

                var content = await _mobileApi.GetChangesByDevice(auth);
                var unescapedContent = JsonConvert.DeserializeObject(content) as string;
                var changesList = JsonConvert.DeserializeObject<List<AdviceJsonItem>>(unescapedContent);
                var allRecords = await Database.GetRecordsAsync();
                foreach (var changedAdvice in changesList)
                {
                    var record = allRecords.Find(i => i.Identifier == changedAdvice.Identifier);
                    if (record != null)
                    {
                        record.TaxonId = changedAdvice.TaxonId;
                        record.RecordDate = changedAdvice.AdviceDate ?? DateTime.MinValue;
                        record.Latitude = (double)changedAdvice.Lat;
                        record.Longitude = (double)changedAdvice.Lon;
                        await Database.UpdateRecord(record);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return count;
        }

        #endregion

        #region Copy Command

        public ICommand CopyCommand { get; set; }
        private async Task Copy(int localRecordId)
        {
            await App.Current.MainPage.Navigation.PushAsync(new TaxonList(localRecordId));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the records to show in the list, initializes the search.
        /// </summary>
        private async void GetSelectedRecord()
        {
            Records = await Database.GetRecordsAsync();

            EnableSearch = (Records.Count != 0);
            OnPropertyChanged(nameof(EnableSearch));
            SelectedRecordList = new ObservableCollection<RecordModel>(Records);
        }

        /// <summary>
        /// Updates the RecordList based on the search.
        /// </summary>
        private void UpdateRecordList()
        {
            if (EnableSearch)
            {
                var results = new List<RecordModel>();

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    results.AddRange(Records.Where(i => i.RecordDate >= FromDate).Where(i => i.RecordDate <= ToDate));
                }
                else
                {
                    var searchText = SearchText.Split(' ');

                    bool isFirstQuery = true;

                    foreach (string item in searchText)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (isFirstQuery)
                            {
                                results.AddRange(Records.Where(i => i.RecordDate >= FromDate).Where(i => i.RecordDate <= ToDate).Where(i => i.SearchString(i.TaxonId).ToLower().Contains(item.ToLower())));
                            }
                            else
                            {
                                results = results.Intersect(Records.Where(i => i.RecordDate >= FromDate).Where(i => i.RecordDate <= ToDate).Where(i => i.SearchString(i.TaxonId).ToLower().Contains(item.ToLower()))).ToList();
                            }
                            isFirstQuery = false;
                        }
                    }
                }

                SelectedRecordList.Clear();
                SelectedRecordList = new ObservableCollection<RecordModel>(results);
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

        public class ResultObj
        {
            public bool succeeded { get; set; }
            public string [] errors { get; set; }
            public int ObservationId { get; set; }
        }

    }
}
