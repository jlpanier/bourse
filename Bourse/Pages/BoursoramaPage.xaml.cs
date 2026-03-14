
namespace Bourse.Pages;

/// <summary>
/// Webview affichage de la page boursorama de l'action sélectionnée
/// </summary>
public partial class BoursoramaPage : ContentPage, IQueryAttributable
{
	public BoursoramaPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("item", out var obj) && obj is Business.Share item)
        {
            Browser.Source = item.Url;
        }
     }
}