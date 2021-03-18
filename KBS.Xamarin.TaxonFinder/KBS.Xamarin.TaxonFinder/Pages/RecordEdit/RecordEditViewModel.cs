using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Views;
using Plugin.FilePicker;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static KBS.App.TaxonFinder.Data.PositionInfo;
using Maps = Xamarin.Forms.Maps;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class RecordEditViewModel : RecordModel, INotifyPropertyChanged
    {
        #region Fields

        private static RecordDatabase _database;
        private readonly IMobileApi _mobileApi;
        private bool _unauthorized;
        private bool _isBusy;
        private string _result;
        private RecordModel _selectedRecord;
        private List<MediaFileModel> _mediaToDelete;

        #endregion

        #region Properties

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
        public bool ExistingRecord { get; set; }
        public bool EnableAutoPosition { get; set; }
        public bool SaveAsTemplate { get; set; }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public int MediaHeight { get; set; }

        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        public ObservableCollection<MediaFileModel> SelectedMedia { get; set; }
        public ObservableCollection<Maps.Position> PositionList { get; set; }

        public int CopyRecordId
        {
            set
            {
                _selectedRecord = Database.GetRecordAsync(value).Result;

                if (_selectedRecord.Position != PositionInfo.PositionOption.Pin)
                {
                    List<PositionModel> positionList = Database.GetPositionAsync(value).Result;
                    foreach (var position in positionList)
                    {
                        this.PositionList.Add(new Maps.Position(position.Latitude, position.Longitude));
                    }
                }

                IsEditable = true;
                CreationDate = _selectedRecord.CreationDate;
                Identifier = Guid.NewGuid();
                RecordDate = _selectedRecord.RecordDate;
                HabitatName = _selectedRecord.HabitatName;
                ReportedByName = _selectedRecord.ReportedByName;
                HabitatDescription = _selectedRecord.HabitatDescription;
                ReportedByName = _selectedRecord.ReportedByName;
                Latitude = _selectedRecord.Latitude;
                Longitude = _selectedRecord.Longitude;
                Height = _selectedRecord.Height;
                Accuracy = _selectedRecord.Accuracy;
                Position = _selectedRecord.Position;
                TotalCount = 1;
                MaleCount = _selectedRecord.MaleCount;
                FemaleCount = _selectedRecord.FemaleCount;

                OnPropertyChanged(nameof(CreationDate));
                OnPropertyChanged(nameof(RecordDate));
                OnPropertyChanged(nameof(HabitatName));
                OnPropertyChanged(nameof(HabitatDescription));
                OnPropertyChanged(nameof(ReportedByName));
                OnPropertyChanged(nameof(Latitude));
                OnPropertyChanged(nameof(Longitude));
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Accuracy));
                OnPropertyChanged(nameof(IsEditable));
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        public int SelectedRecordId
        {
            set
            {
                _selectedRecord = Database.GetRecordAsync(value).Result;

                List<MediaFileModel> mediaList = Database.GetMediaAsync(value).Result;
                SelectedMedia.Clear();
                foreach (var media in mediaList)
                    SelectedMedia.Add(media);

                if (_selectedRecord.Position != PositionInfo.PositionOption.Pin)
                {
                    try
                    {
                        PositionList.Clear();
                        var positionList = Database.GetPositionAsync(value).Result;
                        foreach (var position in positionList)
                        {
                            PositionList.Add(new Maps.Position(position.Latitude, position.Longitude));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                LocalRecordId = value;
                IsEditable = _selectedRecord.IsEditable;
                TaxonId = _selectedRecord.TaxonId;
                CreationDate = _selectedRecord.CreationDate;
                Identifier = _selectedRecord.Identifier;
                RecordDate = _selectedRecord.RecordDate;
                HabitatName = _selectedRecord.HabitatName;
                HabitatDescription = _selectedRecord.HabitatDescription;
                Latitude = _selectedRecord.Latitude;
                Longitude = _selectedRecord.Longitude;
                Height = _selectedRecord.Height;
                Accuracy = _selectedRecord.Accuracy;
                Position = _selectedRecord.Position;
                TotalCount = _selectedRecord.TotalCount;
                MaleCount = _selectedRecord.MaleCount;
                FemaleCount = _selectedRecord.FemaleCount;

                OnPropertyChanged(nameof(CreationDate));
                OnPropertyChanged(nameof(RecordDate));
                OnPropertyChanged(nameof(HabitatName));
                OnPropertyChanged(nameof(HabitatDescription));
                OnPropertyChanged(nameof(Latitude));
                OnPropertyChanged(nameof(Longitude));
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Accuracy));
                OnPropertyChanged(nameof(IsEditable));
                OnPropertyChanged(nameof(TotalCount));
            }
        }

        #endregion

        #region NavigateToWeb Command

        public ICommand NavigateToWebCommand { get; set; }
        private async Task NavigateToWeb(Taxon taxon)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new Uri($"https://bodentierhochvier.de/erfassen/funde"));
        }

        #endregion

        #region Constructor
        public RecordEditViewModel() : base()
        {
            TakePhotoCommand = new Command(async () => await TakePhoto());
            SelectPhotoCommand = new Command(async () => await SelectPhoto());
            RemoveMediaCommand = new Command<MediaFileModel>(async arg => await RemoveMedia(arg));
            SaveRecordCommand = new Command(async () => await SaveRecord());
            DeleteRecordCommand = new Command(async () => await DeleteRecord());
            TakeAudioCommand = new Command(async () => await TakeAudio());
            SelectAudioCommand = new Command(async () => await SelectAudio());
            PositionList = new ObservableCollection<Maps.Position>();
            SelectedMedia = new ObservableCollection<MediaFileModel>();
            MediaHeight = 150;
            NavigateToWebCommand = new Command<Taxon>(async arg => await NavigateToWeb(arg));
            _mobileApi = DependencyService.Get<IMobileApi>();
            _mediaToDelete = new List<MediaFileModel>();
        }

        #endregion

        #region TakePhoto Command

        public ICommand TakePhotoCommand { get; set; }
        private async Task TakePhoto()
        {
            try
            {
                var statusCamera = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                var statusStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (statusCamera != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await Application.Current.MainPage.DisplayAlert("Benötige Kamera-Berechtigung", "Zum Aufnehmen von Bildern wird die Kamera-Berechtigung benötigt.", "Okay");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    statusCamera = results[Permission.Camera];
                }

                if (statusStorage != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await Application.Current.MainPage.DisplayAlert("Benötige Speicher-Berechtigung", "Zum Speichern von Bildern und Fundmeldungen wird die Speicher-Berechtigung benötigt.", "Okay");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    statusStorage = results[Permission.Storage];
                }

                if (statusCamera == PermissionStatus.Granted && statusStorage == PermissionStatus.Granted)
                {
                    await CrossMedia.Current.Initialize();

                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "",
                        Name = string.Format("IMG_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss"))
                    });

                    if (file != null)
                    {
                        SelectedMedia.Add(new MediaFileModel(file.Path, LocalRecordId,MediaFileModel.MediaType.Image));
                        SetMediaHeight();
                    }
                }
                else if (statusCamera != PermissionStatus.Unknown && statusStorage != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigungen können keine Bilder aufgenommen und die Fundmeldung nicht gespeichert werden.", "Okay");
                }
                else if (statusCamera != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigung können keine Bilder aufgenommen werden.", "Okay");
                }
                else if (statusStorage != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigung kann die Fundmeldung nicht gespeichert werden.", "Okay");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        #endregion

        #region SelectPhoto Command

        public ICommand SelectPhotoCommand { get; set; }
        private async Task SelectPhoto()
        {
            try
            {
                var statusPhotos = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                var statusStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (statusPhotos != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Photos))
                    {
                        await Application.Current.MainPage.DisplayAlert("Benötige Foto-Berechtigung", "Zum Auswählen von Bildern wird die Foto-Berechtigung benötigt.", "Okay");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Photos });
                    statusPhotos = results[Permission.Photos];
                }

                if (statusStorage != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await Application.Current.MainPage.DisplayAlert("Benötige Speicher-Berechtigung", "Zum Speichern von Bildern und Fundmeldungen wird die Speicher-Berechtigung benötigt.", "Okay");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    statusStorage = results[Permission.Storage];
                }

                if (statusPhotos == PermissionStatus.Granted && statusStorage == PermissionStatus.Granted)
                {
                    await CrossMedia.Current.Initialize();

                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                    });
                    if (file != null)
                    {
                        var fileHelper = DependencyService.Get<IFileHelper>();
                        var copyMedia = fileHelper.CopyFileToApp(file.Path);
                        SelectedMedia.Add(new MediaFileModel(copyMedia.Path, LocalRecordId, MediaFileModel.MediaType.Image));
                        SetMediaHeight();
                    }
                }
                else if (statusPhotos != PermissionStatus.Unknown && statusStorage != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigungen können keine Bilder aufgenommen und die Fundmeldung nicht gespeichert werden.", "Okay");
                }
                else if (statusPhotos != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigung können keine Bilder ausgewählt werden.", "Okay");
                }
                else if (statusStorage != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigung kann die Fundmeldung nicht gespeichert werden.", "Okay");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region RemoveMedia Command

        public ICommand RemoveMediaCommand { get; set; }
        private async Task RemoveMedia(MediaFileModel mediaFile)
        {
            _mediaToDelete.Add(mediaFile);
            var fileHelper = DependencyService.Get<IFileHelper>();
            fileHelper.DeleteFile(mediaFile.Path);
            SelectedMedia.Remove(mediaFile);
            SetMediaHeight();
        }

        #endregion

        #region SaveRecord Command

        public ICommand SaveRecordCommand { get; set; }
        private async Task SaveRecord()
        {
            if (!String.IsNullOrEmpty(HabitatName) && TotalCount > 0 &&
            (Position == PositionOption.Pin && Latitude != 0 && Longitude != 0) ||
            (Position != PositionOption.Pin && PositionList.Count != 0)
            && !String.IsNullOrEmpty(ReportedByName))
            {
                foreach (var media in _mediaToDelete)
                {
                    if (media != null)
                        await Database.DeleteMedia(media);
                }
                var recordModel = new RecordModel
                {
                    TaxonId = TaxonId,
                    CreationDate = DateTime.Now,
                    Identifier = Identifier == Guid.Empty ? Guid.NewGuid() : Identifier,
                    RecordDate = RecordDate,
                    HabitatName = HabitatName,
                    HabitatDescription = HabitatDescription,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Height = Height,
                    Accuracy = Accuracy,
                    Position = Position,
                    TotalCount = TotalCount,
                    MaleCount = MaleCount,
                    FemaleCount = FemaleCount,
                    IsEditable = IsEditable,
                    ReportedByName = ReportedByName,
                };

                if (ExistingRecord)
                {
                    recordModel.LocalRecordId = this.LocalRecordId;
                    await Database.UpdateRecord(recordModel);
                    var positionsToDelete = await Database.GetPositionAsync(recordModel.LocalRecordId);
                    foreach (var position in positionsToDelete)
                    {
                        await Database.DeletePosition(position);
                    }
                }
                else
                {
                    await Database.SaveRecord(recordModel);
                }

                if (recordModel.Position != PositionInfo.PositionOption.Pin)
                {
                    foreach (var pos in PositionList)
                    {
                        await Database.SavePosition(
                        new PositionModel()
                        {
                            Type = recordModel.Position,
                            LocalRecordId = recordModel.LocalRecordId,
                            Latitude = pos.Latitude,
                            Longitude = pos.Longitude
                        });
                    }
                }

                foreach (var media in SelectedMedia)
                {
                    if ((await Database.GetMediaAsync(recordModel.LocalRecordId)).All(p => p.Path != media.Path))
                    {
                        media.LocalRecordId = recordModel.LocalRecordId;
                        await Database.SaveMedia(media);
                    }
                }
                string result;
                if (SaveAsTemplate)
                {
                    if (ExistingRecord)
                    {
                        result = "Änderungen gespeichert";
                    }
                    else
                    {
                        result = "Fundmeldung angelegt";
                    }
                }
                else
                {
                    try
                    {
                        SelectedRecordId = recordModel.LocalRecordId;
                        if (Database.GetRegister() != null)
                        {
                            AdviceJsonItem[] items = { SaveTaxa() };
                            IsBusy = true;
                            result = await _mobileApi.SaveAdvicesByDevice(items);
                            IsBusy = false;

                            if (result == "true")
                            {
                                Database.SetSynced(_selectedRecord.LocalRecordId);
                                IsEditable = false;
                                result = "Fundmeldung synchronisiert";
                            }
                        }
                        else
                        {
                            IsBusy = false;
                            throw new System.UnauthorizedAccessException("Unautorized");
                        }

                    }
                    catch (Exception e)
                    {
                        if (e is UnauthorizedAccessException)
                        {
                            _unauthorized = true;
                            await App.Current.MainPage.Navigation.PushAsync(new RecordList(_unauthorized));
                        }
                        result = e.Message;
                    }

                }
                if (!_unauthorized)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new RecordList(result));
                }
                var existingPages = App.Current.MainPage.Navigation.NavigationStack.ToList();
                for (int i = 1; i < existingPages.Count - 1; i++)
                {
                    App.Current.MainPage.Navigation.RemovePage(existingPages[i]);
                };
            }

        }

        #endregion

        #region DeleteRecord Command

        public ICommand DeleteRecordCommand { get; set; }
        private async Task DeleteRecord()
        {
            List<MediaFileModel> mediaList = await Database.GetMediaAsync(this.LocalRecordId);
            foreach (var media in mediaList)
            {
                if (media != null)
                    await Database.DeleteMedia(media);
            }
            foreach (var media in SelectedMedia)
            {
                DependencyService.Get<IFileHelper>().DeleteFile(media.Path);
            }
            List<PositionModel> positionList = await Database.GetPositionAsync(this.LocalRecordId);
            foreach (var position in positionList)
            {
                await Database.DeletePosition(position);
            }

            if (ExistingRecord)
            {
                await Database.DeleteRecord(await Database.GetRecordAsync(this.LocalRecordId));
                await App.Current.MainPage.Navigation.PushAsync(new RecordList("Fundmeldung gelöscht"));
                var existingPages = App.Current.MainPage.Navigation.NavigationStack.ToList();
                for (int i = 1; i < existingPages.Count - 1; i++)
                {
                    App.Current.MainPage.Navigation.RemovePage(existingPages[i]);
                };
            }
            else
            {
                await App.Current.MainPage.Navigation.PopAsync();
            }

        }

        #endregion

        #region TakeAudio Command

        public ICommand TakeAudioCommand { get; set; }

        private async Task TakeAudio()
        {
            await App.Current.MainPage.Navigation.PushAsync(new TakeAudio());
        }

        #endregion

        #region SelectAudio Command

        public ICommand SelectAudioCommand { get; set; }

        private async Task SelectAudio()
        {
            var statusStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (statusStorage != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                {
                    await Application.Current.MainPage.DisplayAlert("Benötige Speicher-Berechtigung", "Zum Speichern von Fundmeldungen, Bildern und Audiodateien wird die Speicher-Berechtigung benötigt.", "Okay");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                statusStorage = results[Permission.Storage];
            }

            if (statusStorage == PermissionStatus.Granted)
            {
                var allowedType = new string[1];
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        allowedType[0] = "audio/*";
                        break;
                    case Device.iOS:
                        allowedType[0] = "UTType.Audio";
                        break;
                    default:
                        break;
                }
                var fileData = await CrossFilePicker.Current.PickFile(allowedType);
                if (fileData == null)
                    return; // user canceled file picking
                var fileHelper = DependencyService.Get<IFileHelper>();
                var file = fileHelper.CopyFileToApp(fileData.FilePath);
                SelectedMedia.Add(new MediaFileModel(file.Path, LocalRecordId,MediaFileModel.MediaType.Audio));
                SetMediaHeight();
            }
        }
        //audio/*
        #endregion

        #region Methods

        /// <summary>
        /// Saves the current RecordModel as a AdviceJsonItem.
        /// </summary>
        /// <returns>Returns an AdviceJsonItem of the current RecordModel.</returns>
        public AdviceJsonItem SaveTaxa()
        {
            string baseString;
            List<AdviceImageJsonItem> baseList = new List<AdviceImageJsonItem>();
            List<AdviceImageJsonItem> audioList = new List<AdviceImageJsonItem>();
            var fileHelper = DependencyService.Get<IFileHelper>();
            foreach (var media in SelectedMedia)
            {
                baseString = fileHelper.GetBase64FromImagePath(media.Path);
                baseList.Add(new AdviceImageJsonItem(baseString, media.Path));
            }
            var tempTaxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == (int)(_selectedRecord.TaxonId));
            var taxonName = (tempTaxon != null) ? tempTaxon.TaxonName : "";
            AdviceJsonItem adviceJsonItem = new AdviceJsonItem
            {
                AdviceId = _selectedRecord.LocalRecordId,
                Identifier = _selectedRecord.Identifier,
                TaxonId = _selectedRecord.TaxonId,
                TaxonFullName = taxonName,
                AdviceDate = _selectedRecord.RecordDate,
                AdviceCount = _selectedRecord.TotalCount,
                AdviceCity = _selectedRecord.HabitatName,
                MaleCount = _selectedRecord.MaleCount,
                FemaleCount = _selectedRecord.FemaleCount,
                StateEgg = false,
                StateLarva = false,
                StateImago = true,
                StateNymph = false,
                StatePupa = false,
                StateDead = false,
                Comment = _selectedRecord.HabitatDescription,
                ReportedByName = _selectedRecord.ReportedByName,
                ImageCopyright = "",
                ImageLegend = "",
                UploadCode = 0,
                Lat = _selectedRecord.Position == PositionOption.Pin ? (decimal)_selectedRecord.Latitude : (decimal)RecordEdit.GetCenterOfPositions(PositionList).Latitude,
                Lon = _selectedRecord.Position == PositionOption.Pin ? (decimal)_selectedRecord.Longitude : (decimal)RecordEdit.GetCenterOfPositions(PositionList).Longitude,
                //AreaWkt = _selectedRecord.Position != PositionOption.Pin ? ConvertPositionListToWkt(_selectedRecord.Position, PositionList) : "",
                Zoom = _selectedRecord.Height != null ? (int)_selectedRecord.Height : 1,
                AccuracyType = 1,
                DeviceId = DependencyService.Get<IDeviceId>().GetDeviceId(),
                DeviceHash = Database.GetRegister().Result.DeviceHash,
                LocalityTemplateId = 0,
                Images = baseList.ToArray()
            };
            return adviceJsonItem;
        }

        /// <summary>
        /// Converts a List of Positions to a string of the given WKT-type.
        /// </summary>
        /// <param name="positionOption">Type to convert the Position-List to.</param>
        /// <param name="positionList">List of Positions to Convert.</param>
        /// <returns>Returns a WKT-string based on the List of Positions and the given type.</returns>
        private static string ConvertPositionListToWkt(PositionOption positionOption, ObservableCollection<Maps.Position> positionList)
        {
            var areaWkt = (positionOption == PositionOption.Area) ? "POLYGON ((" : "LINESTRING (";
            foreach (var position in positionList)
            {
                areaWkt += string.Format(CultureInfo.InvariantCulture, "{0} {1},", position.Longitude, position.Latitude);
            }
            return areaWkt.TrimEnd(',') + ((positionOption == PositionOption.Area) ? "))" : ")");
        }
        /// <summary>
        /// Sets the height of the media section based on the number of files.
        /// </summary>
        private void SetMediaHeight()
        {
            MediaHeight = SelectedMedia.Count * 45;
            if (MediaHeight < 150) { MediaHeight = 150; }
            OnPropertyChanged(nameof(MediaHeight));
        }

        public async void AddAudio()
        {
            if (((App)App.Current).AudioRecording != null)
            {
                var recording = new MediaFileModel(((App)App.Current).AudioRecording.Path, LocalRecordId, MediaFileModel.MediaType.Audio);
                SelectedMedia.Add(recording);
                OnPropertyChanged(nameof(SelectedMedia));
                ((App)App.Current).AudioRecording = null;
            }
        }

        #endregion
    }
}