using CoreGraphics;
using Foundation;
using KBS.App.TaxonFinder.CustomRenderers;
using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.iOS.Renderer;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace KBS.App.TaxonFinder.iOS.Renderer
{
    public class TaxonSelectionDelegate : UITextViewDelegate
    {
        public event EventHandler<TaxonLinkClickedEventArgs> TaxonSelected;
        public override bool ShouldInteractWithUrl(UITextView textView, NSUrl URL, NSRange characterRange)
        {
            if (URL.AbsoluteUrl.AbsoluteString.StartsWith("taxonid://"))
            {
                if (TaxonSelected != null)
                {
                    TaxonSelected(textView, new TaxonLinkClickedEventArgs() { TaxonId = int.Parse(URL.AbsoluteUrl.AbsoluteString.Replace("taxonid://", "")) });
                }
                return false;
            }
            if (URL.AbsoluteUrl.AbsoluteString.StartsWith("http://") || URL.AbsoluteUrl.AbsoluteString.StartsWith("https://"))
            {
                if (TaxonSelected != null)
                {
					Launcher.OpenAsync(URL.AbsoluteUrl);
                }
                return false;
            }

            return true;
        }
    }
    public class HtmlLabelRenderer : ViewRenderer<Label, UITextView>
    {


        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (HtmlLabel)Element;
            if (view == null || string.IsNullOrEmpty(view.Text)) return;

            UITextView textView = new UITextView(new CGRect(0, 0, view.Width, view.Height));

            textView.Text = ParseText(view.Text);
            textView.Font = UIFont.SystemFontOfSize((float)view.FontSize);
            textView.Editable = false;
            textView.Selectable = true;
            textView.UserInteractionEnabled = true;
            textView.TextColor = UIColor.Black;
            var d = new TaxonSelectionDelegate();
            d.TaxonSelected += D_TaxonSelected;
            textView.Delegate = d;

            // Setting the data detector types mask to capture all types of link-able data
            textView.DataDetectorTypes = UIDataDetectorType.All;
            textView.BackgroundColor = UIColor.Clear;


            // overriding Xamarin Forms Label and replace with our native control
            SetNativeControl(textView);

			if (textView.HasText)
			{
				var attr = new NSAttributedStringDocumentAttributes();
				var nsError = new NSError();
				attr.DocumentType = NSDocumentType.HTML;

				var text = ParseText(e.NewElement.Text);

				//I wrap the text here with the default font and size
				text = "<style>body{font-family: '" + this.Control.Font.Name + "'; font-size:" + this.Control.Font.PointSize + "px;}</style>" + text;

				var myHtmlData = NSData.FromString(text, NSStringEncoding.Unicode);
				this.Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
			}
		}

        private void D_TaxonSelected(object sender, TaxonLinkClickedEventArgs e)
        {
            ((HtmlLabel)Element)?.OnNavigate(sender, e);
        }

        private string ParseText(string text)
        {
            string result = text;
            if (!string.IsNullOrEmpty(text))
            {
                Regex pattern = new Regex("\\[\\[AS_[A-Za-z-]*\\]\\]");
                //Linkify.AddLinks(textView, pattern, "taxon://",null,new TaxonTransformFilter());

                foreach (Match m in pattern.Matches(result))
                {
                    var syn = TaxonSynonym.GetByPattern(m.Groups[0].Value);

                    if (syn != null)
                    {
						var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == syn.TaxonId && i.HasDiagnosis);
						if (taxon != null)
						{
							//result = result.Replace(syn.Pattern, string.Format("<a href=\"http://www.insekten-sachsen.de/Pages/TaxonomyBrowser.aspx?Id={0}\">{1}</a>", syn.TaxonId, syn.Text));
							result = result.Replace(syn.Pattern, $"<a href=\"taxonid://{syn.TaxonId}\">{syn.Text}</a>");
						}
						else
						{
							result = result.Replace(syn.Pattern, syn.Text);
						}
					}
                    else //if there is no TaxonSynonym at least hide the [[AS_...]] formating
                    {
                        result = result.Replace(m.Groups[0].Value, m.Groups[0].Value.Substring(5, m.Groups[0].Value.Length - 7));

                    }
                }

                //return "<style>body{font-family: '" + this.Control.Font.Name + "'; font-size:" + this.Control.Font.PointSize + "px;}</style>" + result;
            }

            return result;
        }



        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = (HtmlLabel)Element;
            if (view == null || string.IsNullOrEmpty(view.Text)) return;

            UITextView textView;

            if (Control == null)
            {
                textView = new UITextView();
                SetNativeControl(textView);
            }
            else if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                if (Element != null && !string.IsNullOrWhiteSpace(Element.Text))
                {
                    var attr = new NSAttributedStringDocumentAttributes();
                    var nsError = new NSError();
                    attr.DocumentType = NSDocumentType.HTML;

                    var text = "<style>body{font-family: '" + this.Control.Font.Name + "'; font-size:" + this.Control.Font.PointSize + "px;}</style>" + view.Text;
                    var myHtmlData = NSData.FromString(ParseText(text), NSStringEncoding.Unicode);
                    Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
                }
            }
        }

    }
}
