using Newtonsoft.Json;
using PlugEVMe.Models;
using PlugEVMe.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PlugEVMe.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinItemsSourcePage : ContentPage
    {
        public PinItemsSourcePageViewModel _vm
        {
            get { return BindingContext as PinItemsSourcePageViewModel; }
        }

        bool _MapHasBeenInitialized;

        public PinItemsSourcePage()
        {
            InitializeComponent();

            map.PropertyChanged += Map_PropertyChanged;
        }

        private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"Map property changed: {e.PropertyName}");

            switch (e.PropertyName)
            {
                case nameof(Map.VisibleRegion):
                    if (!_MapHasBeenInitialized && map.VisibleRegion != null)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(53.8283459, -1.5794797), Distance.FromMiles(5000)));
                        _MapHasBeenInitialized = true;
                    }
                    break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (map.VisibleRegion != null)
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(53.8283459, -1.5794797), Distance.FromMiles(5000)));
                _MapHasBeenInitialized = true;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(PinItemsSourcePageViewModel.Locations))
            {


            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            Debug.WriteLine($"MapClick: {e.Position.Latitude}, {e.Position.Longitude}");
        }
    }
}
