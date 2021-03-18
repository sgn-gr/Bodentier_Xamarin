using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util.Regex;
using KBS.App.TaxonFinder.CustomRenderers;
using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Droid.Renderer;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace KBS.App.TaxonFinder.Droid.Renderer
{
    public class TaxonMovementMethod : LinkMovementMethod
	{
		public event EventHandler<TaxonLinkClickedEventArgs> LinkClicked;

		private RectF _touchedLineBounds = new RectF();


		public override bool OnTouchEvent(TextView widget, ISpannable buffer, MotionEvent e)
		{
			if (e.Action == MotionEventActions.Up)
			{
				int touchX = (int)e.GetX();
				int touchY = (int)e.GetY();

				var layout = widget.Layout;

				int touchedLine = widget.Layout.GetLineForVertical(touchY);
				int touchOffset = widget.Layout.GetOffsetForHorizontal(touchedLine, touchX);

				_touchedLineBounds.Left = layout.GetLineLeft(touchedLine);
				_touchedLineBounds.Top = layout.GetLineTop(touchedLine);
				_touchedLineBounds.Right = layout.GetLineWidth(touchedLine) + _touchedLineBounds.Left;
				_touchedLineBounds.Bottom = layout.GetLineBottom(touchedLine);

				if (_touchedLineBounds.Contains(touchX, touchY))
				{

					object[] spans = buffer.GetSpans(touchOffset, touchOffset, Java.Lang.Class.FromType(typeof(ClickableSpan)));
					foreach (object span in spans)
					{
						if (span is URLSpan)
						{
							var spanTarget = (URLSpan)span;
							if (LinkClicked != null)
							{
								switch (spanTarget.URL.Split(':')[0])
								{
									case "taxonid":
										LinkClicked(widget, new TaxonLinkClickedEventArgs() { TaxonId = int.Parse(spanTarget.URL.Replace("taxonid://", "")) });
										break;
									case "http":
									case "https":
										Launcher.OpenAsync(new Uri(spanTarget.URL));
										break;
								}
								return true;
							}
						}
					}

				}

				return base.OnTouchEvent(widget, buffer, e);
			}
			else
			{
				return false;
			}
		}
	}


	public class HtmlLabelRenderer : LabelRenderer
	{
		public HtmlLabelRenderer(Context context) : base(context)
		{

		}
		private TaxonMovementMethod mm;

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			//Control?.SetText(labelText, TextView.BufferType.Spannable);
			var view = (HtmlLabel)Element;
			if (view == null || string.IsNullOrEmpty(view.Text)) return;

#pragma warning disable 618
			var labelText = (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N) ? Html.FromHtml(Element.Text, FromHtmlOptions.ModeLegacy) : Html.FromHtml(Element.Text);
#pragma warning restore 618

			TextView textView = new TextView(this.Context);
			textView.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
			textView.SetTextColor(Xamarin.Forms.Color.Black.ToAndroid());

			// Setting the auto link mask to capture all types of link-able data
			textView.AutoLinkMask = 0;
			// Make sure to set text after setting the mask
			//textView.Text = labelText;
			SpannableStringBuilder sb = new SpannableStringBuilder(labelText);

			Pattern pattern = Pattern.Compile("\\[\\[AS_[A-Za-z-]*\\]\\]");
			//Linkify.AddLinks(textView, pattern, "taxon://",null,new TaxonTransformFilter());
			Matcher m = pattern.Matcher(sb);
			int diff = 0;
			while (m.Find())
			{
				var syn = TaxonSynonym.GetByPattern(m.Group(0));

				if (syn != null)
				{
					int start = m.Start() + diff;
					int end = m.End() + diff;

					sb.Replace(start, end, syn.Text);
					var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == syn.TaxonId && i.HasDiagnosis);
					if (taxon != null)
					{
						ClickableSpan cs = new URLSpan("taxonid://" + syn.TaxonId);
						sb.SetSpan(cs, start, start + syn.Text.Length, SpanTypes.InclusiveInclusive);
					}
					diff += syn.Text.Length - syn.Pattern.Length;
				}
				else
				{ //if there is no TaxonSynonym at least hide the [[AS_...]] formating
					int start = m.Start() + diff;
					int end = m.End() + diff;

					sb.Replace(start, end, m.Group(0).Substring(5, m.Group(0).Length - 7));
					diff += -7;
				}
			}

			textView.SetText(sb, TextView.BufferType.Spannable);
			textView.SetTextSize(ComplexUnitType.Dip, (float)view.FontSize);


			if (mm == null)
			{
				mm = new TaxonMovementMethod();
				mm.LinkClicked += Mm_LinkClicked;
			}
			textView.MovementMethod = mm;
			SetNativeControl(textView);


		}

		private void Mm_LinkClicked(object sender, TaxonLinkClickedEventArgs e)
		{
			((HtmlLabel)Element)?.OnNavigate(sender, e);
		}


		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);


		}
	}

}