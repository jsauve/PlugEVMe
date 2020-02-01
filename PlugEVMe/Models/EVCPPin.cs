using System;
using System.ComponentModel;
using PropertyChanged;
using Xamarin.Forms.Maps;

namespace PlugEVMe.Models
{
    // See BaseVieWModel for explanation of property changes
    [AddINotifyPropertyChangedInterface]
    public class EVCPPin : Pin
    {
        private Position _position;
        public string Name { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string DateLastUpdated { get; set; }
        public string PinIcon { get; set; }
        public string Tag { get; internal set; }
        public Uri Url { get; internal set; }
        public string Address { get; }

        public Position Position { get; set; }

        public EVCPPin()
        {
        }
    }
}
