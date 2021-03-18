using System.Collections.Generic;

namespace KBS.App.TaxonFinder.Data
{
    public class TaxonEqualityComparer : EqualityComparer<Taxon>
    {
        public override bool Equals(Taxon x, Taxon y)
        {
            return (x.TaxonId == y.TaxonId && x.TaxonTypeId == y.TaxonTypeId);
        }

        public override int GetHashCode(Taxon obj)
        {
            return new { obj.TaxonId, obj.TaxonTypeId }.GetHashCode();
        }
    }
}
