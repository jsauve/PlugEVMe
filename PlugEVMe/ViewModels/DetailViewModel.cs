using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlugEVMe.Exceptions;
using PlugEVMe.Models;
using PlugEVMe.Services;

using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using PlugEVMe.Generators.MapTools;
using MvvmHelpers.Commands;

namespace PlugEVMe.ViewModels
{
    public class DetailViewModel : BaseViewModel<PlugEVMeEntry>
    {
        public PlugEVMeEntry Entry { get; set; } // See BaseViewModel comments for why I changed this.

        public DetailViewModel(INavService navService, IAnalyticsService analyticsService)
            : base(navService, analyticsService)
        {
        }

        public override async Task Init(PlugEVMeEntry logEntry)
        {
            AnalyticsService?.TrackEvent("Entry Detail Page", new Dictionary<string, string>
            {
                { "Title", logEntry?.Title }
            });

            Entry = logEntry;
        }
    }
}