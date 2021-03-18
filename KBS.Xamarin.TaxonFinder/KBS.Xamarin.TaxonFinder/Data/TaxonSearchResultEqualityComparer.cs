using System.Collections.Generic;

namespace KBS.App.TaxonFinder.Data
{
    public class TaxonSearchResultEqualityComparer: EqualityComparer<TaxonSearchResult>
    {
        public override bool Equals(TaxonSearchResult x, TaxonSearchResult y)
        {
            return (x.TaxonId == y.TaxonId && x.TaxonTypeId == y.TaxonTypeId);
        }

        public override int GetHashCode(TaxonSearchResult obj)
        {
            return new { obj.TaxonId, obj.TaxonTypeId }.GetHashCode();
        }
    }
}
