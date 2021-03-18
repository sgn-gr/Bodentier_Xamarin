using KBS.App.TaxonFinder.ViewModels;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Views
{
    public partial class TakeAudio : ContentPage
	{
		public TakeAudio()
		{
			InitializeComponent();
		}
		public TakeAudio(string mediaPath)
		{
			InitializeComponent();
			TakeAudioViewModel.LoadAudio(mediaPath);
		}
		protected override void OnDisappearing()
		{
			if (TakeAudioViewModel.Recorder.IsRecording)
				TakeAudioViewModel.Recorder.StopRecording();
			TakeAudioViewModel.Player.Stop();
			TakeAudioViewModel.Player.Dispose();
			base.OnDisappearing();
		}
		private TakeAudioViewModel TakeAudioViewModel
		{
			get
			{
				return (TakeAudioViewModel)BindingContext;
			}
		}
	}
}
