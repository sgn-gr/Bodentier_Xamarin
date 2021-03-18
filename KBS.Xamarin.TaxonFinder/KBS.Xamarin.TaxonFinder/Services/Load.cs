using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Services
{
    public static class Load
	{
		public static List<T> FromFile<T>(string fileName)
        {
            Assembly assembly = typeof(T).GetTypeInfo().Assembly;
            string assemblyName = assembly.GetName().Name;
            string result;

            var fileHelper = DependencyService.Get<IFileHelper>();
            //use downloaded json
            if (IsDownloadedDataNewer(fileName) && fileHelper.FileExists(fileName))
            {
                result = fileHelper.GetAllText(fileName);
            }
            //use backup json
            else
            {
                string pathName = $"{assemblyName}.Resources.{fileName}";
                Stream stream = assembly.GetManifestResourceStream(pathName);
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return JsonConvert.DeserializeObject<List<T>>(result);
        }

        public static bool IsDownloadedDataNewer(string fileName)
        {
            var appVersions = ((App)App.Current).AppVersions;
            var downloadedVersions = ((App)App.Current).DownloadedVersions;
            DateTime appDate = default;
            DateTime downloadDate = default;
            try
            {
                var appDateString = appVersions[fileName].Value<string>();
                appDate = DateTime.Parse(appDateString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (downloadedVersions != null)
            {
                try
                {
                    var downloadDateString = downloadedVersions[fileName].Value<string>();
                    downloadDate = DateTime.Parse(downloadDateString);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return downloadDate > appDate;
        }
    }
}