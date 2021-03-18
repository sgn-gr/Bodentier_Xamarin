using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Views
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			VersionLabel.Text = "Version " + AppInfo.VersionString;
		}

		//Artsteckbriefe
		private void OrderListButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new TaxonList(false));
		}

		//Art bestimmen
		private void OrderSelectionButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new OrderSelection());
		}

		//Fund melden
		private void AddRecordButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new TaxonList(true));
		}

		//Fundliste
		private void RecordListButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new RecordList());
		}

		//Aktualisierung
		private void UpdateButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new UpdateData());
		}

		//Anmeldung
		private void RegisterButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new RegisterDevice());
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			try
			{
				bool registered = await ((MainPageViewModel)BindingContext).GetHint();
				RegisterButton.Text = registered ? "Abmeldung" : "Anmeldung";
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private void Feedback_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new Feedback());
		}

		private void Help_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage(this));
		}
	}
}
