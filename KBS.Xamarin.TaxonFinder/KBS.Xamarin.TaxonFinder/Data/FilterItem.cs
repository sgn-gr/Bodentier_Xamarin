using KBS.App.TaxonFinder.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Data
{
    public class FilterItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //public event EventHandler SelectionChanged;

        private int _matchingTaxonCount = 0;
        private List<TaxonImage> _images;
        private bool _filterChanged = true;
        private bool _selected = false;
        public string OptionText { get; set; }
        private static readonly Assembly assembly = typeof(Taxon).GetTypeInfo().Assembly;
        private static readonly string assemblyName = assembly.GetName().Name;
        public ImageSource InaString
        {
            get
            {
                return ImageSource.FromResource($"{assemblyName}.Images.General.ina.jpg");
            }
        }

        public List<TaxonImage> Images
        {
            get
            {
                return GetListSourceJsonIS();
            }
            set
            {
                _images = value;
                OnPropertyChanged("Images");
            }
        }
        public int TagId { get; set; }
        public string TagValue { get; set; }
        public string KeyDataType { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int TaxonId { get; set; }
        public int? VisibilityCategoryId { get; set; }
        public List<int> TaxonHits { get; set; }
        public bool CombineSubOptions { get; set; }
        public int? MaximalCombinations { get; set; }
        public List<string> ListSourceJson { get; set; }
        public FilterItem ParentItem { get; set; }
        public List<TaxonFilterItem> TaxonFilterItems { get; set; }
        public int MatchingTaxonCount(List<int> taxonIds)
        {
            if (_filterChanged)
            {

                _matchingTaxonCount = TaxonFilterItem.GetMatchingTaxon(SelectedItems.ToList(), ((App)App.Current).TaxonFilterItems, ValueFilterDictionary, taxonIds).Count;
                _filterChanged = false;
            }

            return _matchingTaxonCount;
        }

        public List<TaxonSearchResult> MatchingTaxonByExclusion(List<int> taxonIds)
        {
            return TaxonFilterItem.GetMatchingTaxon(SelectedItems.ToList(), ((App)App.Current).TaxonFilterItems, ValueFilterDictionary, taxonIds);
        }

        public List<TaxonSearchResult> MatchingTaxonByRelevance(List<int> taxonIds)
        {
            return TaxonFilterItem.GetMatchingTaxonByRelevance(SelectedItems.ToList(), ((App)App.Current).TaxonFilterItems, ValueFilterDictionary, taxonIds);
        }

        public static FilterItem InitFilters()
        {
            try
            {
                var mainItem = new FilterItem();
                //mainItem.OptionText = "Lepidoptera";
                mainItem.OptionText = "Bodentiere";
                //why 7?
                mainItem.TagId = 0;
                mainItem.SelectedItems = new ObservableCollection<FilterItem>();
                mainItem.ValueFilterDictionary = new Dictionary<int, List<int>>();

                //Toplevel
                var tFilterItems = Load.FromFile<TaxonFilterItem>("TaxonFilterItems.json");

                var topLevelFilterItems = tFilterItems.Select(tfi => new { tfi.TagParentId, tfi.TagParentName, tfi.KeyDataType, tfi.VisibilityCategoryId }).Distinct();
                foreach (var tfi in topLevelFilterItems)
                {
                    if (tfi.KeyDataType != "UNKNOWN" && tfi.KeyDataType != "PIC")
                    {
                        mainItem.SubOptions.Add(new FilterItem { TagId = tfi.TagParentId, OptionText = tfi.TagParentName, KeyDataType = tfi.KeyDataType, VisibilityCategoryId = tfi.VisibilityCategoryId, CombineSubOptions = true, ParentItem = mainItem });
                    }
                }

                //Level 2
                //var templist = Newtonsoft.Json.JsonConvert.DeserializeObject(tFilterItems);
                var idValueListValue = tFilterItems.Where(t => t.KeyDataType == "VALUE").Select(tfi => new { tfi.TagId, tfi.ListSourceJson, tfi.VisibilityCategoryId, tfi.TagValue, tfi.KeyDataType, tfi.TagParentId, tfi.MinValue, tfi.MaxValue, tfi.TaxonId, tfi.TaxonHits });
                var idValueList = tFilterItems.Where(t => t.KeyDataType == "VALUELIST").Select(tfi => new { tfi.TagId, tfi.ListSourceJson, tfi.VisibilityCategoryId, tfi.TagValue, tfi.KeyDataType, tfi.TagParentId, tfi.MinValue, tfi.MaxValue, tfi.TaxonId, tfi.TaxonHits }).GroupBy(tfi => tfi.TagId);
                var distinctIdValueList = idValueList.Distinct().ToList();
                var distinctIdValueListValue = idValueListValue.Distinct().ToList();
                foreach (var tfi2 in distinctIdValueList)
                {
                    FilterItem _currentItem = mainItem.FindByTagId(tfi2.First().TagParentId, false);
                    _currentItem.SubOptions.Add(
                        new FilterItem
                        {
                            TagId = tfi2.First().TagId,
                            TagValue = tfi2.First().TagValue,
                            OptionText = tfi2.First().TagValue,
                            KeyDataType = tfi2.First().KeyDataType,
                            MinValue = tfi2.First().MinValue,
                            MaxValue = tfi2.First().MaxValue,
                            ListSourceJson = tfi2.First().ListSourceJson != null ? tfi2.First().ListSourceJson.Take(3).ToList() : null,
                            ParentItem = _currentItem,
                            TaxonId = tfi2.First().TaxonId,
                            TaxonHits = tfi2.First().TaxonHits,
                            VisibilityCategoryId = tfi2.First().VisibilityCategoryId
                        });
                }
                foreach (var tfi2 in distinctIdValueListValue)
                {
                    FilterItem _currentItem = mainItem.FindByTagId(tfi2.TagParentId, false);
                    _currentItem.SubOptions.Add(
                        new FilterItem
                        {
                            TagId = tfi2.TagId,
                            TagValue = tfi2.TagValue,
                            OptionText = tfi2.TagValue,
                            KeyDataType = tfi2.KeyDataType,
                            MinValue = tfi2.MinValue,
                            MaxValue = tfi2.MaxValue,
                            ParentItem = _currentItem,
                            TaxonId = tfi2.TaxonId,
                            TaxonHits = null,
                            VisibilityCategoryId = tfi2.VisibilityCategoryId
                        });
                }

                return mainItem;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public FilterItem BaseItem
        {
            get
            {
                if (ParentItem != null)
                {
                    return ParentItem.BaseItem;
                }
                else
                {
                    return this;
                }
            }
        }
        public ObservableCollection<FilterItem> SelectedItems { get; set; }
        public Dictionary<int, List<int>> ValueFilterDictionary { get; set; }


        public void clearSelectedItems()
        {
            foreach (var item in BaseItem.SelectedItems.ToList())
                item.Selected = false;

            if (ValueFilterDictionary != null)
            {
                ValueFilterDictionary.Clear();
            }

            BaseItem.SelectedItems.Clear();
            BaseItem.OnPropertyChanged(nameof(MatchingTaxonCount));

        }

        public List<TaxonImage> GetListSourceJsonIS()
        {
            if (ListSourceJson != null)
            {
                var fileHelper = DependencyService.Get<IFileHelper>();
                List<TaxonImage> list = new List<TaxonImage>();
                foreach (string lsj in ListSourceJson)
                {
                    try
                    {
                        var lsjImage = ((App)App.Current).TaxonImages.Where(i => i.Title != null).FirstOrDefault(i => i.Title == lsj.Trim());
                        string filePath = fileHelper.GetLocalAppPath($"{lsj.Trim()}.jpg");
                        if (fileHelper.FileExists(filePath) && lsjImage != null)
                        {
                            //ImageSource img = ImageSource.FromFile(fileHelper.GetLocalAppPath($"{lsj.Trim()}.jpg"));
                            list.Add(lsjImage);
                        }

                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    /**
                    else
                    {
                        list.Add(InaString);
                    }
                    **/
                }
                return list;
            }
            return null;
        }

        public bool HasImages
        {
            get
            {
                if (Images != null)
                {
                    if (Images.Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));

                if (!HasSubOptions)
                {
                    if (value)
                    {
                        BaseItem.SelectedItems.Add(this);
                    }
                    else
                    {
                        BaseItem.SelectedItems.Remove(this);
                    }

                    BaseItem.OnPropertyChanged(nameof(MatchingTaxonCount));
                }

                if (ParentItem != null)
                {
                    var result = ParentItem.HasSelectedItems;

                    if (result != ParentItem.Selected)
                    {
                        ParentItem.Selected = result;
                    }
                }
            }
        }
        public List<FilterItem> SubOptions { get; set; }

        public bool HasSelectedItems
        {
            get
            {
                if (SubOptions.Any(i => i.Selected))
                {
                    return true;
                }

                return false;
            }
        }

        public bool HasSubOptions
        {
            get
            {
                return SubOptions != null && SubOptions.Count != 0;
            }
        }

        public override string ToString()
        {
            return "";
        }

        public FilterItem FindByTagId(int tagId, bool isMainItem)
        {
            if (tagId == TagId)
            {
                return this;
            }

            if (HasSubOptions)
            {
                return FindRecursive(this, tagId);
            }

            return null;
        }

        private FilterItem FindRecursive(FilterItem item, int tagId)
        {

            if (item.HasSubOptions)
            {
                if (item.SubOptions.Any(i => i.TagId == tagId))
                {
                    return item.SubOptions.First(i => i.TagId == tagId);
                }

                foreach (var subItem in item.SubOptions.Where(i => i.HasSubOptions))
                {
                    var result = FindRecursive(subItem, tagId);

                    if (result != null)
                    {
                        return result;
                    }

                }
            }

            return null;
        }

        public FilterItem()
        {
            SubOptions = new List<FilterItem>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "MatchingTaxonCount")
            {
                _filterChanged = true;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}