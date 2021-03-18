using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RecordList : ContentPage
	{
		public RecordList()
		{
			InitializeComponent();
			CheckIfEmpty();
		}
		public RecordList(bool unauthorized)
		{
			InitializeComponent();
			if (unauthorized)
			{
				RecordListViewModel.Unauthorized = unauthorized;
				RecordListViewModel.SyncButtonText = "Anmeldung öffnen";
				RecordListViewModel.Result = "Zur Synchronisation von Fundmeldungen anmelden.";
			}
			CheckIfEmpty();
		}
		public RecordList(string result)
		{
			InitializeComponent();
			RecordListViewModel.Result = result;
			CheckIfEmpty();

		}
		private RecordListViewModel RecordListViewModel
		{
			get
			{
				return (RecordListViewModel)BindingContext;
			}
		}
		private void CheckIfEmpty()
		{
			var recordList = RecordListViewModel.Database.GetRecordsAsync().Result.OrderBy(i => i.RecordDate).ToList();

			if (recordList.Any())
			{
				FromDate.Date = recordList.First().RecordDate;
				ToDate.Date = recordList.Last().RecordDate;
				if (!recordList.Any(i => i.IsEditable))
				{
					//CurrentRecordList.NewRecord = true;
					//CurrentRecordList.SyncButtonText = "Neue Fundmeldung anlegen";
					RecordListViewModel.SyncButtonText = "Fundmeldungen synchronisieren";
					RecordListViewModel.NewRecord = false;
					//
					SyncButton.IsVisible = true;
					//if (ResultLabel.Text == null)
					//{
					//	CurrentRecordList.Result = "Alle Fundmeldungen wurden synchronisiert.";
					//}
				}
			}
			else
			{
				EmptyStack.IsVisible = true;
				RecordListViewModel.NewRecord = true;
				RecordListViewModel.SyncButtonText = "Fundmeldung anlegen";
				if (ResultLabel.Text == null)
				{
					RecordListViewModel.Result = "Du hast noch keine Funde zum Synchronisieren.";
				}
			}
		}

		private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			RecordListViewModel.Result = "";
			var record = (RecordModel)e.Item;
			Navigation.PushAsync(new RecordEdit(record.LocalRecordId, record.TaxonId));
		}

		private void SyncButton_Clicked(object sender, EventArgs e)
		{
			CheckIfEmpty();
		}

		private void Help_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage(this));
		}
	}
}