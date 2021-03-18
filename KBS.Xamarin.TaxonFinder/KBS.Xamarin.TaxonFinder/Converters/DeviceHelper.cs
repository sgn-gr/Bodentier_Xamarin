using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Helper
{
    public class DeviceHelper
    {
        public static string ImagePath(string fileName)
        {
            switch(Device.RuntimePlatform)
            {
                case "Android":
                    return string.Format("/Assets/Images/{0}.png",fileName);
                    
                case "iOS":
                    return string.Format("/Resources/Images/{0}.png", fileName);
            }

            return null;
            
        }
    }
}
