using Bourse.Interfaces;
using Bourse.Pages;
using Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Maui.Mvvm;
using FFImageLoading.Helpers;
using Repository.Dbo;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Bourse.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
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
        async Task Delete(ShareViewModel item)
        {
            item.Remove();
            Items.Remove(item);
            OnPropertyChanged(nameof(Items));
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
        private void Update()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += OnUpdateWork;
            worker.RunWorkerCompleted += OnUpdateCompleted;
            worker.RunWorkerAsync();
        }

        private void OnUpdateWork(object? sender, DoWorkEventArgs e)
        {
            Items.ForEach(_=>_.Fetch());
        }

        private void OnUpdateCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Load();
        }

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

        public void Init()
        {
            Load();
        }

        private void Load()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += OnWork;
            worker.RunWorkerCompleted += OnCompleted;
            worker.RunWorkerAsync(); 
        }

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

        private void OnCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is List<Business.Share> data)
            {
                var items = ShareViewModel.Convert(data);
                Items = new ObservableCollection<ShareViewModel>(items.OrderBy(_=>_.Consensus));  
            }
        }

    }
}
