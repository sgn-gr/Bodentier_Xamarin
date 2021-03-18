using System;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.CustomRenderers
{
    public class TaxonLinkClickedEventArgs : EventArgs
    {
        public int TaxonId { get; set; }
    }
    public class HtmlLabel:Label
    {
        public event EventHandler<TaxonLinkClickedEventArgs> NavigateToTaxon;

        public virtual void OnNavigate(object sender, TaxonLinkClickedEventArgs e)
        {
            if (NavigateToTaxon != null)
            {
                NavigateToTaxon(this, e);
            }
        }
    }
}
