using Bourse.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bourse.ViewModels
{
    /// <summary>
    /// ViewModdel de chaque action
    /// </summary>
    public partial class ShareViewModel : ObservableObject
    {
        /// <summary>
        /// Conversion ViewModdel de chaque action
        /// </summary>
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

        /// <summary>
        /// Couleur de background reflètant le consensus
        /// </summary>
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
        /// Hauteur jaune de la barre taux
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

        /// <summary>
        /// Hauteur verte de la barre concensus
        /// </summary>
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

        /// <summary>
        /// Hauteur rouge de la barre risk
        /// </summary>
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

        /// <summary>
        /// entité de la base de données
        /// </summary>
        public readonly Business.Share Item;

        #region Propriétés

        /// <summary>
        /// Code de l'action
        /// </summary>
        public string Code => Item.Code;

        /// <summary>
        /// CAC si action du CAC40 vide sion
        /// </summary>
        public string Cac => Item.IsCac40 ? $"CAC" : string.Empty;

        /// <summary>
        /// Nom de l'action
        /// </summary>
        public string Name => Item.ShouldUpdate ? $"\U0001F504 {Item.Name}" : Item.Name;

        /// <summary>
        /// Montant de l'action
        /// </summary>
        public double Amount => Item.Amount;

        /// <summary>
        /// Rendement de l'action
        /// </summary>
        public double Rendement => Item.Rendement;

        /// <summary>
        /// Risque boursorama associé à l'action
        /// </summary>
        public double Risk => Item.Risk;

        /// <summary>
        /// Concensus boursorama associé à l'action
        /// </summary>
        public double Consensus => Item.Consensus;

        #endregion

        private ShareViewModel(Business.Share item)
        {
            Item = item;
        }

        /// <summary>
        /// Charge les valeurs de l'action en consultant la page boursorama
        /// </summary>
        /// <returns>VRAI, si mise à jour effectuée</returns>
        public bool Fetch() => Item.Fetch();

    }
}
