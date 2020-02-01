using Akavache;
using MvvmHelpers;
using Newtonsoft.Json;
using PlugEVMe.Exceptions;
using PlugEVMe.Generators;
using PlugEVMe.Models;
using PlugEVMe.Services;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using MvvmHelpers.Commands;
using Position = Plugin.Geolocator.Abstractions.Position;
using System.Windows.Input;

namespace PlugEVMe.ViewModels
{
    // See BaseVieWModel for explanation of property changes
    public class MainViewModel : BaseViewModel
    {
        readonly IBlobCache _cache;

        public ObservableRangeCollection<PlugEVMeEntry> LogEntries { get; set; } = new ObservableRangeCollection<PlugEVMeEntry>();

        public PlugEVMeEntry homeEntry { get; set; }

        // I got rid of a bunch of unnecessary code around your commands.
        // Just use a public getter for each command, and then set the command in your constuctor. Much cleaner.
        public Command<ObservableRangeCollection<PlugEVMeEntry>> PinsCommand { get; }

        public Command<PlugEVMeEntry> ViewCommand { get; }

        public ICommand NewCommand { get; }

        public AsyncCommand RefreshCommand { get; }

        public AsyncCommand InitCommand { get; }

        public MainViewModel(INavService navService, IBlobCache cache, IAnalyticsService analyticsService) : base(navService, analyticsService)
        {
            _cache = cache;

            PinsCommand = new Command<ObservableRangeCollection<PlugEVMeEntry>>(async (entries) => await ExecutePinsCommand(entries));

            ViewCommand = new Command<PlugEVMeEntry>(async (entry) => await ExecuteViewCommand(entry));

            NewCommand = new Command(async () => await ExecuteNewCommand());

            RefreshCommand = new AsyncCommand(async () => await LoadEntries());
        }

        public override async Task Init()
        {
            await LoadHomeEntry();
            await LoadEntries();
        }

        public async Task LoadHomeEntry()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;


                Position position;

                if (!Plugin.Geolocator.CrossGeolocator.IsSupported) { position = new Position(); }
                if (!Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationEnabled) { position = new Position(); }
                if (!Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationAvailable) { position = new Position(); }

                // perhaps use a flag variable to enable/disable returning locations
                var locator = Plugin.Geolocator.CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;
                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(120));

                PlugEVMeEntry entry = new PlugEVMeEntry();

                entry.Latitude = position.Latitude;
                entry.Longitude = position.Longitude;
                entry.Title = "Me";
                entry.Date = DateTime.Now;
                entry.Id = "1";


                if (position.Latitude != 0.0 && position.Longitude != 0.0)
                {
                    Double lat = Convert.ToDouble(position.Latitude);
                    Double lon = Convert.ToDouble(position.Longitude);
                    var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);
                    var placemark = placemarks?.FirstOrDefault();
                    entry.Notes = placemark.ToString();
                    entry.Address = placemark;
                }
                homeEntry = entry;
            }

            catch (NullReferenceException e)
            {
                Debug.WriteLine("NullReferenceException is thrown ! " + e.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Something is thrown ! " + e.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadEntries()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                LogEntries.Clear();

                ReadLibraries readLibraries = new ReadLibraries();
                List<string> libraries = readLibraries.libraries;

                var tempEntries = new List<PlugEVMeEntry>();

                tempEntries.Add(homeEntry);

                foreach (string library in libraries)
                {
                    PlugEVMeEntry _entry = new PlugEVMeEntry();

                    string[] libs = library.Split(',');

                    _entry.Longitude = Convert.ToDouble(libs[libs.Length - 1]);
                    _entry.Latitude = Convert.ToDouble(libs[libs.Length - 2]);
                    libs.Take(libs.Length - 1);
                    libs.Take(libs.Length - 2);

                    _entry.Title = libs[0];
                    _entry.Date = DateTime.Now;
                    _entry.Title = string.Join(" ", libs);
                    _entry.Notes = _entry.Latitude + " " + _entry.Longitude;
                    tempEntries.Add(_entry);
                }

                // AddRange() in ObservableRangeCollection differs from Add() in ObservableCollection in that it fires a single 
                // CollectionChanged notification when all the items are added to the collection at once, instead of many individual
                // CollectionChanged notifications from calling Add() over and over.
                LogEntries.AddRange(tempEntries);
            }
            finally
            {
                IsBusy = false;
            }

            await Task.CompletedTask;
        }

        async Task ExecutePinsCommand(ObservableRangeCollection<PlugEVMeEntry> entries)
        {
            try
            {
                var clone = JsonConvert.DeserializeObject<ObservableRangeCollection<PlugEVMeEntry>>(JsonConvert.SerializeObject(entries));
                await NavService.NavigateTo<PinItemsSourcePageViewModel, ObservableRangeCollection<PlugEVMeEntry>>(clone);
            }
            catch (AppException e)
            {
                Debug.WriteLine("In catch block of Main method.");
                Debug.WriteLine("Caught: {0}", e.Message);
                if (e.InnerException != null)
                    Debug.WriteLine("Inner exception: {0}", e.InnerException);
            }
        }


        async Task ExecuteViewCommand(PlugEVMeEntry entry)
        {
            await NavService.NavigateTo<DetailViewModel, PlugEVMeEntry>(entry);
        }

        async Task ExecuteNewCommand()
        {
            await NavService.NavigateTo<NewEntryViewModel>();
        }
    }
}
