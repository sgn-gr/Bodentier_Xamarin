using System;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Views
{
    public partial class Feedback : ContentPage
	{
		public Feedback()
		{
			InitializeComponent();
		}
		private void SendButton_Clicked(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(MailTextEditor.Text))
			{
				MailTextEditor.Focus();
			}
		}
	}
}
