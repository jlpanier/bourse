using Bourse.Interfaces;
using Bourse.Pages;
using Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFImageLoading.Helpers;
using Repository.Dbo;
using Syncfusion.Maui.DataSource.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Bourse.ViewModels
{
    /// <summary>
    /// Viewmodel de la page principale, gère la liste des actions et les interactions avec celle ci
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// Symbol pour le menu de tri, et ordre de tri
        /// </summary>
        private enum OrderingBy 
        {
            [StringValue("\U0001F520")]
            Name,
            [StringValue("\U0001F47E")]
            Code,
            [StringValue("\U0001F649")]
            Consensus,
            [StringValue("\U0001FAF8")]
            Risk,
            [StringValue("\U0001F44F")]
            Rendement
        }
        private OrderingBy sort = OrderingBy.Consensus;

        [ObservableProperty]
        public string sortBy = OrderingBy.Consensus.GetStringValue();

        [ObservableProperty]
        public bool canDowload = true;
        [ObservableProperty]
        private ObservableCollection<ShareViewModel> items;

         public MainViewModel()
         {
            Items = new ObservableCollection<ShareViewModel>();
         }

        [RelayCommand]
        async Task Add()
        {
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}");
        }

        [RelayCommand]
        async Task Edit(ShareViewModel itemviewmodel)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                ["item"] = itemviewmodel.Item
            };
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}", navigationParameters);
        }

        [RelayCommand]
        private void OrderBy()
        {
            switch (sort)
            {
                case OrderingBy.Name:
                    sort = OrderingBy.Code;
                    Items = new ObservableCollection<ShareViewModel>(Items.OrderBy(_=>_.Code));
                    break;
                case OrderingBy.Code:
                    sort = OrderingBy.Consensus;
                    Items = new ObservableCollection<ShareViewModel>(Items.OrderBy(_ => _.Consensus));
                    break;
                case OrderingBy.Consensus:
                    sort = OrderingBy.Risk;
                    Items = new ObservableCollection<ShareViewModel>(Items.OrderBy(_ => _.Risk));
                    break;
                case OrderingBy.Risk:
                    sort = OrderingBy.Rendement;
                    Items = new ObservableCollection<ShareViewModel>(Items.OrderByDescending(_ => _.Rendement));
                    break;
                case OrderingBy.Rendement:
                    sort = OrderingBy.Name;
                    Items = new ObservableCollection<ShareViewModel>(Items.OrderBy(_ => _.Name));
                    break;
            }
            SortBy = sort.GetStringValue();
        }

        [RelayCommand]
        async Task Tap(ShareViewModel itemviewmodel)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                ["item"] = itemviewmodel.Item
            };
            await Shell.Current.GoToAsync($"{nameof(BoursoramaPage)}", navigationParameters);
        }

        [RelayCommand]
        private void Update()
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += OnUpdateWork;
            worker.RunWorkerCompleted += OnUpdateCompleted;
            worker.ProgressChanged += OnUpdateReport;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Mise à jour des valeurs des actions
        /// </summary>
        private void OnUpdateWork(object? sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
            {
                var i = 1;
                Items.ForEach(_ =>
                {
                    if (_.Fetch() && i % 4 == 0)
                    {
                        worker.ReportProgress(i++, $"En cours...");
                    }
                    i++;
                });
            }
        }

        /// <summary>
        /// Rafraichissement de l'affichage lors de la recherche
        /// </summary>
        private void OnUpdateReport(object? sender, ProgressChangedEventArgs e)
        {
            Load();
        }

        /// <summary>
        /// Rafraichissement de l'affichage après la recherche
        /// </summary>
        private void OnUpdateCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Load();
        }

        /// <summary>
        /// Mise à jour des valeurs des actions en background (démarrage de l'appli)
        /// </summary>
        public async Task FetchAsync()
        {
            await Task.Run(() => Items.ForEach(_ => _.Fetch()));
        }

        /// <summary>
        /// Mise à disposition de la base de données
        /// </summary>
        [RelayCommand]
        private async Task Download()
        {
            try
            {
                var saver = ServiceHelper.GetService<IFileSaver>();
                saver.Download(ShareDbo.DbPath);
                await ServiceHelper.GetService<IAlertService>().ShowAlertAsync("Bourse", "Base de données dans Downloads", "Ok");
            }
            catch (Exception ex)
            {
                await ServiceHelper.GetService<IAlertService>().ShowAlertAsync("Bourse", ex.Message, "Ok");
            }
        }

        /// <summary>
        /// Rafraichissement des données via la base de données
        /// </summary>
        public void Load()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += OnWork;
            worker.RunWorkerCompleted += OnCompleted;
            worker.RunWorkerAsync(); 
        }

        /// <summary>
        /// Rafraichissement des données via la base de données
        /// </summary>
        private void OnWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = Business.Share.Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Rafraichissement des données via la base de données
        /// </summary>
        private void OnCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is List<Business.Share> data)
            {
                var items = ShareViewModel.Convert(data);
                Items = new ObservableCollection<ShareViewModel>(items.OrderBy(_=>_.Consensus));
                CanDowload = data.Any(_=>_.ShouldUpdate);
            }
        }

    }
}
