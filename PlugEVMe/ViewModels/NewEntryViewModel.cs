using PlugEVMe.Models;
using PlugEVMe.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlugEVMe.ViewModels
{
    // See BaseVieWModel for explanation of property changes
	public class NewEntryViewModel : BaseViewModel
    {
		readonly ILocationService _locService;

        public string Title { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Date { get; set; }

        public int Rating { get; set; }

        public string Notes { get; set; }

        // I got rid of a bunch of unnecessary code around your commands.
        // Just use a public getter for each command, and then set the command in your constuctor. Much cleaner.
		public Command SaveCommand { get; }

        public NewEntryViewModel(INavService navService, ILocationService locService,
            //IPlugEVMeDataService plugMeService,
            IAnalyticsService analyticsService)
            : base(navService, analyticsService)
        {
            _locService = locService;

            Date = DateTime.Today;
            Rating = 1;

			SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanSave);
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
