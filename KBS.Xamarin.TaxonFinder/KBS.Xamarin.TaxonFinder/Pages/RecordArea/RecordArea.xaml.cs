using KBS.App.TaxonFinder.CustomRenderers;
using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using static KBS.App.TaxonFinder.Data.PositionInfo;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RecordArea : ContentPage
	{
		private IGeolocator _location;
		private Distance _distance;
		private double _latitude;
		private double _longitude;
		private bool _autoPosition;

		public RecordArea()
		{
			_distance = new Distance(75000);
			_latitude = 50.9295;
			_longitude = 13.4583;
			_autoPosition = true;
			Initialize();
		}

		public RecordArea(double latitude, double longitude, double distance)
		{
			_distance = new Distance(distance);
			_latitude = latitude;
			_longitude = longitude;
			_autoPosition = false;
			Initialize();
		}
		private RecordAreaViewModel RecordAreaViewModel
		{
			get
			{
				return (RecordAreaViewModel)BindingContext;
			}
		}

		public async void Initialize()
		{
			InitializeComponent();
			MapSpan span = MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(_latitude, _longitude), _distance);
			map.MoveToRegion(span);

			try
			{
				var statusLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
				if (statusLocation != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
					{
						await Application.Current.MainPage.DisplayAlert("Benötige Standort-Berechtigung", "Zum automatischen Feststellen des Fundorts wird die Standort-Berechtigung benötigt.", "Okay");
					}

					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
					statusLocation = results[Permission.Location];
				}
				if (statusLocation == PermissionStatus.Granted)
				{
					_location = DependencyService.Get<IGeolocator>();
				}
				else if (statusLocation != PermissionStatus.Unknown)
				{
					await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigung kann der Standort nicht automatisch festgestellt werden.", "Okay");
				}
			}
			catch
			{
			}
			if (_location != null && _location.IsGeolocationAvailable && _location.IsGeolocationEnabled)
			{
				AutoPositionSwitch.IsToggled = _autoPosition;
				ListenToPosition(_autoPosition);
			}
			else
			{
				AutoPositionSwitch.IsToggled = false;
				AutoPositionSwitch.IsEnabled = false;
			}
		}
		private void ListenToPosition(bool listening)
		{
			if (_location.IsListening)
			{
				try
				{
					_location.StopListening();
				}
				catch { }
			}
			if (listening)
			{
				_location.PositionChanged += LocationService_PositionChanged;
				try
				{
					_location.StartListening(10000, 30);
				}
				catch { }
			}
		}

		private void LocationService_PositionChanged(object sender, PositionEventArgs e)
		{
			_latitude = e.Position.Latitude;
			_longitude = e.Position.Longitude;
			MapSpan span = MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(_latitude, _longitude), new Distance(100));
			map.MoveToRegion(span);
		}

		private void RecordMap_Tap(object sender, TapEventArgs e)
		{
			switch (map.MapPositionOption)
			{
				case PositionOption.Pin:
					_latitude = e.Position.Latitude;
					_longitude = e.Position.Longitude;
					SaveButton.IsEnabled = true;
					UpdatePin();
					break;
				case PositionOption.Line:
					map.RouteCoordinates.Add(new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude));
					SaveButton.IsEnabled = map.RouteCoordinates.Count() > 1;
					break;
				case PositionOption.Area:
					map.ShapeCoordinates.Add(new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude));
					SaveButton.IsEnabled = map.ShapeCoordinates.Count() > 2;
					break;
				default:
					break;
			}
		}
		private void UpdatePin()
		{
			var pin = new Pin()
			{
				Position = new Xamarin.Forms.Maps.Position(_latitude, _longitude),
				Label = "Fundort",
				Type = PinType.SearchResult
			};
			map.Pins.Clear();
			map.Pins.Add(pin);
		}

		private void AutoPositionSwitch_Toggled(object sender, ToggledEventArgs e)
		{
			ListenToPosition(e.Value);
		}
		private void PositionListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var localRecord = ((ListView)sender).SelectedItem as RecordModel;
			switch (localRecord.Position)
			{
				case PositionOption.Pin:
					MessagingCenter.Send(this, "Pin", new Xamarin.Forms.Maps.Position(_latitude, _longitude));
					break;
				case PositionOption.Line:
					var linePositionModelList = Database.GetPositionAsync(localRecord.LocalRecordId).Result;
					var linePositionList = new ObservableCollection<Xamarin.Forms.Maps.Position>();
					foreach (var pos in linePositionModelList)
						linePositionList.Add(new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude));
					MessagingCenter.Send(this, "Line", new ObservableCollection<Xamarin.Forms.Maps.Position>(linePositionList) { });
					break;
				case PositionOption.Area:
					var areaPositionModelList = Database.GetPositionAsync(localRecord.LocalRecordId).Result;
					var areaPositionList = new ObservableCollection<Xamarin.Forms.Maps.Position>();
					foreach (var pos in areaPositionModelList)
						areaPositionList.Add(new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude));
					MessagingCenter.Send(this, "Area", new ObservableCollection<Xamarin.Forms.Maps.Position>(areaPositionList) { });
					break;
				default:
					break;
			}
			Navigation.PopAsync();
		}

		private static RecordDatabase database;
		public static RecordDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3"));
				}
				return database;
			}
		}
		private void PinButton_Clicked(object sender, EventArgs e)
		{
			ButtonStack.IsVisible = false;
			PositionToggle.IsVisible = true;
			map.IsVisible = true;
			map.MapPositionOption = PositionOption.Pin;

		}
		private void LineButton_Clicked(object sender, EventArgs e)
		{
			ButtonStack.IsVisible = false;
			PositionToggle.IsVisible = true;
			map.IsVisible = true;
			map.MapPositionOption = PositionOption.Line;
		}
		private void AreaButton_Clicked(object sender, EventArgs e)
		{
			ButtonStack.IsVisible = false;
			PositionToggle.IsVisible = true;
			map.IsVisible = true;
			map.MapPositionOption = PositionOption.Area;
		}
		private void ListButton_Clicked(object sender, EventArgs e)
		{
			ButtonStack.IsVisible = false;
			PositionListView.IsVisible = true;
		}
		private void CancelButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PopAsync();
		}
		private void SaveButton_Clicked(object sender, EventArgs e)
		{
			switch (map.MapPositionOption)
			{
				case PositionOption.Pin:
					MessagingCenter.Send(this, "Pin", new Xamarin.Forms.Maps.Position(_latitude, _longitude));
					break;
				case PositionOption.Line:
					MessagingCenter.Send(this, "Line", new ObservableCollection<Xamarin.Forms.Maps.Position>(map.RouteCoordinates) { });
					break;
				case PositionOption.Area:
					MessagingCenter.Send(this, "Area", new ObservableCollection<Xamarin.Forms.Maps.Position>(map.ShapeCoordinates) { });
					break;
				default:
					break;
			}
			Navigation.PopAsync();
		}

		private void Help_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage(this));
		}
	}
}
