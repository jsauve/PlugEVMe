using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlugEVMe.Models
{
    // See BaseVieWModel for explanation of property changes
    [AddINotifyPropertyChangedInterface]
    class PlugEVMeAddress
    {

        //Base on Xamarin.Essentials.Geocoding.GetPlacemarksAsync.placemark

        [JsonProperty("id")]
        public string Id { get; set; }
        public string AdminArea { get; set; } //Gets or sets the administrative area name of the address, for example, "CA", or null if it is unknown.
        public string CountryCode { get; set; } //Gets or sets the country ISO code.

        public string CountryName { get; set; } //Gets or sets the country name.
        public string FeatureName { get; set; } //Gets or sets the feature name.

        public string Locality { get; set; } //Gets or sets the city or town.

        public string Location { get; set; } //Gets or sets the location of the placemark.

        public string PostalCode { get; set; } //Gets or sets the postal code.

        public string SubAdminArea { get; set; } //Gets or sets the sub-administrative area name of the address, for example, "Santa Clara County", or null if it is unknown.

        public string SubLocality { get; set; } //Gets or sets the sub locality.

        public string SubThoroughfare { get; set; } //Gets or sets optional info: sub street or region.

        public string Thoroughfare { get; set; } //Gets or sets the street name.
    }
}
