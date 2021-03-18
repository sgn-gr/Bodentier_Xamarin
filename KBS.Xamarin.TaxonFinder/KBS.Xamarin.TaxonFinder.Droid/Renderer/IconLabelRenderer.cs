using Android.Content;
using Android.Graphics;
using KBS.App.TaxonFinder.CustomRenderers;
using KBS.App.TaxonFinder.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(IconLabel), typeof(IconLabelRenderer))]
namespace KBS.App.TaxonFinder.Droid.Renderer
{



    public class IconLabelRenderer:LabelRenderer
    {
        public IconLabelRenderer(Context context):base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                //The ttf in /Assets is CaseSensitive, so name it FontAwesome.ttf
                Control.Typeface = Typeface.CreateFromAsset(this.Context.Assets, "FontAwesome.ttf");
            }
        }
    }
}