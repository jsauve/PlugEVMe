using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;
using Xamarin.Essentials;

namespace PlugEVMe.Models
{
    // See BaseVieWModel for explanation of property changes
    [AddINotifyPropertyChangedInterface]
    public class PlugEVMeEntry
    {
 //       [JsonProperty("id")]
        public string Id { get; set; }
        public string Title { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public DateTime Date { get; set; }
		public int Rating { get; set; }
		public string Notes { get; set; }

        public Placemark Address { get; set; }

        // Default values if no value is set
        public PlugEVMeEntry(double lat, double lon)
        {
            Date = DateTime.UtcNow;
            Id = Date.ToString();
            Latitude = lat;
            Longitude = lon; 
            Rating = 0;
            Notes = Latitude.ToString() + " " + Longitude.ToString();
            Title = "Default Title " + Id;
        }

        public PlugEVMeEntry()
        {
            Date = DateTime.UtcNow;
            Id = Date.ToString();
            Latitude = 53.510144;        // Default location in Manchester, UK
            Longitude = -2.2274048000000004; 
            Rating = 0;
            Notes = Latitude.ToString() + " " + Longitude.ToString();
            Title = "Default Title " + Id;
        }

        // by conforming to On[MyProperty]Changed, this will fire when Latitude changes
        public void OnLatitudeChanged()
        {
            Debug.WriteLine($@"{nameof(Latitude)} changed:{Environment.NewLine}  Lat {Latitude}{Environment.NewLine}  Lon {Longitude}{Environment.NewLine}");
        }

        // by conforming to On[MyProperty]Changed, this will fire when Longitude changes
        public void OnLongitudeChanged()
        {
            Debug.WriteLine($@"{nameof(Longitude)} changed:{Environment.NewLine}  Lat {Latitude}{Environment.NewLine}  Lon {Longitude}{Environment.NewLine}");
        }
    }

}
