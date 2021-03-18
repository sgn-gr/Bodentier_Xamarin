using System.ComponentModel;

namespace KBS.App.TaxonFinder.Data
{
    public class TaxonListGroup : ObservableList<Taxon>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool _isExpanded = false;
        public string OrderName { get; set; }
        public string OrderLabel
        {
            get
            {
                return $"{orderLocalName} ({Count})";
            }
        }
        private string orderLocalName;

        public string OrderLocalName
        {
            get
            {
                return (orderLocalName != "") ? orderLocalName : OrderName;
            }
            set
            {
                orderLocalName = value;
            }
        }
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
                OnPropertyChanged(nameof(ExpandedIcon));
            }
        }
        public string ExpandedIcon
        {
            get
            {
                if (IsExpanded)
                {
                    OnPropertyChanged(nameof(ExpandedIcon));
                    return "\u25B2;";
                }
                return "\u25BC";
            }
        }

        public TaxonListGroup(string orderName, string orderLocalName)
        {
            OrderName = orderName;
            OrderLocalName = orderLocalName;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
