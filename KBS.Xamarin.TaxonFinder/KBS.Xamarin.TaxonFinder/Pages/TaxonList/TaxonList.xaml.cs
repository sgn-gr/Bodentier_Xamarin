using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TaxonList : ContentPage
	{
		public TaxonList()
		{
			InitializeComponent();
			CurrentTaxonList.CollapseAllGroups();
		}

		private TaxonListViewModel CurrentTaxonList
		{
			get
			{
				return (TaxonListViewModel)BindingContext;
			}
		}



		public TaxonList(bool newRecord)
		{
			InitializeComponent();
			TaxonListViewModel.NewRecord = newRecord;
			CurrentTaxonList.CollapseAllGroups();
		}

		public TaxonList(int localRecordId)
		{
			InitializeComponent();
			TaxonListViewModel.LocalRecordIdCopy = localRecordId;
			TaxonListViewModel.NewRecord = true;
			CurrentTaxonList.CollapseAllGroups();
		}

		private TaxonListViewModel TaxonListViewModel
		{
			get
			{
				return (TaxonListViewModel)BindingContext;
			}
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var taxon = (Taxon)e.Item;
			if (TaxonListViewModel.LocalRecordIdCopy.HasValue)
			{
				Navigation.PushAsync(new RecordEdit((int)(TaxonListViewModel).LocalRecordIdCopy, taxon.TaxonId, true));
			}
			else if (TaxonListViewModel.NewRecord)
			{
				Navigation.PushAsync(new RecordEdit(taxon.TaxonId));
			}
			else
			{
				Navigation.PushAsync(new TaxonInfo(taxon.TaxonId));
			}
		}

		private void ListView_HeaderTapped(object sender, EventArgs e)
		{
			string orderName = ((sender as Grid).Children[0] as Label).Text;
			CurrentTaxonList.ToggleGroupingByOrderName(orderName);
		}

		private void Switchgroupingbutton_collapse_clicked(object sender, EventArgs e)
		{
			CurrentTaxonList.CollapseAllGroups();
		}

		private void Switchgroupingbutton_expand_clicked(object sender, EventArgs e)
		{
			CurrentTaxonList.UnCollapseAllGroups();
		}

		private void ListView_HeaderTapped(object sender, ItemTappedEventArgs e)
		{
			string orderName = (sender as Button).CommandParameter.ToString();
			CurrentTaxonList.ToggleGroupingByOrderName(orderName);
		}


		private void Help_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage(this));
		}
	}
}