using Plugin.Media.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KBS.App.TaxonFinder.Services
{
    public interface IFileHelper
	{
		bool FileExists(string filename);
		List<string> GetFiles();
		string GetLocalFilePath(string filename);
		string GetLocalAppPath(string filename);
		string GetAllText(string filePath);
		string GetBase64FromImagePath(string imagePath);
		MediaFile CopyFileToApp(string mediaPath);
		void CopyFileToLocal(byte[] file, string filename);
		Task CopyFileToLocalAsync(Task<byte[]> image, string filename);
		byte[] DownloadFile(string uri);
		Task<byte[]> DownloadFileAsync(string uri);
		void DeleteFile(string file);
	}
}
