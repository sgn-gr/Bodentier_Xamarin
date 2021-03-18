using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Views;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Data
{
    public class TaxonImage
    {

        #region Fields

        private ImageSource _loResSource = null;
        private ImageSource _hiResSource = null;
        private static readonly Assembly assembly = typeof(Taxon).GetTypeInfo().Assembly;
        private static readonly string assemblyName = assembly.GetName().Name;
        public static ImageSource inaString = ImageSource.FromResource($"{assemblyName}.Images.General.ina.jpg");

        #endregion

        #region Properties

        public int TaxonId { get; set; }
        public string ImageId { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }

        public string LocalName
        {
            get
            {
                var taxon = ((App)App.Current).Taxa.Find(i => i.TaxonId == TaxonId);
                if (taxon != null)
                {
                    return taxon.LocalName;
                }
                return "";
            }
        }
        public string TaxonName
        {
            get
            {
                var taxon = ((App)App.Current).Taxa.Find(i => i.TaxonId == TaxonId);
                if (taxon != null)
                {
                    return taxon.TaxonName;
                }
                return "";
            }
        }
        public string Description { get; set; }
        public string Copyright { get; set; }

        public bool Male { get; set; }
        public bool Female { get; set; }
        public bool Downside { get; set; }
        public bool SecondForm { get; set; }

        public ImageSource LoRes
        {
            get
            {
                return GetImage(_loResSource, $"{Title}.jpg");
            }
        }

        public ImageSource HiRes
        {
            get
            {
                return GetImage(_hiResSource, $"{Title}.jpg");
            }
        }

        #endregion

        #region Methods
        private ImageSource GetImage(ImageSource resSource, string filePath)
        {
            if (resSource == null && !string.IsNullOrEmpty(ImageId))
            {
                var fileHelper = DependencyService.Get<IFileHelper>();
                var fileName = filePath;
                if (!fileHelper.FileExists(fileName))
                {
                    try
                    {
                        var result = fileHelper.DownloadFile($"https://bodentierhochvier.de/wp-content/uploads/{fileName}");
                        fileHelper.CopyFileToLocal(result, fileName);
                    }
                    catch (Exception)
                    {
                    }
                }
                resSource = ImageSource.FromFile(fileHelper.GetLocalAppPath(fileName));
            }
            return resSource;
        }

        #endregion
    }
}