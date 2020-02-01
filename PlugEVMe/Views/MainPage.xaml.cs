using MvvmHelpers;
using Newtonsoft.Json;
using PlugEVMe.Models;
using PlugEVMe.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlugEVMe.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        private bool _IsOn;
        public ICommand NavigateCommand { get; set; }

        MainViewModel _vm => BindingContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }
        
        public bool IsOn
        {
            get
            {
                return _IsOn;
            }
            set
            {
                _IsOn = value;
                string onText = "Plan Journey";
                string offText = "Sorry. Journey Planning doesn't work yet!";
                JourneyPlannerButton.Text = _IsOn ? onText : offText;
            }
        }
        
        void Looks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var look = (PlugEVMeEntry)e.Item;

            _vm.ViewCommand.Execute(look);

            // Clear selection
            looks.SelectedItem = null;
        }

        // Even though you CAN set the view lifecycle methods as async...you shouldn't.
        // https://channel9.msdn.com/Shows/On-NET/Brandon-Minnick-asyncawait-best-practices
        // https://github.com/brminnick/AsyncAwaitBestPractices
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Initialize MainViewModel
            _vm?.Init()?.SafeFireAndForget();
        }

        protected void ShowEntriesButton_Click(object sender, EventArgs e)
        {
            (sender as Button).Text = "Show was just clicked!";
            var looks = (ObservableRangeCollection<PlugEVMeEntry>)this.looks.ItemsSource;
            var clone = JsonConvert.DeserializeObject<ObservableRangeCollection<PlugEVMeEntry>>(JsonConvert.SerializeObject(looks));
            _vm.PinsCommand.Execute(clone);

        }
        
        protected void JourneyPlannerButton_Click(object sender, EventArgs e)
        {
            IsOn = !IsOn;
        }
    }
}