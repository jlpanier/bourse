using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFImageLoading.Helpers;
using Bourse.Interfaces;
using Bourse.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Repository.Dbo;

namespace Bourse.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Business.Share> items;

         public MainViewModel()
         {
            Items = new ObservableCollection<Business.Share>();
         }

        [RelayCommand]
        async Task Add()
        {
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}");
        }

        [RelayCommand]
        async Task Delete(Business.Share item)
        {
            item.Remove();
            Load();
        }

        [RelayCommand]
        async Task Edit(Business.Share item)
        {
            await Modif(item);
        }

        [RelayCommand]
        private void Action()
        {
        }

        async Task Modif(Business.Share item)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                ["item"] = item
            };
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}", navigationParameters);
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
                Items = new ObservableCollection<Business.Share>(data);
            }
        }

    }
}
