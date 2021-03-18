using KBS.App.TaxonFinder.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{

    public class TaxonListViewModel : INotifyPropertyChanged
    {

        #region Fields

        private bool _newRecord;
        private string _searchText = "";

        #endregion

        #region Properties
        public bool EnableSearchbar { get; set; }
        public bool NewRecord
        {
            get
            {
                return _newRecord;
            }
            set
            {
                _newRecord = value;
                if (_newRecord)
                {
                    BlankSize = new GridLength(1, GridUnitType.Star);
                    ImageSize = new GridLength(0);
                }
                else
                {
                    UpdateSelectedTaxonListSource();
                    BlankSize = new GridLength(0);
                    ImageSize = new GridLength(2, GridUnitType.Star);
                }
                OnPropertyChanged(nameof(BlankSize));
                OnPropertyChanged(nameof(ImageSize));
            }
        }
        public GridLength ImageSize { get; set; }
        public GridLength BlankSize { get; set; }

        public int? LocalRecordIdCopy { get; set; }
        public string PlaceholderText { get; set; }


        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (EnableSearchbar)
                {
                    SelectedTaxonList.Clear();
                    UnCollapseAllGroups();

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        AddListsToGroups(SelectedTaxonListSource);
                        _searchText = value;
                    }
                    else
                    {
                        var results = new List<Taxon>();
                        var searchText = value.Split(' ');

                        bool isFirstQuery = true;

                        foreach (string item in searchText)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (isFirstQuery)
                                {
                                    results.AddRange(SelectedTaxonListSource.Where(i => i.SearchString.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0));
                                }
                                else
                                {
                                    results = results.Intersect(SelectedTaxonListSource.Where(i => i.SearchString.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0), new TaxonEqualityComparer()).ToList();
                                }
                                isFirstQuery = false;
                            }
                        }
                        _searchText = value;

                        AddListsToGroups(results.OrderBy(i => i.LocalName).ThenBy(i => i.TaxonName));
                    }
                    SelectedTaxonList.RefreshView();
                    OnPropertyChanged(nameof(SearchText));
                    OnPropertyChanged(nameof(SelectedTaxonList));
                }
            }
        }
        public ObservableList<TaxonListGroup> SelectedTaxonList { get; set; }
        public ObservableList<Taxon> SelectedTaxonListSource { get; set; }
        public ObservableList<Taxon> HiddenTaxonList { get; set; }

        #endregion

        #region Constructor

        public TaxonListViewModel()
        {
            RegroupListCommand = new Command<TaxonListGroup>(arg => RegroupList(arg));
            SelectedTaxonListSource = new ObservableList<Taxon>();
            HiddenTaxonList = new ObservableList<Taxon>();
            SelectedTaxonList = new ObservableList<TaxonListGroup>();
            EnableSearchbar = true;
            SelectedTaxonListSource.AddRange(((App)App.Current).Taxa.Where(t => t.TaxonomyStateName == "sp.").ToList());
            PlaceholderText = $"Suchen in {SelectedTaxonListSource.Count} Arten";
            OnPropertyChanged(nameof(PlaceholderText));
            OnPropertyChanged(nameof(EnableSearchbar));
            OnPropertyChanged(nameof(SearchText));
        }

        #endregion

        #region RegroupList Command

        public Command<TaxonListGroup> RegroupListCommand { get; set; }
        private void RegroupList(TaxonListGroup taxonGroup)
        {
            /* V 3.0.1: don't group list
			if (taxonGroup.Count() == 0)
			{
				foreach (var item in HiddenTaxonList.Where(i => i.OrderName == taxonGroup.OrderName))
				{
					SelectedTaxonListSource.Add(item);
				}
				HiddenTaxonList.RemoveAll(i => i.OrderName == taxonGroup.OrderName);
			}
			else
			{
				foreach (var item in taxonGroup)
				{
					SelectedTaxonListSource.Remove(item);
					HiddenTaxonList.Add(item);
				}
			}
			AddListsToGroups(SelectedTaxonListSource);
			*/
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collapses all groups of the ListView and backing the data up.
        /// </summary>
        public void CollapseAllGroups()
        {
            SearchText = "";
            foreach (var taxonGroup in SelectedTaxonList)
            {
                foreach (var taxon in taxonGroup)
                {
                    SelectedTaxonListSource.Remove(taxon);
                    HiddenTaxonList.Add(taxon);
                }
            }
            AddListsToGroups(SelectedTaxonListSource);
        }

        public void ToggleGroupingByOrderName(string orderName)
        {

            List<Taxon> tempTBDTaxa = new List<Taxon>();

            bool foundInSelected = false;
            foreach (var taxonGroup in SelectedTaxonList)
            {
                foreach (var taxon in taxonGroup)
                {
                    if (taxon.OrderName == orderName)
                    {
                        foundInSelected = true;
                    }
                    HiddenTaxonList.Add(taxon);
                }
            }

            SelectedTaxonListSource.Clear();

            if (!foundInSelected)
            {
                foreach (var taxon in HiddenTaxonList)
                {
                    if (taxon.OrderName == orderName)
                    {
                        SelectedTaxonListSource.Add(taxon);
                        tempTBDTaxa.Add(taxon);
                    }
                }
                foreach (Taxon t in tempTBDTaxa)
                {
                    HiddenTaxonList.Remove(t);
                }
            }

            AddListsToGroups(SelectedTaxonListSource);
        }

        /// <summary>
        /// Uncollapses all groups of the ListView by adding them back.
        /// </summary>
        public void UnCollapseAllGroups()
        {
            foreach (var taxon in HiddenTaxonList)
            {
                SelectedTaxonListSource.Add(taxon);
            }
            HiddenTaxonList.Clear();
            AddListsToGroups(SelectedTaxonListSource);
        }

        /// <summary>
        /// Remove Taxa without Diagnosis from underlying List-source, add lists to groups
        /// </summary>
        public void UpdateSelectedTaxonListSource()
        {
            SelectedTaxonListSource.RemoveAll(i => !i.HasDiagnosis);
            PlaceholderText = $"Suchen in {SelectedTaxonListSource.Count} Arten";
            OnPropertyChanged(nameof(PlaceholderText));
            SelectedTaxonList.Clear();
            AddListsToGroups(SelectedTaxonListSource.OrderBy(i => i.LocalName).ThenBy(i => i.TaxonName));
            SelectedTaxonList.RefreshView();
            OnPropertyChanged(nameof(SelectedTaxonList));
        }

        /// <summary>
        /// Add all Taxa in groups based on their OrderName.
        /// </summary>
        /// <param name="selectedTaxonListSource">The List of currently displayed Taxa to add in groups.</param>
        private void AddListsToGroups(IEnumerable<Taxon> selectedTaxonListSource)
        {
            SelectedTaxonList.Clear();
            foreach (var shownItem in selectedTaxonListSource)
            {
                if (SelectedTaxonList.Any(i => i.OrderName == shownItem.OrderName))
                {
                    SelectedTaxonList.FirstOrDefault(i => i.OrderName == shownItem.OrderName).Add(shownItem);
                }
                else
                {
                    SelectedTaxonList.Add(new TaxonListGroup(shownItem.OrderName, shownItem.OrderLocalName) { shownItem });
                }
            }
            foreach (var hiddenItem in HiddenTaxonList)
            {
                if (!SelectedTaxonList.Any(i => i.OrderName == hiddenItem.OrderName))
                {
                    SelectedTaxonList.Add(new TaxonListGroup(hiddenItem.OrderName, hiddenItem.OrderLocalName));
                }
            }
            SortObservableList(SelectedTaxonList);
        }

        /// <summary>
        /// Sorts the List of TaxonListGroups by their OrderLocalName.
        /// </summary>
        /// <param name="selectedTaxonList">The List of TaxonListGroups to sort.</param>
        private void SortObservableList(ObservableList<TaxonListGroup> selectedTaxonList)
        {
            var tempGroupList = new ObservableList<TaxonListGroup>();
            foreach (var groupItem in selectedTaxonList.OrderBy(i => i.OrderLocalName))
            {
                tempGroupList.Add(groupItem);
            }
            selectedTaxonList.Clear();
            foreach (var groupItem in tempGroupList)
            {
                selectedTaxonList.Add(groupItem);
            }
            selectedTaxonList.RefreshView();
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
