using System;
using System.Collections.Generic;
using System.Text;

namespace KBS.App.TaxonFinder.Data
{
    class TaxonTagFilterFilterGroup
    {
        public string GroupName { get; set; }
        public List<int> DKIds { get; set; }
        public List<int> DKGIds { get; set; }
        public List<int> TaxonIds { get; set; }
    }
}
