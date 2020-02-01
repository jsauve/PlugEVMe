using MvvmHelpers;
using PlugEVMe.Models;
using PlugEVMe.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PlugEVMe.Generators.MapTools
{
    [AddINotifyPropertyChangedInterface]
    public class BindableEVCPMap : Map
    {
        public BindableEVCPMap()
        {
            EVCPPinsSource = new ObservableRangeCollection<EVCPPin>();
            EVCPPinsSource.CollectionChanged += PinsSourceOnCollectionChanged;
        }
        public ObservableRangeCollection<EVCPPin> EVCPPinsSource { get; } = new ObservableRangeCollection<EVCPPin>();

        public static readonly BindableProperty EVCPPinsProperty = BindableProperty.Create(
        propertyName: "EVCPPinsSource",
        returnType: typeof(ObservableRangeCollection<EVCPPin>),
        declaringType: typeof(BindableEVCPMap),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        validateValue: null,
        propertyChanged: PinsSourcePropertyChanged);
        public MapSpan MapSpan
        {
            get { return (MapSpan)GetValue(MapSpanProperty); }
            set { SetValue(MapSpanProperty, value); }
        }

        public Position Position { get; internal set; }

        public static readonly BindableProperty MapSpanProperty = BindableProperty.Create(
        propertyName: "MapSpan",
        returnType: typeof(MapSpan),
        declaringType: typeof(BindableEVCPMap),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        validateValue: null,
        propertyChanged: MapSpanPropertyChanged);
        private static void MapSpanPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisInstance = bindable as BindableEVCPMap;
            var newMapSpan = newValue as MapSpan;
            thisInstance?.MoveToRegion(newMapSpan);
        }
        private static void PinsSourcePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var thisInstance = bindable as BindableEVCPMap;
            var newPinsSource = newValue as ObservableRangeCollection<EVCPPin>;
            if (thisInstance == null ||
            newPinsSource == null)
                return;
            UpdatePinsSource(thisInstance, newPinsSource);
        }
        
        private void PinsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePinsSource(this, sender as IEnumerable<EVCPPin>);
        }
        private static void UpdatePinsSource(BindableEVCPMap bindableMap, IEnumerable<EVCPPin> newSource)
        {
            bindableMap.Pins.Clear();
            foreach (var pin in newSource)
                bindableMap.Pins.Add(pin);
        }
    }
}