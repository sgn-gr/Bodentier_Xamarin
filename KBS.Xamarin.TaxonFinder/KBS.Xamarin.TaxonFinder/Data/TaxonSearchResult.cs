namespace KBS.App.TaxonFinder.Data
{
    public class TaxonSearchResult
    {
        public int TaxonId { get; set; }
        public int TaxonTypeId { get; set; }
        public int Weight { get; set; }

        public string TaxonKey
        {
            get { return string.Format("{0}T{1}", TaxonId, TaxonTypeId); }
        }
    }
}
