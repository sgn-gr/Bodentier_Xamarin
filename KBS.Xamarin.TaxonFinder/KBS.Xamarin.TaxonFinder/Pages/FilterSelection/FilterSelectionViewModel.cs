using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
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
    public class FilterSelectionViewModel : INotifyPropertyChanged
    {
        #region Fields
        public int _qCount;
        private bool _showSelectedOnly = true;
        private bool _showFilterSelection = true;
        private bool _isNotValueDataType = true;
        private bool _isValueDataType = false;
        private int _sliderValue;
        List<TaxonTagFilterFilterGroup> _tFilterGroups;
        public List<int> _tagFilterIds;
        public List<int> _tagCategoryFilterIds;
        public List<int> _taxonIds;
        private FilterItem _currentItem;
        #endregion

        private Dictionary<string, string> orderToUrlList = new Dictionary<string, string>(){
            {"Bodentiere", "bestimmung-bodentiere"},
            {"Doppelfüßer (Diplopoda)", "bestimmung-doppelfuesser"},
            {"Samenfüßer (Chordeumatida)", "bestimmung-doppelfuesser"},
            {"Bandfüßer (Polydesmida)", "bestimmung-doppelfuesser"},
            {"Schnurfüßer (Julida)", "bestimmung-doppelfuesser"},
            {"Saftkugler (Glomerida)", "bestimmung-doppelfuesser"},
            {"Pinselfüßer (Polyxenida)", "bestimmung-doppelfuesser"},
            {"Bohrfüßer (Polyzoniida)", "bestimmung-doppelfuesser"},
            {"Hundertfüßer (Chilopoda)", "bestimmung-hunderfuesser-neu" },
            {"Steinläufer (Lithobiomorpha)", "bestimmung-hunderfuesser-neu" },
            {"Skolopender(Scolopendromorpha)", "bestimmung-hunderfuesser-neu" },
            {"Erdkriecher (Geophilomorpha)", "bestimmung-hunderfuesser-neu" },
            {"Spinnenläufer (Scutigeromorpha)", "bestimmung-hunderfuesser-neu" },
            {"Asseln (Isopoda)", "bestimmung-landasseln" }
        };


        #region Properties
        public int SliderValue
        {
            set
            {
                if (_sliderValue != value)
                {
                    _sliderValue = value;
                    UpdateFilter(_sliderValue);
                }
            }
            get
            {
                return _sliderValue;
            }
        }

        private List<int> GetTaxonIdsForValue(FilterItem fItem, int sliderValue)
        {
            var taxa = ((App)App.Current).Taxa;
            var taxIds = taxa.Select(t => t.TaxonId).ToList();
            var taxonFilteredIds_temp = fItem.SubOptions.Where(fi => (fi.MaxValue >= sliderValue) && (fi.MinValue <= sliderValue)).Select(fi => fi.TaxonId).ToList();
            return taxonFilteredIds_temp;
        }

        private void UpdateFilter(int sliderValue)
        {
            try
            {
                int fItemId = _currentItem.TagId;
                if (_currentItem.BaseItem.ValueFilterDictionary.ContainsKey(fItemId))
                {
                    _currentItem.BaseItem.ValueFilterDictionary[fItemId] = GetTaxonIdsForValue(_currentItem, sliderValue);
                }
                else
                {
                    _currentItem.BaseItem.ValueFilterDictionary.Add(fItemId, GetTaxonIdsForValue(_currentItem, sliderValue));
                }
                OnPropertyChanged(nameof(SelectedTaxonList));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsNotValueDataType
        {
            get
            {
                return _isNotValueDataType;
            }
            set
            {
                _isNotValueDataType = value;
                OnPropertyChanged(nameof(IsNotValueDataType));
            }
        }
        public bool IsValueDataType
        {
            get
            {
                return _isValueDataType;
            }
            set
            {
                _isValueDataType = value;
                OnPropertyChanged(nameof(IsValueDataType));
            }
        }

        public bool ShowCatUpCommand
        {
            get
            {
                //return !ShowCatUpCommand;
                return !ShowFilterSelection ? ShowFilterSelection : _currentItem != null && _currentItem.ParentItem != null;
            }
        }
        private bool enableReset;
        public bool EnableReset
        {
            get
            {
                return enableReset;
            }
            set
            {
                enableReset = value;
                OnPropertyChanged("EnableReset");
            }
        }
        private bool enableHide;
        public bool EnableHide
        {
            get
            {
                return enableHide;
            }
            set
            {
                enableHide = value;
                OnPropertyChanged("EnableHide");
            }
        }

        public bool ShowCategoryList
        {
            get
            {
                return CategoryItems.Count != 0 && ShowFilterSelection;
            }
        }

        public bool ShowFilterList
        {
            get
            {
                if (_currentItem != null)
                {
                    if (_currentItem.KeyDataType == "VALUE")
                    {
                        _isNotValueDataType = false;
                        _isValueDataType = true;
                    }
                    else
                    {
                        _isNotValueDataType = true;
                        _isValueDataType = false;
                    }
                    OnPropertyChanged(nameof(IsNotValueDataType));
                    OnPropertyChanged(nameof(IsValueDataType));
                }
                return FilterItems.Count != 0 && ShowFilterSelection;
            }
        }

        public bool ShowSelectedOnly
        {
            get
            {
                return _showSelectedOnly;// ((KBS.App.TaxonFinder.App)App.Current).ShowOnlyMatchingTaxa;
            }
            set
            {
                _showSelectedOnly = value;
                OnPropertyChanged(nameof(SelectedTaxonCount));
                OnPropertyChanged(nameof(SelectedTaxonList));
            }
        }

        public bool ShowFilterSelection
        {
            get
            {
                return _showFilterSelection;
            }
            set
            {
                _showFilterSelection = value;
                OnPropertyChanged(nameof(SelectedTaxonCount));
                OnPropertyChanged(nameof(SelectedTaxonList));
                OnPropertyChanged(nameof(IsNotValueDataType));
                OnPropertyChanged(nameof(ShowCategoryList));
                OnPropertyChanged(nameof(ShowFilterList));
            }
        }

        public string _filterTagGroupName;
        public string FilterTagGroupName
        {
            get
            {
                return _filterTagGroupName;
            }
            set
            {
                //Why Filter == null?
                if (((KBS.App.TaxonFinder.App)App.Current).Filter != null)
                {
                    _filterTagGroupName = value;
                    _tagFilterIds = FindTagFilterIdsByFilterGroupName(value);
                    _tagCategoryFilterIds = FindTagCategoryItemIdsByFilterGroupName(value);
                    _taxonIds = FindTaxonIdsByFilterGroupName(value);

                    //List<FilterItem> filterList = ((KBS.App.TaxonFinder.App)App.Current).Filter.FindByTagId

                    _currentItem = ((KBS.App.TaxonFinder.App)App.Current).Filter.FindByTagId(0, true);
                    //_currentItem.BaseItem.PropertyChanged -= _currentItem_PropertyChanged;
                    //_currentItem.BaseItem.PropertyChanged += _currentItem_PropertyChanged;

                    ((Command)GotoParentCommand).ChangeCanExecute();

                    CategoryItems.Clear();
                    FilterItems.Clear();
                    _currentItem.BaseItem.ValueFilterDictionary?.Clear();
                    _currentItem.clearSelectedItems();

                    if (_currentItem.HasSubOptions)
                    {
                        foreach (var opt in _currentItem.SubOptions.Where(i => i.HasSubOptions && _tagCategoryFilterIds.Contains(i.TagId)))
                        {
                            CategoryItems.Add(opt);
                        }

                        foreach (var opt in _currentItem.SubOptions.Where(i => !i.HasSubOptions && _tagFilterIds.Contains(i.TagId)))
                        {
                            FilterItems.Add(opt);
                        }
                    }
                    OnPropertyChanged(nameof(CurrentItemText));
                    OnPropertyChanged(nameof(ShowCatUpCommand));
                    OnPropertyChanged(nameof(ShowCategoryList));
                    OnPropertyChanged(nameof(ShowFilterList));
                    OnPropertyChanged(nameof(SelectedTaxonList));
                    OnPropertyChanged(nameof(SelectedTaxonCount));
                    OnPropertyChanged(nameof(IsNotValueDataType));
                    OnPropertyChanged(nameof(IsValueDataType));
                    OnPropertyChanged(nameof(SliderValue));
                }
            }
        }

        private List<int> FindTagCategoryItemIdsByFilterGroupName(string value)
        {
            TaxonTagFilterFilterGroup ttffg = _tFilterGroups.FirstOrDefault(fg => fg.GroupName == value);
            if (ttffg != null)
            {
                return ttffg.DKGIds;
            }
            return null;
        }

        private List<int> FindTagFilterIdsByFilterGroupName(string value)
        {
            TaxonTagFilterFilterGroup ttffg = _tFilterGroups.FirstOrDefault(fg => fg.GroupName == value);
            if (ttffg != null)
            {
                return ttffg.DKIds;
            }
            return null;
        }

        private List<int> FindTaxonIdsByFilterGroupName(string value)
        {
            TaxonTagFilterFilterGroup ttffg = _tFilterGroups.FirstOrDefault(fg => fg.GroupName == value);
            if (ttffg != null)
            {
                return ttffg.TaxonIds;
            }
            return null;
        }

        public int FilterTag
        {
            set
            {
                //Why Filter == null?
                if (((KBS.App.TaxonFinder.App)App.Current).Filter != null)
                {
                    _currentItem = ((KBS.App.TaxonFinder.App)App.Current).Filter.FindByTagId(value, true);
                    _currentItem.BaseItem.PropertyChanged -= _currentItem_PropertyChanged;
                    _currentItem.BaseItem.PropertyChanged += _currentItem_PropertyChanged;


                    ((Command)GotoParentCommand).ChangeCanExecute();

                    CategoryItems.Clear();
                    FilterItems.Clear();
                    _currentItem.BaseItem.ValueFilterDictionary?.Clear();

                }

                if (_currentItem.HasSubOptions)
                {
                    foreach (var opt in _currentItem.SubOptions.Where(i => i.HasSubOptions && _tagCategoryFilterIds.Contains(i.TagId)))
                    {
                        CategoryItems.Add(opt);
                    }

                    foreach (var opt in _currentItem.SubOptions.Where(i => !i.HasSubOptions && _tagFilterIds.Contains(i.TagId)))
                    {
                        FilterItems.Add(opt);
                    }
                }
                OnPropertyChanged(nameof(CurrentItemText));
                OnPropertyChanged(nameof(ShowCatUpCommand));
                OnPropertyChanged(nameof(ShowCategoryList));
                OnPropertyChanged(nameof(ShowFilterList));
                OnPropertyChanged(nameof(SelectedTaxonList));
                //OnPropertyChanged(nameof(SelectedTaxonCount));
                OnPropertyChanged(nameof(IsNotValueDataType));
                OnPropertyChanged(nameof(IsValueDataType));
                OnPropertyChanged(nameof(SliderValue));
            }
        }

        #region ImageTapped Command

        public ICommand ImageTappedCommand { get; set; }
        private async Task ImageTapped(string imageId)
        {
            if (imageId != null)
            {
                await App.Current.MainPage.Navigation.PushAsync(new TaxonMediaInfo(imageId));
            }
        }
        #endregion

        public string SelectedTaxonCount
        {
            get
            {
                if (_currentItem != null)
                {
                    EnableReset = _currentItem.BaseItem.SelectedItems.Count != 0;
                    OnPropertyChanged(nameof(EnableReset));
                    //EnableReset = _currentItem.BaseItem.HasSelectedItems;
                }
                var result = _currentItem == null || !ShowSelectedOnly ? "" : string.Format("{0}", _currentItem.BaseItem.MatchingTaxonCount(_taxonIds));
                return result;
                //return _currentItem == null || !ShowAllTaxa ? "" : string.Format("{0} Taxa", _currentItem.BaseItem.MatchingTaxonCount.ToString());             
            }
        }

        public string CurrentItemText
        {
            get
            {
                return _currentItem != null ? _currentItem.OptionText : "";
            }
        }
        public string ShowFilterButtonText
        {
            get
            {
                return ShowFilterSelection ? "Filter ausblenden" : "Filter anzeigen";
            }
        }

        public string ShowChevronUpDown
        {
            get
            {
                return ShowFilterSelection ? "\u25BC" : "\u25B2";
            }
        }

        public List<Taxon> SelectedTaxonList
        {
            get
            {
                if (_currentItem != null)
                {

                    var taxa = ((App)App.Current).Taxa.Where(tx => _taxonIds.Contains(tx.TaxonId));
                    var matching = ShowSelectedOnly ? _currentItem.BaseItem.MatchingTaxonByExclusion(_taxonIds) : _currentItem.BaseItem.MatchingTaxonByRelevance(_taxonIds);

                    var q = (from t in taxa join s in matching on t.TaxonKey equals s.TaxonKey orderby s.Weight descending select t).ToList();

                    for (int i = 0; i < q.Count; i++)
                    {
                        q[i].Relevance = (matching.FirstOrDefault(j => j.TaxonKey == q[i].TaxonKey) ?? new TaxonSearchResult()).Weight;
                        //q[i].ShowAll = !ShowAllTaxa;
                        q[i].ShowAll = !ShowSelectedOnly;
                    }
                    EnableHide = (q.Count != 0);
                    return q.ToList();
                }
                List<Taxon> allTaxa = ((App)App.Current).Taxa.ToList();
                if (_taxonIds != null)
                {
                    allTaxa = ((App)App.Current).Taxa.Where(tax => _taxonIds.Contains(tax.TaxonId)).ToList();
                }
                return allTaxa;
                //return new List<Taxon>();
            }
        }
        public ObservableCollection<FilterItem> FilterItems { get; set; }
        public ObservableCollection<FilterItem> CategoryItems { get; set; }

        #endregion

        #region Constructor

        public FilterSelectionViewModel()
        {
            _taxonIds = new List<int>();
            _tFilterGroups = Load.FromFile<TaxonTagFilterFilterGroup>("TaxonTagFilterFilterGroups.json");
            _sliderValue = 0;
            ImageTappedCommand = new Command<string>(async arg => await ImageTapped(arg));
            FilterItems = new ObservableCollection<FilterItem>();
            CategoryItems = new ObservableCollection<FilterItem>();
            ShowSelectedTaxonCommand = new Command(() => ShowSelectedTaxon());
            ResetSelectedTaxonCommand = new Command(() => ResetSelectedTaxon());
            GotoParentCommand = new Command(GotoParent, GotoParentCanExecute);
            ShowSelectedOnly = true;
            NavigateToWebCommand = new Command<Taxon>(async arg => await NavigateToWeb());
            if (_currentItem != null)
            {
                ResetSelectedTaxon();
            }
            FilterItems.Clear();
            CategoryItems.Clear();
        }

        #endregion

        #region ShowSelectedTaxon Command

        public ICommand ShowSelectedTaxonCommand { get; set; }

        private void ShowSelectedTaxon()
        {
            /*App.Current.MainPage.Navigation.PushAsync(new TaxonList(ShowSelectedOnly ? _currentItem.BaseItem.MatchingTaxonByExclusion : _currentItem.BaseItem.MatchingTaxonByRelevance));*/
            ShowFilterSelection = !ShowFilterSelection;
            OnPropertyChanged(nameof(ShowFilterSelection));
            OnPropertyChanged(nameof(ShowFilterButtonText));
            OnPropertyChanged(nameof(ShowChevronUpDown));
            OnPropertyChanged(nameof(IsNotValueDataType));
            OnPropertyChanged(nameof(ShowCatUpCommand));
            //OnPropertyChanged(nameof(SliderValue));

        }

        #endregion

        #region NavigateToWeb Command

        public ICommand NavigateToWebCommand { get; set; }
        private async Task NavigateToWeb()
        {
            string slug = "bestimmung-bodentiere";
            if (orderToUrlList.TryGetValue(FilterTagGroupName, out var _slug))
            {
                slug = _slug;
            }
            string uri = $"https://bodentierhochvier.de/erkennen/{slug}";
            await Launcher.OpenAsync(new Uri(uri));
        }

        #endregion

        #region ResetSelectedTaxon Command

        public ICommand ResetSelectedTaxonCommand { get; set; }
        private void ResetSelectedTaxon()
        {
            _currentItem.clearSelectedItems();
            while (_currentItem != null && _currentItem.ParentItem != null)
                GotoParent(true);
            EnableReset = false;
            if (!ShowFilterSelection && !ShowSelectedOnly)
            {
                ShowFilterSelection = true;
                OnPropertyChanged(nameof(ShowFilterSelection));
            }
        }

        #endregion

        #region GotoParent Command

        public ICommand GotoParentCommand { get; set; }

        private bool GotoParentCanExecute(object arg)
        {
            return _currentItem != null && _currentItem.ParentItem != null;
        }
        private void GotoParent(object obj)
        {
            FilterTag = _currentItem.ParentItem.TagId;
        }

        #endregion


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        private void _currentItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "MatchingTaxonCount")
            {
                OnPropertyChanged(nameof(SelectedTaxonList));
                OnPropertyChanged(nameof(SelectedTaxonCount));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        #endregion
    }
}
