using Bourse.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Bourse.ViewModels
{
    public partial class ShareViewModel : ObservableObject
    {
        public static List<ShareViewModel> Convert(List<Business.Share> data)
        {
            var items = new List<ShareViewModel>();
            data.ForEach(_ => items.Add(new ShareViewModel(_)));
            return items;
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
        }

        public Color BackgroundColor 
        {
            get
            {
                const int max = 4;
                double coeff = (max-Consensus+1)/max;
                int green = Math.Min((int)(coeff * 255),255);
                int red = (int)(255 - green);
                return Color.FromRgb(red, green, (red+green)/2);
            
            }
        }

        public readonly Business.Share Item;

        #region Propriétés

        public string Code => Item.Code;

        public string Name => Item.Name;

        public double Amount => Item.Amount;

        public double Rendement => Item.Rendement;

        public double Risk => Item.Risk;

        public double Consensus => Item.Consensus;

        #endregion

        private ShareViewModel(Business.Share item)
        {
            Item = item;
        }

        public void Fetch() => Item.Fetch();

        public void Remove() => Item.Remove();
    }
}
