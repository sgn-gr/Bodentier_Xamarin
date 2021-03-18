using KBS.App.TaxonFinder.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class FeedbackViewModel : INotifyPropertyChanged
	{
		#region Fields

		private readonly IMobileApi _mobileApi;
		private bool _isSent;
		private bool _isClicked;
		private bool _isBusy;
		private string _result;

		#endregion

		#region Properties

		public bool IsSent
		{
			get { return _isSent; }
			set
			{
				_isSent = value;
				OnPropertyChanged(nameof(IsSent));
			}
		}
		public bool IsClicked
		{
			get { return _isClicked; }
			set
			{
				_isClicked = value;
				OnPropertyChanged(nameof(IsClicked));
			}
		}
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				OnPropertyChanged(nameof(IsBusy));
			}
		}
		public string MailText { get; set; }
		public string MailAdress { get; set; }
		public string Result
		{
			get { return _result; }
			set
			{
				_result = value;
				OnPropertyChanged(nameof(Result));
			}
		}

		#endregion

		#region Constructor

		public FeedbackViewModel()
		{
			SendFeedbackCommand = new Command(async () => await SendFeedback(), () => !IsBusy);
			_mobileApi = DependencyService.Get<IMobileApi>();
		}

		#endregion

		#region SendFeedback Command
		public Command SendFeedbackCommand { get; set; }
		private async Task SendFeedback()
		{
			Result = "";
			if (!String.IsNullOrWhiteSpace(MailText))
			{
				try
				{
					IsClicked = false;
					if (Connectivity.NetworkAccess != NetworkAccess.Internet)
						throw new Exception("Zum Feedback senden Internetverbindung herstellen.");

					IsBusy = true;

					var result = await _mobileApi.SendFeedback(MailText, MailAdress);
					if (result == "success")
					{
						IsClicked = true;
						IsBusy = false;
						IsSent = true;
						await Application.Current.MainPage.DisplayAlert("Nachricht gesendet", "Dein Feedback wurde gesendet.", "Okay");
						await Application.Current.MainPage.Navigation.PopAsync();
					}
					else
					{
						throw new Exception("Ein Fehler ist aufgetreten.");
					}
				}
				catch (Exception e)
				{
					IsBusy = false;
					IsClicked = true;
					Result = e.Message;
				}
			}
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}