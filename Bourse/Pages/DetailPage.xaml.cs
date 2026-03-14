using System.ComponentModel.DataAnnotations;


namespace Bourse.Pages
{
    /// <summary>
    /// Détail d'une action, permet de créer ou modifier une action
    /// </summary>
    public partial class DetailPage : ContentPage, IQueryAttributable
    {
        /// <summary>
        /// Détail de l'action
        /// </summary>
        private Business.Share? _item;

        public DetailPage()
        {
            InitializeComponent();
            AppShell.SetNavBarIsVisible(this, false);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("item", out var obj) && obj is Business.Share item)
            {
                _item = item;
                Code.Text = item.Code;
                Name.Text = item.Name;
                Url.Text = item.Url;
                SwitchCAC40.IsToggled = item.IsCac40;
            }
            else
            {
                Code.Text = "FR0000000000";
                Url.Text = "https://www.boursorama.com/cours/";
            }
        }

        /// <summary>
        /// Validation des données et création ou mise à jour de l'action, puis retour à la page précédente
        /// </summary>
        private async void OnValidateClicked(object sender, EventArgs e)
        {
            try
            {
                Validation();
                if (_item == null)
                {
                    _item = Business.Share.Create(Code.Text, Name.Text, Url.Text, SwitchCAC40.IsToggled);
                }
                else
                {
                    _item.Update(Code.Text, Name.Text, Url.Text, SwitchCAC40.IsToggled);
                }
                _item.Fetch();


                await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    { "Retour", _item }
                });
            }
            catch (ValidationException vex)
            {
                await DisplayAlert("Validation", vex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Vérification des données saisies, lance une ValidationException en cas d'erreur
        /// </summary>
        private void Validation()
        {   
            if (string.IsNullOrEmpty(Code.Text))
            {
                throw new ValidationException("Le code ne peut pas être vide.");
            }
            if (Code.Text == "FR0000000000")
            {
                throw new ValidationException("Le code ne peut pas être FR0000000000.");
            }
            if (string.IsNullOrEmpty(Name.Text))
            {
                throw new ValidationException("Le libellé ne peut pas être vide.");
            }
            if (string.IsNullOrEmpty(Url.Text))
            {
                throw new ValidationException("L'url' ne peut pas être vide.");
            }
            if (!Url.Text.StartsWith("https://www"))
            {
                throw new ValidationException("L'url' est invcalide.");
            }
        }
    }
}