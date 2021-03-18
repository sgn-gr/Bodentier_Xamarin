using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class RegisterDeviceViewModel : RegisterModel, INotifyPropertyChanged
	{
		#region Fields

		private readonly IMobileApi _mobileApi;
		private bool _isBusy;
		private bool _tryingToRegister;
		private bool _isLoggedIn;
		private string _result;
		private static RecordDatabase _database;

		#endregion

		#region Properties

		public string Username { get; set; }
		public string Password { get; set; }
		public string PasswordSecond { get; set; }
		public string Surname { get; set; }
		public string Givenname { get; set; }
		public string TopResult { get; set; }

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				OnPropertyChanged(nameof(IsBusy));
			}
		}

		public bool TryingToRegister
		{

			get { return _tryingToRegister; }
			set
			{
				_tryingToRegister = value;
				OnPropertyChanged(nameof(TryingToRegister));

			}
		}

		public string Result
		{
			get { return _result; }
			set
			{
				_result = value;
				OnPropertyChanged(nameof(Result));
			}
		}
		public bool IsLoggedIn
		{
			get { return _isLoggedIn; }
			set
			{
				_isLoggedIn = value;
				OnPropertyChanged(nameof(IsLoggedIn));

			}
		}
		public static RecordDatabase Database
		{
			get
			{
				if (_database == null)
				{
					_database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3"));
				}
				return _database;
			}
		}

		#endregion

		#region Constructor

		public RegisterDeviceViewModel() : base()
		{
			LoginCommand = new Command(async () => await Login(), () => !IsBusy);
			AddUserCommand = new Command(async () => await AddUser(), () => !IsBusy);
			LogoutCommand = new Command(async () => await Logout());
			_mobileApi = DependencyService.Get<IMobileApi>();

			IsLoggedIn = (Database.GetRegister() != null);
			if (IsLoggedIn)
			{
				TopResult = "Angemeldet als " + Database.GetRegister().Result.UserName;
				OnPropertyChanged(nameof(TopResult));
			}
		}

		#endregion

		#region Login Command

		public Command LoginCommand { get; set; }
		private async Task Login()
		{
			try
			{
				if (Connectivity.NetworkAccess != NetworkAccess.Internet)
					throw new Exception("Zur Anmeldung Internetverbindung herstellen.");

				if (Username == null || Password == null || Username == "" || Password == "")
					throw new Exception("Bitte Eingaben überprüfen.");
				IsBusy = true;
				var result = await _mobileApi.Register(Username, Password);

				if (result == "invalid user")
					throw new Exception("Anmeldung fehlgeschlagen.");

				await Database.Register(result, Username);
				IsBusy = false;
				IsLoggedIn = true;
				Result = "Erfolgreich angemeldet.";
				TopResult = "Angemeldet als " + Username;
				OnPropertyChanged(nameof(TopResult));
			}
			catch (Exception e)
			{
				IsBusy = false;
				Result = e.Message;
			}

		}

		#endregion

		#region AddUser Command

		public Command AddUserCommand { get; set; }
		private async Task AddUser()
		{
			if (TryingToRegister)
			{
				try
				{
					if (Connectivity.NetworkAccess != NetworkAccess.Internet)
						throw new Exception("Zur Registrierung Internetverbindung herstellen.");

					if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(PasswordSecond) || String.IsNullOrEmpty(Surname) || String.IsNullOrEmpty(Givenname))
						throw new Exception("Bitte Eingaben überprüfen.");

					if (PasswordSecond != Password)
						throw new Exception("Passwort stimmt nicht überein.");
					IsBusy = true;
					Result = await _mobileApi.AddNewUser(Givenname, Surname, Username, Password, "comment", "app");
					IsBusy = false;
					//if result positive
					IsLoggedIn = true;
					TopResult = "Registrierung als " + Username + " wurde beantragt und wird zeitnah bearbeitet. Es erfolgt eine Benachrichtigung per E-Mail.";
					OnPropertyChanged(nameof(TopResult));
				}
				catch (Exception e)
				{
					IsBusy = false;
					Result = e.Message;
				};
			}
			else
			{
				Result = "";
			}

		}

		#endregion

		#region Logout Command

		public Command LogoutCommand { get; set; }
		private async Task Logout()
		{
			await Database.Logout();
			IsLoggedIn = false;
			Result = "";
			Username = "";
			Password = "";
			OnPropertyChanged(nameof(Username));
			OnPropertyChanged(nameof(Password));
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
