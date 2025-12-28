using Bourse.ViewModels;
using Repository.Dbo;

namespace Bourse.Pages
{
    [QueryProperty(nameof(Retour), "Retour")]
    public partial class MainPage : ContentPage
    {
        public Business.Share Retour
        {
            set
            {
                if (BindingContext is MainViewModel vm)
                {
                    vm.Init();
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(); 
            AppShell.SetNavBarIsVisible(this, false);
        }

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            AppShell.SetNavBarIsVisible(this, false);
        }

        /// <summary>
        /// customize behavior immediately prior to the page becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel vm)
            {
                while(!ShareDbo.Instance.IsReady())
                {
                    await Task.Delay(100);
                }
                vm.Init();
            }
        }
    }
}