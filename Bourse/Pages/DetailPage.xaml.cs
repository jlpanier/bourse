using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Bourse.Pages
{
    public partial class DetailPage : ContentPage, IQueryAttributable
    {
        Business.Share? _item;

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