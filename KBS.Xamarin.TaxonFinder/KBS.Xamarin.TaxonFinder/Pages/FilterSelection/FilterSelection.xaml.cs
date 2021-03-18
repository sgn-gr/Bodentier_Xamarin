using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterSelection : ContentPage
    {
        
        public string FilterTagGroupName
        {
            set
            {
                ((FilterSelectionViewModel)BindingContext).FilterTagGroupName = value;
            }
        }

        public int FilterTag
        {
            set
            {
                ((FilterSelectionViewModel)BindingContext).FilterTag = value;
            }
        }

        public object SliderValInfo { get; private set; }
        public double SliderVal { get; private set; }

        public FilterSelection()
        {
            InitializeComponent();
        }

        public FilterSelection(int parentFilterId)
        {
            InitializeComponent();
            FilterTag = parentFilterId;
        }

        public FilterSelection(string filterGroupName)
        {
            InitializeComponent();
            FilterTagGroupName = filterGroupName;
        }

        private void CategoryList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (FilterItem)e.Item;
            FilterTag = item.TagId;
        }

        private void SelectionList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (FilterItem)e.Item;
            item.Selected = !item.Selected;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var taxon = (Taxon)e.Item;
            if (taxon.TaxonomyStateName == "sp.")
            {
                Navigation.PushAsync(new TaxonInfo(taxon.TaxonId));
            }
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }
    }
}