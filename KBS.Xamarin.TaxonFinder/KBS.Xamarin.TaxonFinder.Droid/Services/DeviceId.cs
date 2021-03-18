using Android.App;
using KBS.App.TaxonFinder.Droid.Services;
using KBS.App.TaxonFinder.Services;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceId))]
namespace KBS.App.TaxonFinder.Droid.Services
{
    public class DeviceId : IDeviceId
    {
        public string GetDeviceId()
        {
            return Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId);
        }
    }
}
