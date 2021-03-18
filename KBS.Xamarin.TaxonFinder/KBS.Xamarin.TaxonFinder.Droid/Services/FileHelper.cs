using KBS.App.TaxonFinder.Droid.Services;
using KBS.App.TaxonFinder.Services;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace KBS.App.TaxonFinder.Droid.Services
{
    public class FileHelper : IFileHelper
    {
        public bool FileExists(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
            return File.Exists(path);
        }

        public List<string> GetFiles()
        {
            var fileList = new List<string>();
            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal)))
            {
                fileList.Add(Path.GetFileName(file));
            }
            return fileList;
        }

        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }

        public string GetLocalAppPath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (!Directory.Exists(Path.Combine(path, filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(path, filename)));
            }

            return Path.Combine(path, filename);
        }

        public MediaFile CopyFileToApp(string mediaPath)
        {
            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Path.GetFileName(mediaPath));

            File.Copy(mediaPath, documentsPath, true);
            return new MediaFile(documentsPath, null);
        }

        public void CopyFileToLocal(byte[] file, string filename)
        {
            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);

            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(documentsPath));
            }
            File.WriteAllBytes(documentsPath, file);
        }

        public async Task CopyFileToLocalAsync(Task<byte[]> file, string filename)
        {
            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);

            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(documentsPath));
            }
            var bytes = await file;
            if (bytes != null)
            {
                File.WriteAllBytes(documentsPath, bytes);
            }
        }

        public string GetAllText(string filePath)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filePath);
            return File.ReadAllText(path);
        }

        public string GetBase64FromImagePath(string imagePath)
        {
            byte[] b = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(b);
        }

        public byte[] DownloadFile(string uri)
        {
            byte[] result;
            try
            {
                using (WebClient client = new WebClient())
                {
                    result = client.DownloadData(new Uri(uri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            return result;
        }

        public async Task<byte[]> DownloadFileAsync(string uri)
        {
            byte[] result;
            try
            {
                using (WebClient client = new WebClient())
                {
                    result = await client.DownloadDataTaskAsync(new Uri(uri));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            return result;
        }

        public void DeleteFile(string file)
        {
            File.Delete(file);
        }
    }
}
