using Bourse.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
                const int max = 3;
                double coeff = (max-Consensus+1.2)/max;
                int green = Math.Max(Math.Min((int)(coeff * 255),255),0);
                int red = (int)(255 - green);
                return Color.FromRgb(red, green, 128);
            
            }
        }

        /// <summary>
        /// Hauteur jaune de la barre 
        /// </summary>
        public int HeighRate
        {
            get
            {
                if (_heightRate == null)
                {
                    // Rendement : 0 -> 10%
                    // Hauteur max 30
                    _heightRate = Math.Min((int)(Rendement * 30000 / 100), 30);
                }
                return _heightRate ?? 0;
            }
        }
        private int? _heightRate;

        public int HeighConcensus
        {
            get
            {
                if (_heightConcensus == null)
                {
                    // Concensus : 4.0 -> 1.0
                    // Hauteur max 30
                    int valeur = (int)((45 - (15 * Consensus)));
                    _heightConcensus = Math.Max(Math.Min(valeur, 30), 2);
                }
                return _heightConcensus ?? 0;
            }
        }
        private int? _heightConcensus;

        public int HeighRisk
        {
            get
            {
                if (_heightRisk == null)
                {
                    // Risk : 0.0 -> 10.0%
                    // Hauteur max 30
                    _heightRisk = Math.Max(Math.Min((int)(30-(1.5*Risk)), 30),2);
                }
                return _heightRisk ?? 0;
            }
        }
        private int? _heightRisk;

        public readonly Business.Share Item;

        #region Propriétés

        public string Code => Item.Code;

        public string Cac => Item.IsCac40 ? "CAC" : string.Empty;

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
