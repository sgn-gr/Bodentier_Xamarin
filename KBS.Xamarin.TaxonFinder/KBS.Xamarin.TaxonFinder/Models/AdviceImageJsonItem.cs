using System.IO;
using System.Runtime.Serialization;

namespace KBS.App.TaxonFinder.Models
{
	public class AdviceImageJsonItem
    {
        [DataMember]
        public string ImageName { get; set; }

        [DataMember]
        public string ImageBase64 { get; set; }

        public AdviceImageJsonItem(string imageBase, string imageCompleteName)
        {
            ImageBase64 = imageBase;
            ImageName = Path.GetFileName(imageCompleteName);
        }
    }

}
