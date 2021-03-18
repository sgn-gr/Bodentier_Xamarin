using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TaxonMediaInfo : ContentPage
	{

		double currentScale = 1;
		double startScale = 1;
		double xOffset = 0;
		double yOffset = 0;

		public TaxonMediaInfo()
		{
			InitializeComponent();
		}

		public TaxonMediaInfo(string mediaTitle)
		{
			InitializeComponent();
			if(mediaTitle != null)
            {
				if (Int32.TryParse(mediaTitle, out var mediaId))
				{
					TaxonMediaInfoViewModel.SelectedMediaId = mediaId;
				}
				else if (Guid.TryParse(mediaTitle, out var mediaGuid))
				{
					TaxonMediaInfoViewModel.SelectedMediaGuid = mediaGuid;
				}
				else
				{
					//string title for slider images
					TaxonMediaInfoViewModel.SelectedMediaTitle = mediaTitle;
				}
			}
			var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
			var screenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
			TaxonImage.HeightRequest = screenWidth / 3.43;
		}

		public TaxonMediaInfo(int mediaId)
		{
			InitializeComponent();
			if (mediaId != 0)
			{
				TaxonMediaInfoViewModel.SelectedMediaId = mediaId;
			}
			var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
			var screenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
			TaxonImage.HeightRequest = screenWidth / 3.43;
		}
		public TaxonMediaInfo(string imagePath, int taxonId)
		{
			InitializeComponent();
			String imgTitle = imagePath.Split('/').Last();
			Regex rgx = new Regex(@"^.+_2\d{7}_\d{6}\.[a-z]{3}$");
			TaxonImage.Source = imagePath;
			//CopyrightLabel.Text = imgTitle;
			var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == (int)(taxonId));
			TitleLabel.Text = (taxon != null) ? taxon.LocalName : "Unbekannte Art";
			//SubTitleLabel.Text = (taxon != null) ? taxon.TaxonName : "Unbekannte Art";
			if (rgx.IsMatch(imgTitle))
			{
				DescriptionLabel.Text = "Aufgenommen am " + imgTitle.Split('_')[1].Substring(6, 2) + "." + imgTitle.Split('_')[1].Substring(4, 2) + "." + imgTitle.Split('_')[1].Substring(0, 4) + " um " + imgTitle.Split('_')[2].Substring(0, 2) + ":" + imgTitle.Split('_')[2].Substring(2, 2) + " Uhr.";
			}
			else
			{
				DescriptionLabel.Text = "";
			}
		}
		private TaxonMediaInfoViewModel TaxonMediaInfoViewModel
		{
			get
			{
				return (TaxonMediaInfoViewModel)BindingContext;
			}
		}

		void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
		{
			if (e.Status == GestureStatus.Started)
			{
				// Store the current scale factor applied to the wrapped user interface element,
				// and zero the components for the center point of the translate transform.
				startScale = Content.Scale;
				Content.AnchorX = 0;
				Content.AnchorY = 0;
			}
			if (e.Status == GestureStatus.Running)
			{
				// Calculate the scale factor to be applied.
				currentScale += (e.Scale - 1) * startScale;
				currentScale = Math.Max(1, currentScale);

				// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
				// so get the X pixel coordinate.
				double renderedX = Content.X + xOffset;
				double deltaX = renderedX / Width;
				double deltaWidth = Width / (Content.Width * startScale);
				double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

				// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
				// so get the Y pixel coordinate.
				double renderedY = Content.Y + yOffset;
				double deltaY = renderedY / Height;
				double deltaHeight = Height / (Content.Height * startScale);
				double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

				// Calculate the transformed element pixel coordinates.
				double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
				double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

				// Apply translation based on the change in origin.
				Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
				Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

				// Apply scale factor.
				Content.Scale = currentScale;
			}
			if (e.Status == GestureStatus.Completed)
			{
				// Store the translation delta's of the wrapped user interface element.
				xOffset = Content.TranslationX;
				yOffset = Content.TranslationY;
			}
		}

	}
}