using Bourse.ViewModels;
using Repository.Dbo;

namespace Bourse.Pages
{
    /// <summary>
    /// Page principale affichant la liste des actions
    /// </summary>
    [QueryProperty(nameof(Retour), "Retour")]
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Retour de la page détail de l'action
        /// </summary>
        public Business.Share Retour
        {
            set
            {
                if (BindingContext is MainViewModel vm)
                {
                    // On recharge toutes les données (j'aurai pu chargé que la donnée)
                    vm.Load();
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
                while(!ShareDbo.Instance.IsReady()) // On attend que la BD soit prête
                {
                    await Task.Delay(100);
                }
                vm.Load(); // Chargement des données
                await vm.FetchAsync(); // Récupération des données à jour depuis l'API en background

            }
        }
    }
}