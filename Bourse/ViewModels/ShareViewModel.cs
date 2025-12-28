using Bourse.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Bourse.ViewModels
{
    public partial class ShareViewModel : ObservableObject
    {
        public static ObservableCollection<ShareViewModel> Convert(List<Business.Share> data)
        {
            var items = new List<ShareViewModel>();
            data.ForEach(_ => items.Add(new ShareViewModel(_)));
            return new ObservableCollection<ShareViewModel>(items);
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
