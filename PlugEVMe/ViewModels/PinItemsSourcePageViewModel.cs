﻿using Akavache;
using PlugEVMe.Generators;
using PlugEVMe.Models;
using PlugEVMe.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using PlugEVMe.ViewModels;

using Position = Xamarin.Forms.Maps.Position;
using MvvmHelpers;
using Map = Xamarin.Forms.Maps.Map;

namespace PlugEVMe.ViewModels
{
    public class PinItemsSourcePageViewModel : BaseViewModel<ObservableCollection<PlugEVMeEntry>>
    {
        int _pinCreatedCount = 0;
        readonly IBlobCache _cache;
        
        ObservableCollection<Pin> _locations;
        ObservableCollection<PlugEVMeEntry> _logEntries;

        public ObservableCollection<Pin> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlugEVMeEntry> LogEntries
        {
            get { return _logEntries; }
            set
            {
                _logEntries = value;
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddLocationCommand { get; }
        public ICommand RemoveLocationCommand { get; }
        public ICommand ClearLocationsCommand { get; }
        public ICommand UpdateLocationsCommand { get; }
        public ICommand ReplaceLocationCommand { get; }

        public PinItemsSourcePageViewModel(INavService navService, IAnalyticsService analyticsService)
            : base(navService,
                   analyticsService)
        {
            AddLocationCommand = new Command(AddLocation);
            RemoveLocationCommand = new Command(RemoveLocation);
            ClearLocationsCommand = new Command(() => _locations.Clear());
            UpdateLocationsCommand = new Command(UpdateLocations);
            ReplaceLocationCommand = new Command(ReplaceLocation);
        }

        void AddLocation()
        {
            _locations.Add(NewLocation());
        }

        void RemoveLocation()
        {
            if (_locations.Any())
            {
                _locations.Remove(_locations.First());
            }
        }

        void UpdateLocations()
        {
            if (!_locations.Any())
            {
                return;
            }

            double lastLatitude = _locations.Last().Position.Latitude;
            foreach (Pin location in Locations)
            {
                var newposted = new Pin()
                {
                    Position = new Position(lastLatitude, location.Position.Longitude),
                    Label = lastLatitude + ", " + location.Position.Longitude.ToString()
                };
                Locations.Add(newposted);
            }
        }

        void ReplaceLocation()
        {
            if (!_locations.Any())
            {
                return;
            }

            _locations[_locations.Count - 1] = NewLocation();
        }

        Pin NewLocation()
        {
            return new Pin();
        }

        public override async Task Init(ObservableCollection<PlugEVMeEntry> logEntries)
        {
            AnalyticsService.TrackEvent("Entry Detail Page", new Dictionary<string, string>
            {
                { "Title", logEntries.ToString() }
            });
            Debug.WriteLine(logEntries.ToString());

            LogEntries = logEntries;
            var map = new Map(MapSpan.FromCenterAndRadius(new Position(37, -122), Distance.FromMiles(10)));

            var pin = new Pin()
            {
                Position = new Position(37, -122),
                Label = "Default Pin!"
            };
            _pinCreatedCount++;
            map.Pins.Add(pin);
            Locations.Add(pin);

            // Add pins to map
            if (LogEntries != null)
            {
                foreach (var evCP in logEntries)
                {
                    Pin newpin = new Pin()
                    {
                        Position = new Position(Convert.ToDouble(evCP.Latitude), Convert.ToDouble(evCP.Longitude)),
                        Type = PinType.Place,
                        Label = Convert.ToString(evCP.Id) + " " + evCP.Title.ToString()
                    };
                    newpin.MarkerClicked += async (s, args) =>
                    {
                        args.HideInfoWindow = true;
                        string pinName = ((Pin)s).Label;
                        Debug.WriteLine("Pin Clicked", $"{pinName} was clicked.", "Ok");
                    };
                    map.Pins.Add(newpin);
                    Locations.Add(newpin);
                    _pinCreatedCount++;
                }
            }
        }
    }
}
