using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{

    public class UpdateDataViewModel : INotifyPropertyChanged
    {
        #region Fields

        private const string ServiceUrl = "https://corenet.kbs-leipzig.de/json/bodentier_app_json/";
        private const string VersionsFile = "Versions.json";
        private static string _oldVersionDate;
        private static JObject _oldVersionJson;
        private readonly IFileHelper _fileHelper;
        private bool _isBusy;
        private string _dataStatus;
        private string _result;

        #endregion

        #region Properties

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public string DataStatus
        {
            get { return _dataStatus; }
            set
            {
                _dataStatus = value;
                OnPropertyChanged(nameof(DataStatus));
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

        #endregion

        #region Constructor

        public UpdateDataViewModel()
        {
            LoadDataCommand = new Command(async () => await LoadData(false), () => !IsBusy);
            LoadDataCommand_lores = new Command(async () => await LoadData(true), () => !IsBusy);

            _fileHelper = DependencyService.Get<IFileHelper>();
            //check version file, display state
            if (_fileHelper.FileExists(VersionsFile))
            {
                var oldVersions = _fileHelper.GetAllText(VersionsFile);
                try
                {
                    _oldVersionJson = JObject.Parse(oldVersions);
                    _oldVersionDate = _oldVersionJson["Versions.json"].Value<string>();
                    var imageDate = Preferences.Get("imageDate", "");
                    if (imageDate != "")
                    {
                        DataStatus = $"Daten vom Stand {DateTime.Parse(_oldVersionDate).ToString("dd.MM.yyyy")}.";
                    }
                    else
                    {
                        DataStatus = "Daten bisher nur unvollständig aktualisiert.";
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                DataStatus = "Daten bisher noch nicht aktualisiert.";
            }
        }

        #endregion

        #region LoadData Command

        public Command LoadDataCommand { get; set; }
        public Command LoadDataCommand_lores { get; set; }
        private async Task LoadData(bool lores = false)
        {
            Result = "";

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Benötige Internetverbindung", "Zum Laden der Dateien wird eine Internetverbindung benötigt.", "Okay");
            }
            else if (await Application.Current.MainPage.DisplayAlert("Große Dateimengen herunterladen?", "Beim Fortfahren werden mehrere Megabyte (bis zu 500MB) Bilddaten heruntergeladen. Am besten sollte dies mit WLAN heruntergeladen werden.", "Fortfahren", "Abbrechen"))
            {
                IsBusy = true;
                await LoadJson();
                try
                {
                    Dictionary<string, string> filesList = new Dictionary<string, string>();
                    //var count = ((App)App.Current).TaxonImages.Count;
                    //count += tFilterItems.Count;
                    var tFilterItems = Load.FromFile<FilterItem>("TaxonFilterItems.json");
                    var i = 0;

                    foreach (var image in ((App)App.Current).TaxonImages)
                    {
                        string[] fileNames;
                        if (!lores)
                        {
                            fileNames = new string[] { $"{image.Title}-300x300.jpg", $"{image.Title}.jpg" };
                        }
                        else
                        {
                            fileNames = new string[] { $"{image.Title}-113x150.jpg", $"{image.Title}.jpg" };

                        }
                        if (!filesList.ContainsKey(fileNames[0]))
                        {
                            filesList.Add(fileNames[0], fileNames[1]);
                        }
                    }

                    foreach (FilterItem tFilter in tFilterItems)
                    {
                        string[] fileNames;
                        if (tFilter.ListSourceJson != null)
                        {
                            foreach (string img in tFilter.ListSourceJson)
                            {
                                if (img != null)
                                {
                                    if (!lores)
                                    {
                                        fileNames = new string[] { $"{img.Trim()}-300x300.jpg", $"{img.Trim()}.jpg" };
                                    }
                                    else
                                    {
                                        fileNames = new string[] { $"{img.Trim()}-113x150.jpg", $"{img.Trim()}.jpg" };
                                    }
                                    if (!filesList.ContainsKey(fileNames[0]))
                                    {
                                        filesList.Add(fileNames[0], fileNames[1]);
                                    }

                                }
                            }
                        }
                    }
                    //filesList = (List<string[]>)filesList.DistinctBy(x => x.).ToList();
                    var count = filesList.Count;

                    foreach (var fileName in filesList)
                    {
                        i++;
                        if (!_fileHelper.FileExists(fileName.Key))
                        {
                            try
                            {
                                var downloadFile = _fileHelper.DownloadFileAsync($"https://www.bodentierhochvier.de/wp-content/uploads/{fileName.Key}");
                                if (downloadFile != null)
                                {
                                    await _fileHelper.CopyFileToLocalAsync(downloadFile, fileName.Value);
                                }
                                else
                                {
                                    downloadFile = _fileHelper.DownloadFileAsync($"https://www.bodentierhochvier.de/wp-content/uploads/{fileName.Value}");
                                    if (downloadFile != null)
                                    {
                                        await _fileHelper.CopyFileToLocalAsync(downloadFile, fileName.Value);
                                    }
                                }
                                string percent = ((double)(i * 1000 / count) / 10).ToString("0.0");
                                DataStatus = $"Lade Bilder {i} von {count} ({percent} %)";

                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }

                    var versions = _fileHelper.GetAllText(VersionsFile);
                    var versionsJson = JObject.Parse(versions);
                    var versionDate = versionsJson["TaxonImages.json"].Value<string>();
                    var imageDate = versionsJson["Versions.json"].Value<string>();
                    Preferences.Set("imageDate", imageDate);
                    DataStatus = $"Daten vom Stand {DateTime.Parse(versionDate).ToString("dd.MM.yyyy")}.";
                    Result = "Daten erfolgreich aktualisiert.";
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    Result = "Beim Laden der Bilder ist ein Problem aufgetreten.";
                }
                finally
                {
                    IsBusy = false;
                }

            }
        }


        private async Task LoadJson()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Benötige Internetverbindung", "Zum Laden der Dateien wird eine Internetverbindung benötigt.", "Okay");
            }
            else
            {
                try
                {
                    DataStatus = "Lade Artinformationen …";
                    //load version file from internet
                    //json / bodentier_app_json
                    var versions = await _fileHelper.DownloadFileAsync(ServiceUrl + VersionsFile);

                    //get last time updated
                    var versionJsonString = System.Text.Encoding.UTF8.GetString(versions, 0, versions.Length);
                    JObject versionJson = JObject.Parse(versionJsonString);
                    var versionDate = versionJson["Versions.json"].Value<string>();
                    if (_fileHelper.FileExists(VersionsFile))
                    {
                        //if internet files not already loaded (== _oldVersionDate not set) and more up to date
                        if (!String.IsNullOrEmpty(_oldVersionDate) && versionDate != _oldVersionDate)
                        {
                            //update all files out of date
                            foreach (var file in versionJson)
                            {
                                var fileName = file.Key;
                                if (versionJson[fileName].Value<string>() != _oldVersionJson[fileName].Value<string>())
                                {
                                    var jsonFile = await _fileHelper.DownloadFileAsync(ServiceUrl + fileName);
                                    _fileHelper.CopyFileToLocal(jsonFile, fileName);
                                }
                            }

                        }
                    }
                    //load all files for the first time
                    else
                    {
                        foreach (var firstFile in versionJson)
                        {
                            var firstFileName = firstFile.Key;
                            var firstJsonFile = await _fileHelper.DownloadFileAsync(ServiceUrl + firstFileName);
                            _fileHelper.CopyFileToLocal(firstJsonFile, firstFileName);
                        }
                    }

                    //reload the Data in App
                    ((App)App.Current).LoadData();
                }
                catch (Exception e)
                {
                    var msg = e.InnerException;
                    Result = "Beim Laden der Artinformationen ist ein Problem aufgetreten.";
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
