using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Data
{
    public class Taxon
    {
        public int TaxonId { get; set; }
        public int TaxonTypeId { get; set; }
        public string TaxonName { get; set; }
        private string localName;
        public string LocalName
        {
            get
            {
                return (localName != "") ? localName : TaxonName;
            }
            set
            {
                localName = value;
            }
        }

        public string FamilyName { get; set; }
        private string familyLocalName;
        public string FamilyLocalName
        {
            get
            {
                return (familyLocalName != "") ? familyLocalName : FamilyName;
            }
            set
            {
                familyLocalName = value;
            }
        }
        public string OrderName { get; set; }
        private string orderLocalName;
        public string OrderLocalName
        {
            get
            {
                return (orderLocalName != "") ? orderLocalName : OrderName;
            }
            set
            {
                orderLocalName = value;
            }
        }

        public int? OrderId { get; set; }
        public string TaxonomyStateName { get; set; }

        public string TaxonAuthor { get; set; }
        public string Diagnosis { get; set; }
        public bool HasDiagnosis { get; set; }

        public int? IdentificationLevelMale { get; set; }
        public int? IdentificationLevelFemale { get; set; }

        public int Relevance { get; set; }
        public int TotalCriteria { get; set; }
        public bool ShowAll { get; set; }

        public string TaxonKey
        {
            get { return string.Format("{0}T{1}", TaxonId, TaxonTypeId); }
        }

        private ImageSource _identImageMale = null;
        private ImageSource _identImageFemale = null;

        private static readonly Assembly assembly = typeof(Taxon).GetTypeInfo().Assembly;
        private static readonly string assemblyName = assembly.GetName().Name;

        public ImageSource IdentImageMale
        {
            get
            {
                if (_identImageMale == null)
                    _identImageMale = ImageSource.FromResource($"{assemblyName}.Images.Ident.{GetIdentNameByLevel(IdentificationLevelMale, false)}");
                return _identImageMale;
            }
        }

        public ImageSource IdentImageFemale
        {
            get
            {
                if (_identImageFemale == null)
                    _identImageFemale = ImageSource.FromResource($"{assemblyName}.Images.Ident.{GetIdentNameByLevel(IdentificationLevelFemale, true)}");
                return _identImageFemale;
            }
        }

        public string AllowTaxonInfoViewIcon
        {
            get
            {
                if (TaxonomyStateName == "sp.")
                {
                    return "\u2794";
                }
                return "";
            }
        }

        private string GetIdentNameByLevel(int? level, bool female)
        {
            if (level.HasValue)
            {
                switch (level.Value)
                {
                    case 1:
                        return "ident_green" + (female ? "_r" : "") + ".png";
                    case 2:
                        return "ident_yellow" + (female ? "_r" : "") + ".png";
                    case 3:
                        return "ident_red" + (female ? "_r" : "") + ".png";
                    case 4:
                        return "ident_grey" + (female ? "_r" : "") + ".png";

                }

            }
            return "ident_blank.png";
        }

        public ImageSource ListImage
        {
            get
            {
                var inaString = ImageSource.FromResource($"{assemblyName}.Images.General.ina.jpg");
                if (ImagesLoaded && Images.Count != 0)
                {
                    //var image = (Images.FirstOrDefault(j => j.ImageId == ImageTypes.OrderBy(i => i.TaxonId).First().ImageId) ?? Images.First()).LoRes;
                    //{File: /data/user/0/KBS.App.TaxonFinder/files/Polz_ger_habitus1.jpg}
                    var image = Images.First().LoRes == null ? Images.First().HiRes : Images.First().LoRes;
                    if (image != null)
                    {
                        return image;
                    }
                }
                return inaString;
            }
        }

        public bool ImagesLoaded
        {
            get
            {
                var imageDate = Preferences.Get("imageDate", string.Empty);
                return !string.IsNullOrEmpty(imageDate);
            }
        }
        public bool HasImages
        {
            get
            {
                if (Images.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public List<TaxonImage> Images { get; set; }
        public List<TaxonImageType> ImageTypes { get; set; }
        public string SearchString
        {
            get
            {
                var searchString = (string.IsNullOrEmpty(TaxonName) ? "" : TaxonName) + (string.IsNullOrEmpty(LocalName) ? "" : LocalName) + (string.IsNullOrEmpty(FamilyName) ? "" : FamilyName) + (string.IsNullOrEmpty(FamilyLocalName) ? "" : FamilyLocalName) + (string.IsNullOrEmpty(OrderName) ? "" : OrderName) + (string.IsNullOrEmpty(OrderLocalName) ? "" : OrderLocalName);
                return searchString;
            }
        }
    }
}
