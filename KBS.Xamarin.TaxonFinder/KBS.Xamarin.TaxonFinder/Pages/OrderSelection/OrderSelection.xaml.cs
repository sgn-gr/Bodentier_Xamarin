using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Views
{
    public partial class OrderSelection : ContentPage
    {

        public List<string> groupList = new List<string> { "Bodentiere", "Doppelfüßer (Diplopoda)", "Samenfüßer (Chordeumatida)", "Bandfüßer (Polydesmida)", "Schnurfüßer (Julida)", "Saftkugler (Glomerida)", "Pinselfüßer (Polyxenida)", "Bohrfüßer (Polyzoniida)", "Hundertfüßer (Chilopoda)", "Steinläufer (Lithobiomorpha)", "Skolopender(Scolopendromorpha)", "Erdkriecher (Geophilomorpha)", "Spinnenläufer (Scutigeromorpha)", "Asseln (Isopoda)" };

        public OrderSelection()
        {
            InitializeComponent();
        }

        private void BodentiereSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Bodentiere"));
        }

        private void DoppelfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Doppelfüßer (Diplopoda)"));
        }
        private void SamenfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Samenfüßer (Chordeumatida)"));
        }

        private void BandfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Bandfüßer (Polydesmida)"));
        }

        private void SchnurfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Schnurfüßer (Julida)"));
        }

        private void SaftkuglerSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Saftkugler (Glomerida)"));
        }

        private void PinselfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Pinselfüßer (Polyxenida)"));
        }

        private void BohrfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Bohrfüßer (Polyzoniida)"));
        }

        private void HundertfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Hundertfüßer (Chilopoda)"));
        }

        private void SteinlaeuferSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Steinläufer (Lithobiomorpha)"));
        }

        private void SkolopenderSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Skolopender (Scolopendromorpha)"));
        }

        private void ErdkriecherSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Erdkriecher (Geophilomorpha)"));
        }

        private void SpinnenlaeuferSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Spinnenläufer (Scutigeromorpha)"));
        }

        private void LandasselnSelectionButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FilterSelection("Asseln (Isopoda)"));
        }

    }
}
