using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MvvmHelpers.Commands;
using PlugEVMe.Models;
using PlugEVMe.Services;
using PropertyChanged;

namespace PlugEVMe.ViewModels
{
    // By using PropertyChanged.Fody, you can get rid of ALL the code I commented out and everything still works as you would expect.
    // The [AddINotifyPropertyChangedInterface] attribute instructs the compiler to inject INotifyPropertyChangedInterface to this class at compile-time,
    // AND it automatically sets up the firing of PropertyChanged events for all public getter properties on the class. So you went from having
    // a bunch of complex for for your IsBusy propety to only needing: "public bool IsBusy { get; set; }". Voila! You can bind to IsBusy the way
    // would any other binding-capable property. And since your other viewmodels derive from BaseViewModel, they don't need to have the
    // [AddINotifyPropertyChangedInterface] attribute; their public getters will all get the same treatment. So I will modify your other
    // viewmodels accordingly.
    // You can also use [AddINotifyPropertyChangedInterface] on all of your data models, which makes it so every property on your data models emit PropertyChanged events too. Very handy!
    [AddINotifyPropertyChangedInterface] 
    public abstract class BaseViewModel
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        protected INavService NavService { get; private set; }

        protected IAnalyticsService AnalyticsService { get; private set; }

        public bool IsBusy { get; protected set; } // IsBusy should never be able to be set by a consuming class, but it can be set by subclasses.

        public abstract Task Init();

        // PropertyChanged.Fody makes it so all you need to do in order handle property changes is create a method that conforms to:
        // On[YourProperty]Changed. Way easier than setting up all that complex event handling.
        public void OnIsBusyChanged()
        {
            // do whatever you want with IsBusy
            Debug.WriteLine($"{nameof(IsBusy)} changed: {IsBusy}");
        }

        public BaseViewModel(INavService navService, IAnalyticsService analyticsService)
        {
            NavService = navService;
            AnalyticsService = analyticsService;
        }

        // All of this PropertyChanged code is no longer needed because Fody "weaves" it all in for you at compile time.

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    var propertyChangedHandler = PropertyChanged;

        //    Debug.WriteLine("NotifyPropertyChanged: " + propertyName);

        //    if (propertyChangedHandler != null)
        //        propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        //}

        //protected virtual void OnIsBusyChanged([CallerMemberName] string propertyName = null)
        //{
        //    var propertyChangedHandler = PropertyChanged;
        //    if (propertyChangedHandler != null)
        //        propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        //}

    }

    public abstract class BaseViewModel<TParameter> : BaseViewModel
    {
        protected BaseViewModel(INavService navService, IAnalyticsService analyticsService)
            : base(navService, analyticsService)
        {
        }

        public override async Task Init()
        {
            var param = Activator.CreateInstance<TParameter>();
            await Init(param);
        }

        public abstract Task Init(TParameter parameter);
    }
}