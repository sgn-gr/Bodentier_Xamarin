using KBS.App.TaxonFinder.iOS.Services;
using KBS.App.TaxonFinder.Services;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceId))]
namespace KBS.App.TaxonFinder.iOS.Services
{
    public class DeviceId : IDeviceId
    {
        public string GetDeviceId()
        {
            return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
    }
}
