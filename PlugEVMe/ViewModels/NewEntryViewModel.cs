﻿using PlugEVMe.Models;
using PlugEVMe.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlugEVMe.ViewModels
{
	public class NewEntryViewModel : BaseViewModel
    {
		readonly ILocationService _locService;

        string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged();
				SaveCommand.ChangeCanExecute();
			}
		}

		double _latitude;
		public double Latitude
		{
			get { return _latitude; }
			set
			{
				_latitude = value;
				OnPropertyChanged();
			}
		}

		double _longitude;
		public double Longitude
		{
			get { return _longitude; }
			set
			{
				_longitude = value;
				OnPropertyChanged();
			}
		}

		DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged();
			}
		}

		int _rating;
		public int Rating
		{
			get { return _rating; }
			set
			{
				_rating = value;
				OnPropertyChanged();
			}
		}

		string _notes;
		public string Notes
		{
			get { return _notes; }
			set
			{
				_notes = value;
				OnPropertyChanged();
			}
		}

		Command _saveCommand;
		public Command SaveCommand
		{
			get
			{
				return _saveCommand ?? (_saveCommand = new Command(async () => await ExecuteSaveCommand(), CanSave));
			}
		}

        public NewEntryViewModel(INavService navService, ILocationService locService,
            IPlugEVMeDataService plugMeService,
            IAnalyticsService analyticsService)
            : base(navService, analyticsService)
        {
            _locService = locService;

            Date = DateTime.Today;
            Rating = 1;
        }

        public override async Task Init()
		{
			var coords = await _locService.GetGeoCoordinatesAsync();
			Latitude = coords.Latitude;
			Longitude = coords.Longitude;
		}

		async Task ExecuteSaveCommand()
		{
			if (IsBusy)
			{
				return;
			}

			IsBusy = true;

			try
			{
				var newItem = new PlugEVMeEntry
                {
					Title = Title,
					Latitude = Latitude,
					Longitude = Longitude,
					Date = Date,
					Rating = Rating,
					Notes = Notes
				};

				await Task.Delay(3000);

				await NavService.GoBack();
			}
			finally
			{
				IsBusy = false;
			}
		}

		bool CanSave()
		{
			return !string.IsNullOrWhiteSpace(Title);
		}
	}
}
