using System;
using PropertyChanged;

namespace PlugEVMe.Models
{
    // See BaseVieWModel for explanation of property changes
    [AddINotifyPropertyChangedInterface]
    public class GeoCoords
    {
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
