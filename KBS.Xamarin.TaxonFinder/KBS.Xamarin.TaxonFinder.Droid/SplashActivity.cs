
using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;

namespace KBS.App.TaxonFinder.Droid
{
    [Activity(Label = "BODENTIER hoch 4", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }



    }
}