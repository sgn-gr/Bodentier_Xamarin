using System.Collections.Generic;
using System.Linq;

namespace KBS.App.TaxonFinder.Data
{
    public class TaxonSynonym
	{
		public int TaxonId { get; set; }
		public string Pattern { get; set; }
		public string Text { get; set; }

		private static List<TaxonSynonym> _taxonList;

		public static TaxonSynonym GetByPattern(string pattern)
		{
			_taxonList = ((App)App.Current).TaxonSynonyms;
			return _taxonList.FirstOrDefault(i => i.Pattern.ToLower() == pattern.ToLower());
		}
	}
}
