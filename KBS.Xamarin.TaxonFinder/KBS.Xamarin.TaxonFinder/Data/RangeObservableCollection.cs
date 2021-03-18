using System.Collections.Generic;
using System.Collections.Specialized;

namespace KBS.App.TaxonFinder.Data
{
    public class ObservableList<T> : List<T>, INotifyCollectionChanged
	{
		//private bool _suppressNotification = false;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public void RefreshView()
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}
	}

}
