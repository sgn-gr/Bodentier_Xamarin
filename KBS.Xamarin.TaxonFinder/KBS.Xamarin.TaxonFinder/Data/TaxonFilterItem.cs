using System.Collections.Generic;
using System.Linq;

namespace KBS.App.TaxonFinder.Data
{
    public class TaxonFilterItem
    {
        public int TagId { get; set; }
        public int TaxonId { get; set; }
        public int TaxonTypeId { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string TagValue { get; set; }
        public int TagParentId { get; set; }
        public string TagParentName { get; set; }
        public List<int> TaxonHits { get; set; }
        public string KeyDataType { get; set; }
        public int? VisibilityCategoryId { get; set; }
        public List<string> ListSourceJson { get; set; }
        public bool IsValueType { get { return !string.IsNullOrEmpty(TagValue); } }

        public string TaxonKey
        {
            get
            {
                return string.Format("{0}T{1}", TaxonId, TaxonTypeId);
            }
        }

        public static List<TaxonSearchResult> GetMatchingTaxon(List<FilterItem> items, List<TaxonFilterItem> taxonList, Dictionary<int, List<int>> FilterDictionary, List<int> groupedTaxonIds)
        {
            var parentIds = items.Select(i => i.ParentItem.TagId).Distinct();
            var result = new List<TaxonSearchResult>();

            bool firstQuery = true;
            if(items.Count > 0)
            {
                foreach (int parentId in parentIds)
                {
                    var parentItem = items.Select(i => i.ParentItem).First(i => i.TagId == parentId);

                    if (parentItem.CombineSubOptions)
                    {
                        var localList = new List<TaxonSearchResult>(); //= firstQuery ? taxonList : result;

                        foreach (var item in items.Where(i => i.ParentItem.TagId == parentId && i.Selected))
                        {
                            localList.AddRange(taxonList.Where(i => i.TagId == item.TagId && i.TagValue == item.TagValue)
                                .Select(i => new TaxonSearchResult() { TaxonId = i.TaxonId, TaxonTypeId = i.TaxonTypeId }).Distinct(new TaxonSearchResultEqualityComparer()).ToList());
                        }

                        result = firstQuery ? localList : localList.Intersect(result, new TaxonSearchResultEqualityComparer()).ToList();
                        firstQuery = false;
                    }
                    else
                    {

                        foreach (var item in items.Where(i => i.ParentItem.TagId == parentId && i.Selected))
                        {
                            if (firstQuery)
                            {
                                result.AddRange(taxonList.Where(i => i.TagId == item.TagId && (!i.IsValueType || i.TagValue == item.TagValue))
                                    .Select(i => new TaxonSearchResult() { TaxonId = i.TaxonId, TaxonTypeId = i.TaxonTypeId }).Distinct(new TaxonSearchResultEqualityComparer()).ToList());
                            }
                            else
                            {
                                result = result.Intersect(taxonList.Where(i => i.TagId == item.TagId && (!i.IsValueType || i.TagValue == item.TagValue))
                                    .Select(i => new TaxonSearchResult() { TaxonId = i.TaxonId, TaxonTypeId = i.TaxonTypeId }).Distinct(new TaxonSearchResultEqualityComparer()), new TaxonSearchResultEqualityComparer()).ToList();
                            }

                            firstQuery = false;
                        }
                    }
                }
            } else
            {
                if(groupedTaxonIds != null)
                {
                    result = ((App)App.Current).Taxa.Where(t => groupedTaxonIds.Contains(t.TaxonId)).Select(i => new TaxonSearchResult() { TaxonId = i.TaxonId, TaxonTypeId = i.TaxonTypeId }).ToList();

                } else
                {
                    result = ((App)App.Current).Taxa.Select(i => new TaxonSearchResult() { TaxonId = i.TaxonId, TaxonTypeId = i.TaxonTypeId }).ToList();
                }
            }


            List<int> taxIds = result.Select(t => t.TaxonId).ToList();

            if (FilterDictionary != null)
            {
                if(FilterDictionary.Count > 0)
                {
                    foreach (List<int> tIds in FilterDictionary.Values)
                    {
                        taxIds.Intersect(tIds);
                    }
                }
            }

            if(groupedTaxonIds != null)
            {
                taxIds = taxIds.Intersect(groupedTaxonIds).ToList();
            }

            //
            return result.Where(t => taxIds.Contains(t.TaxonId)).ToList();
        }


        public static List<TaxonSearchResult> GetMatchingTaxonByRelevance(List<FilterItem> items, List<TaxonFilterItem> taxonList, Dictionary<int, List<int>> FilterDictionary, List<int> groupedTaxonIds)
        {
            var parentIds = items.Select(i => i.ParentItem.TagId).Distinct();
            var result = taxonList.Select(i => new TaxonSearchResult() { TaxonId = i.TaxonId, TaxonTypeId = i.TaxonTypeId, Weight = 0 }).Distinct(new TaxonSearchResultEqualityComparer()).ToList();

            foreach (int parentId in parentIds)
            {
                var parentItem = items.Select(i => i.ParentItem).First(i => i.TagId == parentId);


                foreach (var item in items.Where(i => i.ParentItem.TagId == parentId && i.Selected))
                {
                    foreach (var si in taxonList.Where(i => i.TagId == item.TagId && (!i.IsValueType || i.TagValue == item.TagValue)).ToList())
                    {
                        result.Single(i => i.TaxonId == si.TaxonId && i.TaxonTypeId == si.TaxonTypeId).Weight += 1;
                    }
                }

            }

            List<int> taxIds = result.Select(t => t.TaxonId).ToList();

            if (FilterDictionary != null)
            {

                foreach (List<int> taxonIds in FilterDictionary.Values)
                {
                    taxIds = taxIds.Intersect(taxonIds).ToList();
                }
            }

            if(groupedTaxonIds != null)
            {
                taxIds = taxIds.Intersect(groupedTaxonIds).ToList();
            }

            //
            return result.Where(t => taxIds.Contains(t.TaxonId)).ToList();

        }
    }
}
